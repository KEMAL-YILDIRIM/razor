﻿// <auto-generated/>
#pragma warning disable 1591
namespace AspNetCoreGeneratedDocument
{
    #line default
    using TModel = global::System.Object;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
    #line default
    #line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute("Identifier", "/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml")]
    [global::System.Runtime.CompilerServices.CreateNewOnMetadataUpdateAttribute]
    #nullable restore
    internal sealed class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_Markup_InCodeBlocks : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
  
    var people = new Person[]
    {
        new Person() { Name = "Taylor", Age = 95, }
    };

    void PrintName(Person person)
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
        __o = person.Name;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
                               
    }

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
   PrintName(people[0]) 

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
   AnnounceBirthday(people[0]); 

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 17 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
            
    void AnnounceBirthday(Person person)
    {
        var formatted = $"Mr. {person.Name}";
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 22 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
                           __o = formatted;

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
              

        

#line default
#line hidden
#nullable disable
#nullable restore
#line 26 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
         for (var i = 0; i < person.Age / 10; i++)
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
            __o = i;

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
                                         
        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
             

        if (person.Age < 20)
        {
            return;
        }

        

#line default
#line hidden
#nullable disable
#nullable restore
#line 37 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Markup_InCodeBlocks.cshtml"
                               
    }

    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

#line default
#line hidden
#nullable disable
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
