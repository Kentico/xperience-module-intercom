<%@ Page Title="Intercom Access Token" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" AutoEventWireup="false" Inherits="CMSModules_Intercom_Pages_AccessTokenDialog"  Codebehind="Intercom_AccessTokenDialog.aspx.cs" Theme="Default" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
        <script type="text/javascript">
            function CloseAndRefresh() {
                if (wopener) {
                    wopener.location.replace(wopener.location);
                }
                CloseDialog();
            }
        </script>
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" />
</asp:Content>
