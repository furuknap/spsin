using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPSIN.EventHandlers.Features
{
    public class SPSINSetupReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;

            // Content Types
            if (web.Site.Features[new Guid("0d3ea6d3-6fda-4466-a35d-27f888160ad1")] == null)
            {
                web.Site.Features.Add(new Guid("0d3ea6d3-6fda-4466-a35d-27f888160ad1"));
            }

            // List
            if (web.Features[new Guid("dd11f199-ac1f-451b-a4d2-badb7f42460b")] == null)
            {
                web.Features.Add(new Guid("dd11f199-ac1f-451b-a4d2-badb7f42460b"));
            }

            // DelegateControl
            if (web.Features[new Guid("57985636-1626-4b31-9319-bf9931d904a8")] == null)
            {
                web.Features.Add(new Guid("57985636-1626-4b31-9319-bf9931d904a8"));
            }

            // Site Settings
            if (web.Features[new Guid("5768a1ee-3bf0-4993-b0ec-6bd82da0504d")] == null)
            {
                web.Features.Add(new Guid("5768a1ee-3bf0-4993-b0ec-6bd82da0504d"));
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;

            // Content Types
            if (web.Site.Features[new Guid("0d3ea6d3-6fda-4466-a35d-27f888160ad1")] != null)
            {
                web.Site.Features.Remove(new Guid("0d3ea6d3-6fda-4466-a35d-27f888160ad1"));
            }

            // List
            if (web.Features[new Guid("dd11f199-ac1f-451b-a4d2-badb7f42460b")] != null)
            {
                web.Features.Remove(new Guid("dd11f199-ac1f-451b-a4d2-badb7f42460b"));
            }

            // DelegateControl
            if (web.Features[new Guid("57985636-1626-4b31-9319-bf9931d904a8")] != null)
            {
                web.Features.Remove(new Guid("57985636-1626-4b31-9319-bf9931d904a8"));
            }

            // Site Settings
            if (web.Features[new Guid("5768a1ee-3bf0-4993-b0ec-6bd82da0504d")] != null)
            {
                web.Features.Remove(new Guid("5768a1ee-3bf0-4993-b0ec-6bd82da0504d"));
            }

        }


        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {

        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {

        }
    }
}
