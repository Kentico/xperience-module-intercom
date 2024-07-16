<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Files_System_MediaProtection"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Administration - System - Files - Media protection" 
    CodeBehind="System_MediaProtection.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/SmartTip.ascx" TagPrefix="cms" TagName="SmartTip" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:SmartTip runat="server" ID="tipMediaProtection" Visible="true" />
    <div class="cms-bootstrap">
        <div class="content-form-element url-input">
            <cms:CMSTextBox runat="server" ID="txtUrl" />
        </div>
        <div class="content-form-element">
            <cms:CMSButton runat="server" ID="btnProtectUrl" ButtonStyle="Primary" EnableViewState="false" />
        </div>
        <div class="content-form-element url-label">
            <asp:Label ID="lblProtectedUrl" runat="server" />
        </div>
        <div class="content-form-element">
            <asp:Label ID="lblProtectedUrlResult" Text="" runat="server" />
        </div>
    </div>
</asp:Content>