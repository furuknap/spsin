using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;

namespace SPSIN.Handlers
{
    public abstract class SPSINResourceHandler
    {
        public virtual bool ShouldLoad(SPSINContext context)
        {
            return true;
        }

        public virtual string GetResourceString(SPSINContext context)
        {
            return string.Empty;
        }

    }

    public class SPSINContext
    {
        public SPContext Context;
        public SPListItem Item;
        public HttpContext HttpContext;
    }
}
