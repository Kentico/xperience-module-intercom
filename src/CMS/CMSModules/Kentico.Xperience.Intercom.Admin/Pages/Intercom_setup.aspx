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
        <h2>Intercom integration</h2>
        <div class="wrapper">
            <p>The Intercom chat service integration allows for the updating and extension of contact data based on information provided by visitors during chatbot conversations on your website.</p> 
            <p>It also records conversation transcripts and logs conversation activities that can be used by other Kentico Xperience marketing features to further enrich the customer experience.</p>
            
        </div>
        <cms:CMSUpdatePanel ID="pnlForm" runat="server" EnableViewState="false" UpdateMode="Always">
            <ContentTemplate>
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
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblClientID" runat="server" EnableViewState="false" ResourceString="Intercom Client ID"
                                                    DisplayColon="true" AssociatedControlID="txtClientID" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtClientID" EnableViewState="false" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblClientSecret" runat="server" EnableViewState="false" ResourceString="Intercom Client Secret"
                                                    DisplayColon="true" AssociatedControlID="txtClientSecret" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtClientSecret" EnableViewState="false" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="lblIdentityVerificationSecret" runat="server" EnableViewState="false" ResourceString="Intercom Identity Verification Secret"
                                                    DisplayColon="true" AssociatedControlID="txtIdentityVerificationSecret" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSTextBox runat="server" ID="txtIdentityVerificationSecret" EnableViewState="false" />
                                        <div class="explanation-text-settings"><a target="_blank" href="https://github.com/Kentico/xperience-module-intercom/blob/master/README.md">Where can I find the Intercom IDs and Secrets?</a></div>
                                    </div>
                                </div>
                                    <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" ID="LocalizedLabel1" runat="server" EnableViewState="false" ResourceString="Send contact attributes to Intercom:"
                                                    DisplayColon="true" AssociatedControlID="radAlways" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <div class="control-group">
                                            <cms:CMSRadioButton ID="radAlways" runat="server" Text="Always" GroupName="RadioPrivacy" AutoPostBack="true" />
                                        </div>
                                        <div class="control-group">
                                            <cms:CMSRadioButton ID="radNever" runat="server" Text="Never" GroupName="RadioPrivacy" AutoPostBack="true" />
                                        </div>
                                        <div class="control-group-inline-forced">
                                                <cms:CMSRadioButton  ID="radConsent" runat="server" Text="Only if contact has agreed to a specific consent:" GroupName="RadioPrivacy" AutoPostBack="true" />
                                                <asp:PlaceHolder ID="plcConsentSelector" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group form-group-submit">
                                        <cms:CMSButton runat="server" ButtonStyle="Primary" ID="btnSetupSave" OnClick="ButtonSetupSave_Click" EnableViewState="false" Text="Save" />
                                </div>
                            </div>
                </asp:Panel>
                <asp:Panel ID="pnlAccessToken" runat="server">
                    <h5>Intercom data access</h5>
                    <cms:MessagesPlaceHolder ID="msgAccessToken" runat="server" IsLiveSite="false" />
                    <div class="form-horizontal">
                        <div class="form-group">
                            <p>Xperience requires an access token to obtain your Intercom workspace's data via the API.</p>
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
                    <h5>Webhook request security</h5>
                    <div class="form-horizontal">
                        <div class="form-group">
                            <p>Contact data updates and logging of activities in Xperience is done via webhook requests sent using the Intercom Series feature.</p>
                            <p>Security is provided by an API key that must be included in every webhook request, For more information, see <a target="_blank" href="https://github.com/Kentico/xperience-module-intercom/blob/master/README.md">Using Series webhooks</a></p>
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
                                <div class="explanation-text-settings">Provide the key value within the <strong>XperienceApiKey</strong> header in every webhook request.</div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
