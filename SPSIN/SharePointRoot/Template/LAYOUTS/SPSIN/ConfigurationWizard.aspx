<%@ Page Language="C#" MasterPageFile="~/_layouts/application.master" Inherits="SPSIN.ApplicationPages.ConfigurationWizard, SPSIN, Version=1.0.0.0, Culture=neutral, PublicKeyToken=29a1bc68ea6f4b3b" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <h1>
            SP SIN Configuration Packages</h1>
        Please select a package to add below. Note that adding a package will add that configuration
        to the site even if the configuration has been added before.<br />
        <asp:Label Font-Bold="true" ForeColor="Red" runat="server" ID="SPSIN_Message" />
    </div>
    <asp:Panel runat="server" ID="SPSIN_PackagesPanel">
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderLeftActions" runat="server">
            <p>
                <asp:HyperLink ID="HyperLink1" NavigateUrl="http://spsin.com/" Target="_blank" runat="server">SP SIN on the web</asp:HyperLink><br />
            </p>
            <p>
                <asp:HyperLink NavigateUrl="http://spsin.com/" Target="_blank" runat="server" ID="SPSIN_UpdateLink"
                    Visible="false">Upgrade Available!</asp:HyperLink></p>
    <SharePoint:DelegateControl ControlId="SPSINFeatureMenu" runat="server" AllowMultipleControls="true">
    </SharePoint:DelegateControl>
</asp:Content>
