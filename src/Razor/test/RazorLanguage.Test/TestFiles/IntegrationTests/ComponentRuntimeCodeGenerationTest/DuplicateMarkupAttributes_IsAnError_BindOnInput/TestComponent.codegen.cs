// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
    public partial class TestComponent : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddMarkupContent(1, "\r\n  ");
            __builder.OpenElement(2, "input");
            __builder.AddAttribute(3, "type", "text");
            __builder.AddAttribute(4, "oninput", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.ChangeEventArgs>(this, 
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
                                                                               () => {}

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(5, "value", Microsoft.AspNetCore.Components.BindConverter.FormatValue(
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
                                   text

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(6, "oninput", Microsoft.AspNetCore.Components.EventCallback.Factory.CreateBinder(this, __value => text = __value, text));
            __builder.SetUpdatesAttributeName("value");
            __builder.CloseElement();
            __builder.AddMarkupContent(7, "\r\n");
            __builder.CloseElement();
        }
        #pragma warning restore 1998
#nullable restore
#line 5 "x:\dir\subdir\Test\TestComponent.cshtml"
            
    private string text = "hi";

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
