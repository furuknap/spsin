using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint;

namespace SPSIN
{
    public abstract class SINCycleReceiver
    {
        public virtual void BeforeContextLoad(SINCycleContext context)
        {
        }
        public virtual void AfterContextLoad(SINCycleContext context)
        {
        }
        public virtual void BeforeResourceItemsLoad(SINCycleContext context)
        {
        }
        public virtual void AfterResourceItemsLoad(SINCycleContext context, List<SPListItem> ResourceItems)
        {
        }
        public virtual Boolean BeforeResourceItemShouldLoadEvaluation(SINCycleContext context, SPListItem resourceItem, Boolean ShouldLoadThisResource)
        {
            return ShouldLoadThisResource;
        }
        public virtual Boolean AfterResourceItemShouldLoadEvaluation(SINCycleContext context, SPListItem resourceItem, Boolean ShouldLoadThisResource)
        {
            return ShouldLoadThisResource;
        }
        public virtual string BeforeResourceItemGetResourceStringEvaluation(SINCycleContext context, SPListItem resourceItem)
        {
            return string.Empty;
        }

        public virtual string AfterResourceItemGetResourceStringEvaluation(SINCycleContext context, SPListItem resourceItem, string ResourceString)
        {
            return ResourceString;
        }

        public virtual string AfterProcessingComplete(SINCycleContext context, string Output)
        {
            return Output;
        }
    }

    public class SINCycleContext
    {
        public SPContext SPContext;
        public HttpContext HttpContext;
        public List<SPListItem> AllResourceItems;
        public SPListItem CurrentResourceItem;

    }

}
