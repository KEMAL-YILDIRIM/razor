﻿#pragma checksum "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "c9cc7d097735e4074bc4f71cd46101fc821fdb0592f90d491f09fdecddf3caf3"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCoreGeneratedDocument.TestFiles_IntegrationTests_CodeGenerationIntegrationTest_ExpressionsInCode), @"mvc.1.0.view", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml")]
namespace AspNetCoreGeneratedDocument
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
    #line default
    #line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"c9cc7d097735e4074bc4f71cd46101fc821fdb0592f90d491f09fdecddf3caf3", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute("Identifier", "/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml")]
    [global::System.Runtime.CompilerServices.CreateNewOnMetadataUpdateAttribute]
    #nullable restore
    internal sealed class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_ExpressionsInCode : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line (1,3)-(4,1) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"

    object foo = null;
    string bar = "Foo";

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n");
#nullable restore
#line (6,2)-(7,5) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"
if(foo != null) {
    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line (7,6)-(7,9) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"
foo

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line (7,9)-(9,1) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"

} else {

#line default
#line hidden
#nullable disable

            WriteLiteral("    <p>Foo is Null!</p>\r\n");
#nullable restore
#line (10,1)-(11,1) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"
}

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n<p>\r\n");
#nullable restore
#line (13,2)-(14,5) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"
if(!String.IsNullOrEmpty(bar)) {
    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line (14,7)-(14,28) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"
bar.Replace("F", "B")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line (14,29)-(16,1) "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/ExpressionsInCode.cshtml"

}

#line default
#line hidden
#nullable disable

            WriteLiteral("</p>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
