using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace SPSIN
{
    public abstract class ConfigurationPackageHandler
    {
        public virtual List<SPFeatureDefinition> GetConfigurationPackages(SPContext context)
        {
            throw new NotImplementedException();
        }

        public virtual void AddConfigurationPackage(string featureID, SPWeb targetWeb)
        {
            throw new NotImplementedException();
        }
    }
}
