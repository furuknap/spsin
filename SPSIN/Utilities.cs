using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using System.Xml;
using System.Reflection;
using SPSIN.Handlers;

namespace SPSIN
{
    public class Utilities
    {
        protected static readonly Version thisVersion = new Version("1.2.3");
        public static readonly string SandBoxSolutionAssemblyName = "SPSIN.SandboxSolutionSupport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=29a1bc68ea6f4b3b";
        public static readonly string SandBoxSolutionAssemblyConfigurationPackageHandlerClass = "SPSIN.SandboxConfigurationPackageHandler";

        public static Version GetVersion()
        {
            return thisVersion;
        }

        public static ConfigurationPackageHandler GetConfigurationPackageHandlerForSandbox()
        {
            Assembly sbAssembly = Assembly.Load(Utilities.SandBoxSolutionAssemblyName);
            ConfigurationPackageHandler handler = (ConfigurationPackageHandler)sbAssembly.CreateInstance("SPSIN.SandboxConfigurationPackageHandler");

            return handler;
        }

        internal static List<SPListItem> GetAllItemsForContext(SPContext context, List<SINCycleReceiver> receivers)
        {
            HttpContext httpContext = HttpContext.Current;
            SPWeb web = context.Web;
            SPList resourceList = web.Lists[GetResourceListID(context)];

            SPView defaultView = resourceList.DefaultView;

            SPListItemCollection allResourceItemsRaw = resourceList.GetItems(defaultView);
            List<SPListItem> allResourceItems = new List<SPListItem>();

            List<SPListItem> returnItems = new List<SPListItem>();

            foreach (SPListItem rawItem in allResourceItemsRaw)
            {
                SPListItem item = resourceList.GetItemById(rawItem.ID);
                allResourceItems.Add(item);
            }

            foreach (SPListItem item in allResourceItems)
            {
                SPContentType ct = item.ContentType;
                string ctResourceConfiguration = ct.XmlDocuments["http://schemas.spsin.com/Resource"];
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ctResourceConfiguration);
                string receiverAssembly = xmlDoc["ScriptResourceReceiver"]["ReceiverAssembly"].InnerText;
                string receiverClass = xmlDoc["ScriptResourceReceiver"]["ReceiverClass"].InnerText;
                Assembly assembly = Assembly.Load(receiverAssembly);
                SPSINResourceHandler resourceItem = (SPSINResourceHandler)assembly.CreateInstance(receiverClass);
                SPSINContext newcontext = new SPSINContext();
                newcontext.Context = context;
                newcontext.Item = item;
                newcontext.HttpContext = httpContext;

                Boolean shouldEvaluate = true;
                
                foreach (SINCycleReceiver receiver in receivers)
                {
                    SINCycleContext scContext = new SINCycleContext();
                    scContext.CurrentResourceItem = item;
                    scContext.HttpContext = newcontext.HttpContext;
                    scContext.SPContext = newcontext.Context;
                    scContext.AllResourceItems = allResourceItems;

                    shouldEvaluate = receiver.BeforeResourceItemShouldLoadEvaluation(scContext, item, shouldEvaluate);

                }
                
                
                if (shouldEvaluate)
                {
                    Boolean shouldLoad = resourceItem.ShouldLoad(newcontext);

                    foreach (SINCycleReceiver receiver in receivers)
                    {
                        SINCycleContext scContext = new SINCycleContext();
                        scContext.CurrentResourceItem = item;
                        scContext.HttpContext = newcontext.HttpContext;
                        scContext.SPContext = newcontext.Context;
                        scContext.AllResourceItems = allResourceItems;

                        shouldLoad = receiver.AfterResourceItemShouldLoadEvaluation(scContext, item, shouldLoad);

                    }
                    if (shouldLoad)
                    {
                        returnItems.Add(item);
                    }
                }

            }

            return returnItems;
        }

        private static Guid GetResourceListID(SPContext context)
        {
            SPWeb web = context.Web;
            return GetResourceListID(web);
        }

        private static Guid GetResourceListID(SPWeb web)
        {
            string URL = GetResourceListInternalURL();
            string webURL = web.Url;

            if (!webURL.EndsWith("/"))
            {
                webURL += "/";
            }
            SPList list = web.GetList(webURL + URL);
            return list.ID;
        }

        public static string GetEvaluationOrderFieldName(SPWeb web)
        {
            SPList list = web.Lists[GetResourceListName(web)];
            return list.Fields.GetFieldByInternalName(GetEvaluationOrderFieldInternalName()).Title;
        }

        private static string GetEvaluationOrderFieldInternalName()
        {
            return "EvaluationOrder";
        }

        public static string GetResourceListAbsoluteURL(SPWeb web)
        {
            string url = web.Url;
            if (url == "/")
            {
                url += "";
            }
            url += GetResourceListServerRelativeURL(web);
            return url;
        }

        public static string GetResourceListServerRelativeURL(SPWeb web)
        {
            SPList list = web.Lists[GetResourceListName(web)];
            return list.DefaultView.ServerRelativeUrl;
        }

        public static string GetResourceListName(SPWeb web)
        {
            string URL = GetResourceListInternalURL();
            string webURL = web.Url;

            if (!webURL.EndsWith("/"))
            {
                webURL += "/";
            }
            SPList list = web.GetList(webURL + URL);
            return list.Title;
        }

        public static string GetResourceListName(SPContext context)
        {
            SPWeb web = context.Web;
            return GetResourceListName(web);
        }

        private static string GetResourceListInternalURL()
        {
            return "SPSINResources";
        }

        public static string GetResourceURLFieldName(SPWeb web)
        {
            SPList list = web.Lists[GetResourceListName(web)];
            return list.Fields.GetFieldByInternalName(GetResourceURLFieldInternalName()).Title;
        }

        private static string GetResourceURLFieldInternalName()
        {
            return "ResourceAddress";
        }

        internal static string GetURLFilterFieldName(SPWeb web)
        {
            SPList list = web.Lists[GetResourceListName(web)];
            return list.Fields.GetFieldByInternalName(GetFilterURLFieldInternalName()).Title;
        }

        private static string GetFilterURLFieldInternalName()
        {
            return "URLFilter";
        }

        internal static List<SPFeatureDefinition> GetAllConfigurationPackages()
        {
            return GetAllConfigurationPackages(SPContext.Current);
        }
        internal static List<SPFeatureDefinition> GetAllConfigurationPackages(SPContext context)
        {
            List<SPFeatureDefinition> featureDefinitions = new List<SPFeatureDefinition>();
            List<SPFeatureDefinition> allFeatures = new List<SPFeatureDefinition>();

            if (SPFarm.Local.BuildVersion > new Version("14.0.0.0"))
            {
                //Load handler for sandbox solutions
                try
                {
                    ConfigurationPackageHandler handler = GetConfigurationPackageHandlerForSandbox();
                    allFeatures = handler.GetConfigurationPackages(context);
                }
                catch
                {
                }
            }

            SPFeatureDefinitionCollection farmFeatures = SPFarm.Local.FeatureDefinitions;

            foreach (SPFeatureDefinition farmFeature in farmFeatures)
            {
                allFeatures.Add(farmFeature);
            }

            foreach (SPFeatureDefinition feature in allFeatures)
            {
                if (feature.Properties["SPSIN_ConfigPackage_Title"] != null &&
                    !string.IsNullOrEmpty(feature.Properties["SPSIN_ConfigPackage_Title"].Value))
                {
                    featureDefinitions.Add(feature);
                }
            }
            return featureDefinitions;
        }

        internal static List<SINCycleReceiver> LoadSINCycleReceivers(SPContext spContext)
        {
            SPWeb currentWeb = spContext.Web;
            List<SINCycleReceiver> receivers = new List<SINCycleReceiver>();
            SPFeatureCollection webFeatures = currentWeb.Features;

            List<KeyValuePair<Int32, SINCycleReceiver>> orderedReceivers = new List<KeyValuePair<int, SINCycleReceiver>>();

            foreach (SPFeature feature in webFeatures)
            {
                SPFeatureDefinition definition = feature.Definition;

                if (definition != null
                    &&
                    definition.Properties["SPSIN_SINCycleReceiver_Assembly"] != null
                    &&
                    definition.Properties["SPSIN_SINCycleReceiver_Class"] != null
                    &&
                    definition.Properties["SPSIN_SINCycleReceiver_Sequence"] != null
                    )
                {
                    try
                    {
                        string receiverAssembly = definition.Properties["SPSIN_SINCycleReceiver_Assembly"].Value;
                        string receiverClass = definition.Properties["SPSIN_SINCycleReceiver_Class"].Value;
                        string receiverSequenceString = definition.Properties["SPSIN_SINCycleReceiver_Sequence"].Value;

                        int receiverSequence;

                        if (!Int32.TryParse(receiverSequenceString, out receiverSequence))
                        {
                            // Invalid sequence, send to end of queue
                            receiverSequence = Int32.MaxValue;
                        }

                        Assembly assembly = Assembly.Load(receiverAssembly);
                        SINCycleReceiver receiverItem = (SINCycleReceiver)assembly.CreateInstance(receiverClass);

                        int insertAt = orderedReceivers.Count; // default to last position

                        for (int i = 0; i < orderedReceivers.Count; i++)
                        {
                            if (orderedReceivers[i].Key > receiverSequence)
                            {
                                insertAt = i;
                            }
                        }

                        orderedReceivers.Insert(insertAt, new KeyValuePair<int, SINCycleReceiver>(receiverSequence, receiverItem));

                    }
                    catch
                    {
                        // Error in loading...
                    }
                }
                else
                {
                    // Not a (correctly configured) SinCycle receiver
                }

            }

            foreach (KeyValuePair<int, SINCycleReceiver> receiver in orderedReceivers)
            {
                receivers.Add(receiver.Value);
            }

            return receivers;

        }
    }
}
