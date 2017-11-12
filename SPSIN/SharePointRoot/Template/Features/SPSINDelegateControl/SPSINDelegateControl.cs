using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using System.Text;
using System.Xml;
using System.Reflection;
using SPSIN.Handlers;

namespace SPSIN
{
    public class SPSINDelegateControl : Control
    {
        private List<SINCycleReceiver> SINCycleReceivers = new List<SINCycleReceiver>();
        

        protected override void OnInit(EventArgs e)
        {
            SPContext spContext = SPContext.Current;

            if (spContext != null)
            {
                SINCycleReceivers = Utilities.LoadSINCycleReceivers(spContext);
            }
        }

        protected override void CreateChildControls()
        {
            try
            {
                foreach (SINCycleReceiver receiver in SINCycleReceivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    receiver.BeforeContextLoad(scContext);
                }

                SPContext currentcontext = SPContext.Current;
                HttpContext httpContext = HttpContext.Current;

                foreach (SINCycleReceiver receiver in SINCycleReceivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    scContext.SPContext = currentcontext;
                    scContext.HttpContext = httpContext;
                    receiver.AfterContextLoad(scContext);
                }

                foreach (SINCycleReceiver receiver in SINCycleReceivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    scContext.SPContext = currentcontext;
                    scContext.HttpContext = httpContext;
                    receiver.BeforeResourceItemsLoad(scContext);
                }

                List<SPListItem> allItems = Utilities.GetAllItemsForContext(currentcontext, SINCycleReceivers);

                foreach (SINCycleReceiver receiver in SINCycleReceivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    scContext.SPContext = currentcontext;
                    scContext.HttpContext = httpContext;
                    receiver.AfterResourceItemsLoad(scContext, allItems);
                }

                StringBuilder sb = new StringBuilder();

                foreach (SPListItem item in allItems)
                {
                    SPContentType ct = item.ContentType;
                    string ctResourceConfiguration = ct.XmlDocuments["http://schemas.spsin.com/Resource"];
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ctResourceConfiguration);
                    string receiverAssembly = xmlDoc["ScriptResourceReceiver"]["ReceiverAssembly"].InnerText;
                    string receiverClass = xmlDoc["ScriptResourceReceiver"]["ReceiverClass"].InnerText;
                    Assembly assembly = Assembly.Load(receiverAssembly);
                    SPSINResourceHandler resourceItem = (SPSINResourceHandler)assembly.CreateInstance(receiverClass);
                    SPSINContext context = new SPSINContext();
                    context.Context = currentcontext;
                    context.Item = item;
                    context.HttpContext = httpContext;

                    string resourceString = "";

                    foreach (SINCycleReceiver receiver in SINCycleReceivers)
                    {
                        SINCycleContext scContext = new SINCycleContext();
                        scContext.SPContext = currentcontext;
                        scContext.HttpContext = httpContext;
                        scContext.AllResourceItems = allItems;
                        scContext.CurrentResourceItem = item;
                        resourceString += receiver.BeforeResourceItemGetResourceStringEvaluation(scContext, item);
                    }

                    resourceString+=resourceItem.GetResourceString(context);

                    foreach (SINCycleReceiver receiver in SINCycleReceivers)
                    {
                        SINCycleContext scContext = new SINCycleContext();
                        scContext.SPContext = currentcontext;
                        scContext.HttpContext = httpContext;
                        scContext.AllResourceItems = allItems;
                        scContext.CurrentResourceItem = item;
                        resourceString = receiver.AfterResourceItemGetResourceStringEvaluation(scContext, item, resourceString);
                    }

                    sb.Append(resourceString);
                    sb.Append(Environment.NewLine);
                }

                string output = sb.ToString();
                foreach (SINCycleReceiver receiver in SINCycleReceivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    scContext.SPContext = currentcontext;
                    scContext.HttpContext = httpContext;
                    scContext.AllResourceItems = allItems;
                    output = receiver.AfterProcessingComplete(scContext, output);
                }
                
                Controls.Add(new LiteralControl(output));
            }
            catch (Exception e)
            {
                Controls.Add(new LiteralControl(string.Format(@"<!-- SP SIN Encountered an exception while processing: {0} -->", e.ToString())));
            }
        }

    }
}
