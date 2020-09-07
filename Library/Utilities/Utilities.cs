using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Utilities
{
    public static class Utilities
    {
        public static string IsActive(this IHtmlHelper html,
                                  string control,
                                  string action,
                                  string routeid)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
            var routeId = (string)routeData.Values["id"];
            if (routeId==null)
            {
                routeId = "";
            }
            // both must match
            var returnActive = control == routeControl &&
                               action == routeAction && 
                               routeid == routeId;

            return returnActive ? "active" : "";
        }
        public static string AreBranchesActive(this IHtmlHelper html,
                                  string control,
                                  string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
            if (routeAction == "Detail" && routeControl == "Branch")
            {
                return "active";
            }
            // both must match
            return "";
        }
    }
}
