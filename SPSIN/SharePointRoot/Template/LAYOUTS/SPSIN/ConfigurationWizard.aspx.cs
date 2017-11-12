using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using System.Net;
using System.Reflection;

namespace SPSIN.ApplicationPages
{
    public partial class ConfigurationWizard : LayoutsPageBase
    {
        protected readonly Version thisVersion = Utilities.GetVersion();

        protected Panel SPSIN_PackagesPanel;
        protected Label SPSIN_Message;
        protected HyperLink SPSIN_UpdateLink;

        protected override void OnInit(EventArgs e)
        {
            SPSIN_Message.Text = "";

            try
            {
                WebClient client = new WebClient();
                string latestVersionString = client.DownloadString("http://spsin.com/latestversion.txt");
                Version latestVersion = new Version(latestVersionString);

                if (latestVersion > thisVersion)
                {
                    SPSIN_Message.Text = string.Format("There is a new version of SP SIN available ({1}). You are running version {0}. Please go to <a href=\"http://spsin.com/\">http://spsin.com/</a> to get the latest version.", thisVersion.ToString(), latestVersion.ToString());
                    SPSIN_UpdateLink.Visible = true;
                }
            }
            catch
            {
                SPSIN_Message.Text = string.Format("Unable to check for updated version. Please check the latest version number on <a href=\"http://spsin.com/latestversion.txt\">http://spsin.com/latestversion.txt</a> manually and compare to the current version ({0}) to see if you are running the latest version", thisVersion.ToString());
            }

        }

        protected override void CreateChildControls()
        {
            SPSIN_PackagesPanel.Controls.Clear();
            List<SPFeatureDefinition> features = Utilities.GetAllConfigurationPackages();

            Control allPackagesControls = new Control();

            foreach (SPFeatureDefinition feature in features)
            {
                if (feature.Properties["SPSIN_ConfigPackage_Title"] != null &&
                    !string.IsNullOrEmpty(feature.Properties["SPSIN_ConfigPackage_Title"].Value))
                {
                    allPackagesControls.Controls.Add(GetPackageControlForFeature(feature));
                }
            }

            if (allPackagesControls.Controls.Count > 0)
            {
                SPSIN_PackagesPanel.Controls.Add(allPackagesControls);
            }
            else
            {
                SPSIN_PackagesPanel.Controls.Add(new LiteralControl("No packages found. You may check out <a href=\"http://spsin.com/\">http://spsin.com/</a> to download packages for SP SIN."));
            }
        }

        private Control GetPackageControlForFeature(SPFeatureDefinition feature)
        {
            Control c = new Control();
            string name = feature.Properties["SPSIN_ConfigPackage_Title"].Value;
            string description = "No Description Available";

            if (feature.Properties["SPSIN_ConfigPackage_Description"] != null)
            {
                description = feature.Properties["SPSIN_ConfigPackage_Description"].Value;
            }

            string htmlString = string.Format(@"
<h2>{0}</h2>
<p>{1}</p>
", name, description);

            Button addButton = new Button();
            addButton.ID = "add_" + feature.Id;
            addButton.Click += new EventHandler(addButton_Click);
            addButton.Text = "Add " + name;

            Panel packagePanel = new Panel();
            packagePanel.Controls.Add(new LiteralControl(htmlString));
            packagePanel.Controls.Add(addButton);
            packagePanel.CssClass = "SPSIN-packagePanel";

            c.Controls.Add(packagePanel);

            return c;
        }

        void addButton_Click(object sender, EventArgs e)
        {
            SPContext context = SPContext.Current;

            if (context != null)
            {
                SPWeb web = context.Web;

                Control b = (Control)sender;
                string[] parts = b.ID.Split('_');
                if (parts.Length == 2 && parts[0] == "add")
                {
                    string featureID = parts[1];

                    SPFeatureDefinition def = SPFarm.Local.FeatureDefinitions[new Guid(featureID)];

                    if (def == null)
                    {
                        // Maybe sandbox...
                        if (SPFarm.Local.BuildVersion > new Version("14.0.0.0"))
                        {
                            //Load handler for sandbox solutions
                            try
                            {
                                ConfigurationPackageHandler handler = Utilities.GetConfigurationPackageHandlerForSandbox();
                                handler.AddConfigurationPackage(featureID, web);
                                SPSIN_Message.Text = "Configuration Added";
                            }
                            catch (Exception ex)
                            {
                                throw new SPException("Sorry, we cannot add that package. " + ex.Message);
                            }
                        }
                    }
                    else if (def.Scope == SPFeatureScope.Web)
                    {
                        if (web.Features[new Guid(featureID)] == null)
                        {
                            web.Features.Add(new Guid(featureID));
                            web.Features.Remove(new Guid(featureID));
                            SPSIN_Message.Text = "Configuration Added";

                        }
                    }
                    else
                    {
                        throw new SPException(string.Format("Sorry, the package with the feature ID {0} seems to be an invalid SP SIN configuration package. We even tried treating it as a sandbox solution. An SP SIN Configuration package needs to be web scoped (tell your developer) and you need to have permissions to activate the feature.", featureID));
                    }
                }

            }

        }
    }
}
