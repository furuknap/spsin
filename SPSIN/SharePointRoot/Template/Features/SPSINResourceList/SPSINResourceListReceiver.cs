using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPSIN.EventHandlers.Features
{
    public class SPSINResourceListReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;

            SPList resourceList = web.Lists[Utilities.GetResourceListName(web)];
            resourceList.Hidden = true;
            resourceList.ContentTypesEnabled = true;

            // Set up content types
            SPContentTypeId scriptWebCTID = new SPContentTypeId("0x01005288A3160BABA04DAA0B8A015A3D490101");
            SPContentTypeId bestMatchScript = resourceList.ContentTypes.BestMatch(new SPContentTypeId("0x01005288A3160BABA04DAA0B8A015A3D490101"));
            if (!bestMatchScript.IsChildOf(scriptWebCTID))
            {
                SPContentType scriptCT = web.AvailableContentTypes[new SPContentTypeId("0x01005288A3160BABA04DAA0B8A015A3D490101")];
                resourceList.ContentTypes.Add(scriptCT);
            }

            SPContentTypeId styleWebCTID = new SPContentTypeId("0x01005288A3160BABA04DAA0B8A015A3D490102");
            SPContentTypeId bestMatchStyle = resourceList.ContentTypes.BestMatch(styleWebCTID);
            if (!bestMatchStyle.IsChildOf(styleWebCTID))
            {
                SPContentType StyleCT = web.AvailableContentTypes[styleWebCTID];
                resourceList.ContentTypes.Add(StyleCT);
            }

            SPContentTypeId itemCTId = resourceList.ContentTypes.BestMatch(new SPContentTypeId("0x01"));

            resourceList.ContentTypes[itemCTId].Delete();

            resourceList.Update();

            // Set up default view
            SPView defaultView = resourceList.DefaultView;
            defaultView.Query = "<OrderBy><FieldRef Name=\"EvaluationOrder\" /></OrderBy>";
            if (!defaultView.ViewFields.Exists("EvaluationOrder"))
            {
                defaultView.ViewFields.Add("EvaluationOrder");
            }
            defaultView.Update();



        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {

        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {

        }

    }
}
