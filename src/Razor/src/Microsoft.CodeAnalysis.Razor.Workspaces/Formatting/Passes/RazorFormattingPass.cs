﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Components;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Razor.Language.Syntax;
using Microsoft.AspNetCore.Razor.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using RazorRazorSyntaxNodeList = Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxList<Microsoft.AspNetCore.Razor.Language.Syntax.RazorSyntaxNode>;
using RazorSyntaxNode = Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode;
using RazorSyntaxNodeList = Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxList<Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode>;

namespace Microsoft.CodeAnalysis.Razor.Formatting;

internal sealed class RazorFormattingPass : IFormattingPass
{
    public async Task<FormattingResult> ExecuteAsync(FormattingContext context, FormattingResult result, CancellationToken cancellationToken)
    {
        // Apply previous edits if any.
        var originalText = context.SourceText;
        var changedText = originalText;
        var changedContext = context;
        if (result.Edits.Length > 0)
        {
            var changes = result.Edits.Select(originalText.GetTextChange).ToArray();
            changedText = changedText.WithChanges(changes);
            changedContext = await context.WithTextAsync(changedText).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
        }

        // Format the razor bits of the file
        var syntaxTree = changedContext.CodeDocument.GetSyntaxTree();
        var edits = FormatRazor(changedContext, syntaxTree);

        // Compute the final combined set of edits
        var formattingChanges = edits.Select(changedText.GetTextChange);
        changedText = changedText.WithChanges(formattingChanges);

        var finalChanges = changedText.GetTextChanges(originalText);
        var finalEdits = finalChanges.Select(originalText.GetTextEdit).ToArray();

        return new FormattingResult(finalEdits);
    }

    private static ImmutableArray<TextEdit> FormatRazor(FormattingContext context, RazorSyntaxTree syntaxTree)
    {
        using var edits = new PooledArrayBuilder<TextEdit>();
        var source = syntaxTree.Source;

        foreach (var node in syntaxTree.Root.DescendantNodes())
        {
            // Disclaimer: CSharpCodeBlockSyntax is used a _lot_ in razor so these methods are probably
            // being overly careful to only try to format syntax forms they care about.
            TryFormatCSharpBlockStructure(context, ref edits.AsRef(), source, node); // TODO
            TryFormatSingleLineDirective(ref edits.AsRef(), source, node);
            TryFormatBlocks(context, ref edits.AsRef(), source, node);
        }

        return edits.ToImmutable();
    }

    private static void TryFormatBlocks(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // We only want to run one of these
        _ = TryFormatFunctionsBlock(context, ref edits, source, node) ||
            TryFormatCSharpExplicitTransition(context, ref edits, source, node) ||
            TryFormatHtmlInCSharp(context, ref edits, source, node) ||
            TryFormatComplexCSharpBlock(context, ref edits, source, node) ||
            TryFormatSectionBlock(context, ref edits, source, node);
    }

    private static bool TryFormatSectionBlock(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // @section Goo {
        // }
        //
        // or
        //
        // @section Goo
        // {
        // }
        if (node is CSharpCodeBlockSyntax directiveCode &&
            directiveCode.Children is [RazorDirectiveSyntax directive] &&
            directive.DirectiveDescriptor?.Directive == SectionDirective.Directive.Directive &&
            directive.Body is RazorDirectiveBodySyntax { CSharpCode: { } code })
        {
            var children = code.Children;
            if (TryGetWhitespace(children, out var whitespaceBeforeSectionName, out var whitespaceAfterSectionName))
            {
                // For whitespace we normalize it differently depending on if its multi-line or not
                FormatWhitespaceBetweenDirectiveAndBrace(whitespaceBeforeSectionName, directive, ref edits, source, context, forceNewLine: false);
                FormatWhitespaceBetweenDirectiveAndBrace(whitespaceAfterSectionName, directive, ref edits, source, context, forceNewLine: false);

                return true;
            }
            else if (children.TryGetOpenBraceToken(out var brace))
            {
                // If there is no whitespace at all we normalize to a single space
                var edit = VsLspFactory.CreateTextEdit(brace.GetRange(source).Start, " ");
                edits.Add(edit);

                return true;
            }
        }

        return false;

        static bool TryGetWhitespace(RazorRazorSyntaxNodeList children, [NotNullWhen(true)] out CSharpStatementLiteralSyntax? whitespaceBeforeSectionName, [NotNullWhen(true)] out UnclassifiedTextLiteralSyntax? whitespaceAfterSectionName)
        {
            // If there is whitespace between the directive and the section name, and the section name and the brace, they will be in the first child
            // and third child of the 6 total children
            whitespaceBeforeSectionName = null;
            whitespaceAfterSectionName = null;
            if (children.Count == 6 &&
                children[0] is CSharpStatementLiteralSyntax before &&
                before.ContainsOnlyWhitespace() &&
                children[2] is UnclassifiedTextLiteralSyntax after &&
                after.ContainsOnlyWhitespace())
            {
                whitespaceBeforeSectionName = before;
                whitespaceAfterSectionName = after;

            }

            return whitespaceBeforeSectionName != null;
        }
    }

    private static bool TryFormatFunctionsBlock(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // @functions
        // {
        // }
        //
        // or
        //
        // @code
        // {
        // }
        if (node is CSharpCodeBlockSyntax { Children: [RazorDirectiveSyntax { Body: RazorDirectiveBodySyntax body } directive] })
        {
            if (!IsCodeOrFunctionsBlock(body.Keyword))
            {
                return false;
            }

            var csharpCodeChildren = body.CSharpCode.Children;
            if (!csharpCodeChildren.TryGetOpenBraceNode(out var openBrace) ||
                !csharpCodeChildren.TryGetCloseBraceNode(out var closeBrace))
            {
                // Don't trust ourselves in an incomplete scenario.
                return false;
            }

            var code = csharpCodeChildren.PreviousSiblingOrSelf(closeBrace) as CSharpCodeBlockSyntax;

            var openBraceNode = openBrace;
            var codeNode = code.AssumeNotNull();
            var closeBraceNode = closeBrace;

            return FormatBlock(context, source, directive, openBraceNode, codeNode, closeBraceNode, ref edits);
        }

        return false;
    }

    private static bool TryFormatCSharpExplicitTransition(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // We're looking for a code block like this:
        //
        // @{
        //     var x = 1;
        // }
        if (node is CSharpCodeBlockSyntax explicitCode &&
            explicitCode.Children.FirstOrDefault() is CSharpStatementSyntax statement &&
            statement.Body is CSharpStatementBodySyntax csharpStatementBody)
        {
            var openBraceNode = csharpStatementBody.OpenBrace;
            var codeNode = csharpStatementBody.CSharpCode;
            var closeBraceNode = csharpStatementBody.CloseBrace;

            return FormatBlock(context, source, directiveNode: null, openBraceNode, codeNode, closeBraceNode, ref edits);
        }

        return false;
    }

    private static bool TryFormatComplexCSharpBlock(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // complex situations like
        // @{
        //  void Method(){
        //      @(DateTime.Now)
        //  }
        // }
        if (node is CSharpRazorBlockSyntax csharpRazorBlock &&
            csharpRazorBlock.Parent is CSharpCodeBlockSyntax innerCodeBlock &&
            csharpRazorBlock.Parent.Parent is CSharpCodeBlockSyntax outerCodeBlock)
        {
            var codeNode = csharpRazorBlock;
            var openBraceNode = outerCodeBlock.Children.PreviousSiblingOrSelf(innerCodeBlock);
            var closeBraceNode = outerCodeBlock.Children.NextSiblingOrSelf(innerCodeBlock);

            return FormatBlock(context, source, directiveNode: null, openBraceNode, codeNode, closeBraceNode, ref edits);
        }

        return false;
    }

    private static bool TryFormatHtmlInCSharp(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // void Method()
        // {
        //     <div></div>
        // }
        if (node is MarkupBlockSyntax markupBlockNode &&
            markupBlockNode.Parent is CSharpCodeBlockSyntax cSharpCodeBlock)
        {
            var openBraceNode = cSharpCodeBlock.Children.PreviousSiblingOrSelf(markupBlockNode);
            var closeBraceNode = cSharpCodeBlock.Children.NextSiblingOrSelf(markupBlockNode);

            return FormatBlock(context, source, directiveNode: null, openBraceNode, markupBlockNode, closeBraceNode, ref edits);
        }

        return false;
    }

    private static void TryFormatCSharpBlockStructure(FormattingContext context, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // We're looking for a code block like this:
        //
        // @code {
        //    var x = 1;
        // }
        //
        // The nodes will be a grandchild of a RazorDirective (the "@code") and we expect there to be
        // at least three children, being:
        // 1. Optional whitespace
        // 2. The opening brace
        // 3. The C# code
        // 4. The closing brace
        if (node is CSharpCodeBlockSyntax code &&
            node.Parent?.Parent is RazorDirectiveSyntax directive &&
            !directive.ContainsDiagnostics &&
            directive.DirectiveDescriptor?.Kind == DirectiveKind.CodeBlock)
        {
            // If we're formatting a @code or @functions directive, the user might have indicated they always want a newline
            var forceNewLine = context.Options.CodeBlockBraceOnNextLine &&
                directive.Body is RazorDirectiveBodySyntax { Keyword: { } keyword } &&
                IsCodeOrFunctionsBlock(keyword);

            var children = code.Children;
            if (TryGetLeadingWhitespace(children, out var whitespace))
            {
                // For whitespace we normalize it differently depending on if its multi-line or not
                FormatWhitespaceBetweenDirectiveAndBrace(whitespace, directive, ref edits, source, context, forceNewLine);
            }
            else if (children.TryGetOpenBraceToken(out var brace))
            {
                // If there is no whitespace at all we normalize to a single space
                var edit = VsLspFactory.CreateTextEdit(
                    brace.GetRange(source).Start,
                    forceNewLine
                        ? context.NewLineString + FormattingUtilities.GetIndentationString(
                            directive.GetLinePositionSpan(source).Start.Character, context.Options.InsertSpaces, context.Options.TabSize)
                        : " ");

                edits.Add(edit);
            }
        }

        static bool TryGetLeadingWhitespace(RazorRazorSyntaxNodeList children, [NotNullWhen(true)] out UnclassifiedTextLiteralSyntax? whitespace)
        {
            // If there is whitespace between the directive and the brace, it will be in the first child
            // of the 4 total children
            whitespace = null;
            if (children.Count == 4 &&
                children[0] is UnclassifiedTextLiteralSyntax literal &&
                literal.ContainsOnlyWhitespace())
            {
                whitespace = literal;
            }

            return whitespace != null;
        }
    }

    private static void TryFormatSingleLineDirective(ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, RazorSyntaxNode node)
    {
        // Looking for single line directives like
        //
        // @attribute [Obsolete("old")]
        //
        // The CSharpCodeBlockSyntax covers everything from the end of "attribute" to the end of the line
        if (IsSingleLineDirective(node, out var children) || node.IsUsingDirective(out children))
        {
            // Shrink any block of C# that only has whitespace down to a single space.
            // In the @attribute case above this would only be the whitespace between the directive and code
            // but for @inject its also between the type and the field name.
            foreach (var child in children)
            {
                if (child.ContainsOnlyWhitespace(includingNewLines: false))
                {
                    ShrinkToSingleSpace(child, ref edits, source);
                }
            }
        }

        static bool IsSingleLineDirective(RazorSyntaxNode node, out RazorSyntaxNodeList children)
        {
            if (node is CSharpCodeBlockSyntax content &&
                node.Parent?.Parent is RazorDirectiveSyntax directive &&
                directive.DirectiveDescriptor?.Kind == DirectiveKind.SingleLine)
            {
                children = content.Children;
                return true;
            }

            children = default;
            return false;
        }
    }

    private static void FormatWhitespaceBetweenDirectiveAndBrace(RazorSyntaxNode node, RazorDirectiveSyntax directive, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source, FormattingContext context, bool forceNewLine)
    {
        if (node.ContainsOnlyWhitespace(includingNewLines: false) && !forceNewLine)
        {
            ShrinkToSingleSpace(node, ref edits, source);
        }
        else
        {
            // If there is a newline then we want to have just one newline after the directive
            // and indent the { to match the @
            var edit = VsLspFactory.CreateTextEdit(
                node.GetRange(source),
                context.NewLineString + FormattingUtilities.GetIndentationString(
                    directive.GetLinePositionSpan(source).Start.Character, context.Options.InsertSpaces, context.Options.TabSize));

            edits.Add(edit);
        }
    }

    private static void ShrinkToSingleSpace(RazorSyntaxNode node, ref PooledArrayBuilder<TextEdit> edits, RazorSourceDocument source)
    {
        // If there is anything other than one single space then we replace with one space between directive and brace.
        //
        // ie, "@code     {" will become "@code {"
        var edit = VsLspFactory.CreateTextEdit(node.GetRange(source), " ");
        edits.Add(edit);
    }

    private static bool FormatBlock(FormattingContext context, RazorSourceDocument source, RazorSyntaxNode? directiveNode, RazorSyntaxNode openBraceNode, RazorSyntaxNode codeNode, RazorSyntaxNode closeBraceNode, ref PooledArrayBuilder<TextEdit> edits)
    {
        var didFormat = false;

        var openBraceRange = openBraceNode.GetRangeWithoutWhitespace(source);
        var codeRange = codeNode.GetRangeWithoutWhitespace(source);

        if (openBraceRange is not null &&
            codeRange is not null &&
            openBraceRange.End.Line == codeRange.Start.Line &&
            !RangeHasBeenModified(ref edits, codeRange))
        {
            var additionalIndentationLevel = GetAdditionalIndentationLevel(context, openBraceRange, openBraceNode, codeNode);
            var newText = context.NewLineString;
            if (additionalIndentationLevel > 0)
            {
                newText += FormattingUtilities.GetIndentationString(additionalIndentationLevel, context.Options.InsertSpaces, context.Options.TabSize);
            }

            var edit = VsLspFactory.CreateTextEdit(openBraceRange.End, newText);
            edits.Add(edit);
            didFormat = true;
        }

        var closeBraceRange = closeBraceNode.GetRangeWithoutWhitespace(source);
        if (codeRange is not null &&
            closeBraceRange is not null &&
            !RangeHasBeenModified(ref edits, codeRange))
        {
            if (directiveNode is not null &&
                directiveNode.GetRange(source).Start.Character < closeBraceRange.Start.Character)
            {
                // If we have a directive, then we line the close brace up with it, and ensure
                // there is a close brace
                var edit = VsLspFactory.CreateTextEdit(start: codeRange.End, end: closeBraceRange.Start,
                    context.NewLineString + FormattingUtilities.GetIndentationString(
                        directiveNode.GetRange(source).Start.Character, context.Options.InsertSpaces, context.Options.TabSize));

                edits.Add(edit);
                didFormat = true;
            }
            else if (codeRange.End.Line == closeBraceRange.Start.Line)
            {
                // Add a Newline between the content and the "}" if one doesn't already exist.
                var edit = VsLspFactory.CreateTextEdit(codeRange.End, context.NewLineString);
                edits.Add(edit);
                didFormat = true;
            }
        }

        return didFormat;

        static bool RangeHasBeenModified(ref PooledArrayBuilder<TextEdit> edits, Range range)
        {
            // Because we don't always know what kind of Razor object we're operating on we have to do this to avoid duplicate edits.
            // The other way to accomplish this would be to apply the edits after every node and function, but that's not in scope for my current work.
            var hasBeenModified = edits.Any(e => e.Range.End == range.End);

            return hasBeenModified;
        }

        static int GetAdditionalIndentationLevel(FormattingContext context, Range range, RazorSyntaxNode openBraceNode, RazorSyntaxNode codeNode)
        {
            if (!context.TryGetIndentationLevel(codeNode.Position, out var desiredIndentationLevel))
            {
                // If for some reason we don't match a particular span use the indentation for the whole line
                var indentations = context.GetIndentations();
                var indentation = indentations[range.Start.Line];
                desiredIndentationLevel = indentation.HtmlIndentationLevel + indentation.RazorIndentationLevel;
            }

            var desiredIndentationOffset = context.GetIndentationOffsetForLevel(desiredIndentationLevel);
            var currentIndentationOffset = GetTrailingWhitespaceLength(openBraceNode, context) + GetLeadingWhitespaceLength(codeNode, context);

            return desiredIndentationOffset - currentIndentationOffset;

            static int GetLeadingWhitespaceLength(RazorSyntaxNode node, FormattingContext context)
            {
                var tokens = node.GetTokens();
                var whitespaceLength = 0;

                foreach (var token in tokens)
                {
                    if (token.IsWhitespace())
                    {
                        if (token.Kind == SyntaxKind.NewLine)
                        {
                            // We need to reset when we move to a new line.
                            whitespaceLength = 0;
                        }
                        else if (token.IsSpace())
                        {
                            whitespaceLength++;
                        }
                        else if (token.IsTab())
                        {
                            whitespaceLength += (int)context.Options.TabSize;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return whitespaceLength;
            }

            static int GetTrailingWhitespaceLength(RazorSyntaxNode node, FormattingContext context)
            {
                var tokens = node.GetTokens();
                var whitespaceLength = 0;

                for (var i = tokens.Count - 1; i >= 0; i--)
                {
                    var token = tokens[i];
                    if (token.IsWhitespace())
                    {
                        if (token.Kind == SyntaxKind.NewLine)
                        {
                            whitespaceLength = 0;
                        }
                        else if (token.IsSpace())
                        {
                            whitespaceLength++;
                        }
                        else if (token.IsTab())
                        {
                            whitespaceLength += (int)context.Options.TabSize;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return whitespaceLength;
            }
        }
    }

    private static bool IsCodeOrFunctionsBlock(RazorSyntaxNode keyword)
    {
        var keywordContent = keyword.GetContent();
        return keywordContent == FunctionsDirective.Directive.Directive ||
            keywordContent == ComponentCodeDirective.Directive.Directive;
    }
}
