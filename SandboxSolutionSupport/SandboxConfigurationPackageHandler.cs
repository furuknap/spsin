using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace SPSIN
{
    public class SandboxConfigurationPackageHandler : ConfigurationPackageHandler
    {
        public override List<SPFeatureDefinition> GetConfigurationPackages(SPContext context)
        {
            List<SPFeatureDefinition> definitions = new List<SPFeatureDefinition>();
            foreach (SPFeatureDefinition feature in context.Site.FeatureDefinitions)
            {
                definitions.Add(feature);
            }
            return definitions;
        }

        public override void AddConfigurationPackage(string featureID, SPWeb targetWeb)
        {
            try
            {
                SPSite site = targetWeb.Site;

                if (site.FeatureDefinitions[new Guid(featureID)] != null)
                {
                    SPFeatureDefinition featureDef = site.FeatureDefinitions[new Guid(featureID)];
                    if (featureDef.Scope == SPFeatureScope.Web)
                    {
                        targetWeb.Features.Add(featureDef.Id,false, SPFeatureDefinitionScope.Site);
                        targetWeb.Features.Remove(featureDef.Id);

                    }
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
