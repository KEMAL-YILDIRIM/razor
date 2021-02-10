﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.LanguageServer.ContainedLanguage;
using Microsoft.VisualStudio.LanguageServer.ContainedLanguage.MessageInterception;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.LanguageServerClient.Razor.Feedback;
using Microsoft.VisualStudio.LanguageServerClient.Razor.HtmlCSharp;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.LanguageServerClient.Razor
{
    [Export(typeof(MessageInterceptor))]
    [InterceptMethod(Methods.TextDocumentPublishDiagnosticsName)]
    internal class RazorHtmlPublishDiagnosticsInterceptor : MessageInterceptor
    {
        private readonly LSPDocumentManager _documentManager;
        private readonly LSPDiagnosticsTranslator _diagnosticsProvider;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RazorHtmlPublishDiagnosticsInterceptor(
            LSPDocumentManager documentManager,
            LSPDiagnosticsTranslator diagnosticsProvider,
            HTMLCSharpLanguageServerFeedbackFileLoggerProvider loggerProvider)
        {
            if (documentManager is null)
            {
                throw new ArgumentNullException(nameof(documentManager));
            }

            if (diagnosticsProvider is null)
            {
                throw new ArgumentNullException(nameof(diagnosticsProvider));
            }

            if (loggerProvider == null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            _documentManager = documentManager;
            _diagnosticsProvider = diagnosticsProvider;

            _logger = loggerProvider.CreateLogger(nameof(RazorHtmlPublishDiagnosticsInterceptor));
        }

        public override async Task<InterceptionResult> ApplyChangesAsync(JToken token, string containedLanguageName, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var diagnosticParams = token.ToObject<VSPublishDiagnosticParams>();

            if (diagnosticParams?.Uri is null)
            {
                throw new ArgumentException("Conversion of token failed.");
            }

            _logger.LogInformation($"Received HTML Publish diagnostic request for {diagnosticParams.Uri} with {diagnosticParams.Diagnostics.Length} diagnostics.");

            // We only support interception of Virtual HTML Files
            if (!RazorLSPConventions.IsVirtualHtmlFile(diagnosticParams.Uri))
            {
                return CreateDefaultResponse(token);
            }

            var htmlDocumentUri = diagnosticParams.Uri;
            var razorDocumentUri = RazorLSPConventions.GetRazorDocumentUri(htmlDocumentUri);

            // Note; this is an `interceptor` & not a handler, hence
            // it's possible another interceptor mutates this request
            // later in the toolchain. Such an interceptor would likely
            // expect a `__virtual.html` suffix instead of `.razor`.
            diagnosticParams.Uri = razorDocumentUri;

            if (!_documentManager.TryGetDocument(razorDocumentUri, out var razorDocumentSnapshot))
            {
                _logger.LogInformation($"Failed to find document {razorDocumentUri}.");
                return CreateEmptyDiagnosticsResponse(diagnosticParams);
            }

            if (!razorDocumentSnapshot.TryGetVirtualDocument<HtmlVirtualDocumentSnapshot>(out var htmlDocumentSnapshot) ||
                !htmlDocumentSnapshot.Uri.Equals(htmlDocumentUri))
            {
                _logger.LogInformation($"Failed to find virtual HTML document {htmlDocumentUri}.");
                return CreateEmptyDiagnosticsResponse(diagnosticParams);
            }

            // Return early if there aren't any diagnostics to process
            if (diagnosticParams.Diagnostics?.Any() != true)
            {
                _logger.LogInformation("No diagnostics to process.");
                return CreateResponse(diagnosticParams);
            }

            var processedDiagnostics = await _diagnosticsProvider.TranslateAsync(
                RazorLanguageKind.Html,
                razorDocumentUri,
                diagnosticParams.Diagnostics,
                cancellationToken
            ).ConfigureAwait(false);

            // Note it's possible the document version changed between when the diagnostics were created
            // and when we finished remapping the diagnostics. This could result in lingering / misaligned diagnostics.
            // We're choosing to do this over clearing out the diagnostics as that would lead to flickering.
            //
            // This'll need to be revisited based on preferences with flickering vs lingering.

            _logger.LogInformation($"Returning {processedDiagnostics.Diagnostics.Length} diagnostics.");
            diagnosticParams.Diagnostics = processedDiagnostics.Diagnostics;

            return CreateResponse(diagnosticParams);


            static InterceptionResult CreateDefaultResponse(JToken token) =>
                new InterceptionResult(token, changedDocumentUri: false);

            static InterceptionResult CreateEmptyDiagnosticsResponse(VSPublishDiagnosticParams diagnosticParams)
            {
                diagnosticParams.Diagnostics = Array.Empty<Diagnostic>();
                return CreateResponse(diagnosticParams);
            }

            static InterceptionResult CreateResponse(VSPublishDiagnosticParams diagnosticParams)
            {
                var newToken = JToken.FromObject(diagnosticParams);
                return new InterceptionResult(newToken, changedDocumentUri: true);
            }
        }
    }
}
