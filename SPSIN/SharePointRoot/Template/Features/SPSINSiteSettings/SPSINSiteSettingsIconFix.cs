using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;

namespace SPSIN
{
    public class SPSINSiteSettingsIconFix : Control
    {
        protected override void OnInit(EventArgs e)
        {

            try
            {
                if (HttpContext.Current.Request.Url.LocalPath.Contains("settings.aspx"))
                {
                    Control c = FindSPSINImage(Page.Master);        
                    if (c != null)
                    {
                        if (c is FeatureLinkSections)
                        {

                            ((FeatureLinkSections)c).AddGroup += new EventHandler<AddGroupEventArgs>(s_AddGroup);

                        }
                    }
                }
            }
            catch
            {
                // Can't find it, OK to ignore
            }
        }

        void SPSINSiteSettingsIconFix_AddLink(object sender, AddLinkEventArgs e)
        {
            HyperLink link = e.Link;
            string s = link.Text;
        }

        private Control FindSPSINImage(Control control)
        {
            Control foundControl = null;

            if (control.Controls!= null)
            {
                foreach (Control c in control.Controls)
                {
                    if (foundControl == null)
                    {
                        try
                        {
                            if (ThisIsWhatWeAreLookingFor(c))
                            {
                                return c;
                            }
                            else
                            {
                                foundControl = FindSPSINImage(c);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            else
            {
            }
            return foundControl;
        }

        private bool ThisIsWhatWeAreLookingFor(Control c)
        {
            string id = c.ClientID;
            if (c is FeatureLinkSections)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void s_AddGroup(object sender, AddGroupEventArgs e)
        {
            LinkSection ls = e.LinkSection;
            if (ls.Title == "SP SIN")
            {
                ls.Template_OtherControls = new FixTemplate();
            }

        }
    }

    public class FixTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            container.Controls.Add(new LiteralControl(@"
<div id=""SPSIN_FixIcon""></div>
<script>
try {
    var element = document.getElementById('SPSIN_FixIcon');
    var sibling = getSiblings(element.parentNode); 

    var imageNode = sibling[0].childNodes[0];
    imageNode.src = ""/_layouts/SPSIN/IMAGES/SPSINIcon48x48.png"";
}
catch (error) {
// Do nothing, we're probably in SP2007
}
function getChildren(n, skipMe){     
    var r = [];     
    var elem = null;     
    for ( ; n; n = n.nextSibling )         
        if ( n.nodeType == 1 && n != skipMe)
        r.push( n );             
        return r; 
    };  

function getSiblings(n) {     
    return getChildren(n.parentNode.firstChild, n); 
} 

</script>

"));
        }
    }
}
