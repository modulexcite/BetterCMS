﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BetterCms.Module.Root.Views.Sidebar
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 1 "..\..\Views\Sidebar\Footer.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Sidebar/Footer.cshtml")]
    public partial class Footer : System.Web.Mvc.WebViewPage<string>
    {
        public Footer()
        {
        }
        public override void Execute()
        {
WriteLiteral("<div");

WriteLiteral(" class=\"bcms-sidemenu-footer\"");

WriteLiteral(">\r\n    <span");

WriteLiteral(" class=\"bcms-version-number\"");

WriteLiteral(">");

            
            #line 4 "..\..\Views\Sidebar\Footer.cshtml"
                                 Write(Model);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n    <div");

WriteAttribute("title", Tuple.Create(" title=\"", 160), Tuple.Create("\"", 213)
            
            #line 5 "..\..\Views\Sidebar\Footer.cshtml"
, Tuple.Create(Tuple.Create("", 168), Tuple.Create<System.Object, System.Int32>(RootGlobalization.Sidebar_Footer_DragTooltip
            
            #line default
            #line hidden
, 168), false)
);

WriteLiteral(" id=\"bcms-sidemenu-position-handle\"");

WriteLiteral(" class=\"bcms-sidemenu-position-handle\"");

WriteLiteral(">\r\n        <span");

WriteLiteral(" class=\"bcms-sidemenu-stick-text\"");

WriteLiteral(">");

            
            #line 6 "..\..\Views\Sidebar\Footer.cshtml"
                                          Write(RootGlobalization.Sidebar_Footer_Right_DragTitle);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n    </div>\r\n</div>");

        }
    }
}
#pragma warning restore 1591
