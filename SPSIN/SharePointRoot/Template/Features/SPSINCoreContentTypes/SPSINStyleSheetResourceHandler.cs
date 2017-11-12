using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSIN.Handlers
{
    public class StyleSheetResourceHandler : SPSINResourceHandler
    {
        public override string GetResourceString(SPSINContext context)
        {
            string resourceURL = context.Item[Utilities.GetResourceURLFieldName(context.Context.Web)].ToString();
            resourceURL = SPUtility.GetServerRelativeUrlFromPrefixedUrl(resourceURL);
            string returnString = string.Format(@"<link rel=""stylesheet"" type=""text/css"" href=""{0}""/>", resourceURL);

            return returnString;
        }

        public override bool ShouldLoad(SPSINContext context)
        {
            SPListItem item = context.Item;
            SPContext spcontext = context.Context;
            HttpContext httpContext = context.HttpContext;

            bool result = true;

            string filter = "";
            string currentURL = httpContext.Request.Url.PathAndQuery;
            string filterURLFieldName = Utilities.GetURLFilterFieldName(spcontext.Web);
            if (item[filterURLFieldName] != null)
            {
                filter = item[filterURLFieldName].ToString();
            }

            if (!string.IsNullOrEmpty(filter))
            {
                if (currentURL.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }


    }
}
