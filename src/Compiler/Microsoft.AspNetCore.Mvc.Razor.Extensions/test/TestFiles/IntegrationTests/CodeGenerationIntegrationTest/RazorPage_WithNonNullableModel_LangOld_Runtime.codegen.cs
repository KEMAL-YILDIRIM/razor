﻿#pragma checksum "TestFiles\IntegrationTests\CodeGenerationIntegrationTest\test.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "938aa77e05d0524687967a4964c7424054e2f7b837772fe19895341aa7ef5bda"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCoreGeneratedDocument.TestFiles_IntegrationTests_CodeGenerationIntegrationTest_test), @"mvc.1.0.razor-page", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/test.cshtml")]
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
#nullable restore
#line (2,2)-(3,1) "TestFiles\IntegrationTests\CodeGenerationIntegrationTest\test.cshtml"
using TestNamespace

#line default
#line hidden
#nullable disable
    ;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"938aa77e05d0524687967a4964c7424054e2f7b837772fe19895341aa7ef5bda", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/test.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute("Identifier", "/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/test.cshtml")]
    [global::System.Runtime.CompilerServices.CreateNewOnMetadataUpdateAttribute]
    #nullable restore
    internal sealed class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_test : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<h1>");
            Write(
#nullable restore
#line (5,6)-(5,16) "TestFiles\IntegrationTests\CodeGenerationIntegrationTest\test.cshtml"
Model.Name

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</h1>\r\n\r\n<h2>");
            Write(
#nullable restore
#line (7,6)-(7,20) "TestFiles\IntegrationTests\CodeGenerationIntegrationTest\test.cshtml"
Model?.Address

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</h2>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TestModel> Html { get; private set; } = default!;
        #nullable disable
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<TestModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<TestModel>)PageContext?.ViewData;
        public TestModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
