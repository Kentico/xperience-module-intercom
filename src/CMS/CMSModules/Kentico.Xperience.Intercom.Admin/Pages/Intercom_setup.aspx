<%@ Page Language="C#" AutoEventWireup="false" Inherits="CMSModules_Intercom_Pages_Setup"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" CodeBehind="Intercom_setup.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <style>
        .intercom .wrapper {
            margin-bottom: 20px;
            display: block
        }

        .intercom .setup {
            margin-bottom: 60px;
            display: block
        }

        .intercom .TokenStatus {
            margin-left: 4px;
            padding: 0 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="intercom">
        <h2>Intercom chat service</h2>
        <div class="wrapper">
            <p>
                The integration allows updating contact information and logging custom activities.
            </p>
        </div>

        <asp:Panel ID="pnlSetup" runat="server" DefaultButton="btnSetupSave" CssClass="setup">
            <h4>Setup</h4>
            <cms:MessagesPlaceHolder ID="plcSetupMessage" runat="server" IsLiveSite="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblEnableIntercom" runat="server" EnableViewState="false" ResourceString="Enable Intercom"
                                DisplayColon="true" AssociatedControlID="chkEnableIntercom" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkEnableIntercom" runat="server" EnableViewState="false" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblAppID" runat="server" EnableViewState="false" ResourceString="Intercom App ID"
                                 DisplayColon="true" AssociatedControlID="txtAppID" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtAppID" EnableViewState="false" />
                        <div class="explanation-text-settings"><a target="_blank" href="https://www.intercom.com/help/en/articles/3539-where-can-i-find-my-workspace-id-app-id">Where can I find App ID?</a></div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblClientID" runat="server" EnableViewState="false" ResourceString="Intercom Client ID"
                                 DisplayColon="true" AssociatedControlID="txtClientID" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtClientID" EnableViewState="false" />
                        <div class="explanation-text-settings"><a target="_blank" href="https://github.com/Kentico/xperience-module-intercom/blob/master/README.md">Where can I find Client ID?</a></div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblClientSecret" runat="server" EnableViewState="false" ResourceString="Intercom Client Secret"
                                 DisplayColon="true" AssociatedControlID="txtClientSecret" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtClientSecret" EnableViewState="false" />
                        <div class="explanation-text-settings"><a target="_blank" href="https://github.com/Kentico/xperience-module-intercom/blob/master/README.md">Where can I find Client Secret?</a></div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblIdentityVerificationSecret" runat="server" EnableViewState="false" ResourceString="Intercom Identity Verification Secret"
                                 DisplayColon="true" AssociatedControlID="txtIdentityVerificationSecret" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox runat="server" ID="txtIdentityVerificationSecret" EnableViewState="false" />
                        <div class="explanation-text-settings"><a target="_blank" href="https://github.com/Kentico/xperience-module-intercom/blob/master/README.md">Where can I find Identity Verification Secret?</a></div>
                    </div>
                </div>
                <div class="form-group form-group-submit">
                        <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnSetupSave" OnClick="ButtonSetupSave_Click" EnableViewState="false" Text="Save" />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlAccessToken" runat="server">
            <h5>Accessing Intercom data via API</h5>
            <cms:MessagesPlaceHolder ID="msgAccessToken" runat="server" IsLiveSite="false" />
            <div class="form-horizontal">
                <div class="form-group">
                    <p>Access tokens are used to access your workspace's data via API.</p>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblGetToken" runat="server" EnableViewState="false" ResourceString="API Access"
                            DisplayColon="true" />
                    </div>
                    <div class="control-group-inline">
                         <asp:Literal ID="ltlTokenStatus" runat="server" EnableViewState="false" />
                        <cms:LocalizedButton runat="server" ID="btnGetToken" OnClick="ButtonGetToken_Click" Text="Get token" CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlAPIKey" runat="server">
            <h5>Securing webhook requests</h5>
            <div class="form-horizontal">
                <div class="form-group">
                    <p>API key must be included in every webhook request within the Series feature. For more information see <a target="_blank" href="#">How to use Series webhooks?</a></p>
                    <p><span style="color:red;font-style:italic;">This is a temporary solution until Intercom fixes <span class="code">X-Hub-Signature</span> header in Series webhook.</span></p>
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblWebhookAPIKey" runat="server" EnableViewState="false" ResourceString="Your API Key"
                            DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="control-group-inline">
                             <cms:CMSTextBox runat="server" ID="txtWebhookAPIKey" Enabled="true" EnableViewState="false" />
                             <cms:LocalizedCopyToClipboardButton ID="btnCopy" runat="server" CopySourceControlID="txtWebhookAPIKey" ResourceString="general.copy" EnableViewState="false" ButtonStyle="Default" />
                             <cms:LocalizedButton runat="server" ID="btnGenerateWebhookAPIKey" OnClick="ButtonGenerateWebhookAPIKey_Click" Text="Generate new" ButtonStyle="Default" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
