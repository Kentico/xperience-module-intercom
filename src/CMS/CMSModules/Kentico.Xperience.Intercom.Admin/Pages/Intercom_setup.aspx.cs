using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using Kentico.Xperience.Intercom.Admin;

[UIElement("Kentico.Xperience.Intercom.Admin", "Kentico.Xperience.Intercom.Admin")]
public partial class CMSModules_Intercom_Pages_Setup : CMSPage
{
    private readonly ISettingsService settingsService;
    private readonly ISiteService siteService;
    private UniSelector consentSelector;

    public CMSModules_Intercom_Pages_Setup()
    {
        settingsService = Service.Resolve<ISettingsService>();
        siteService = Service.Resolve<ISiteService>();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetupPrivacyRadioButtons();
        SetupConsentSelector();

        chkEnableIntercom.Checked = SettingsKeyInfoProvider.GetBoolValue($"{SiteContext.CurrentSiteName}.CMSIntercomEnabled");
        txtAppID.Text = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomAppID");
        txtClientID.Text = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomClientID");
        txtClientSecret.Text = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomClientSecret");
        txtIdentityVerificationSecret.Text = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomIdentityVerificationSecret");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckPermissions("Kentico.Xperience.Intercom.Admin", PermissionsEnum.Read.ToString());
        if (SiteContext.CurrentSite == null)
        {
            RedirectToResourceNotAvailableOnSite("Kentico.Xperience.Intercom.Admin");
        }

        if (!IsOnlineMarketingEnabled())
        {
            btnSetupSave.Enabled = false;
            btnGenerateWebhookAPIKey.Enabled = false;
            btnGetToken.Enabled = false;

            ShowInformation("On-line marketing must be enabled to use the Intercom integration.");
        }
        else if (!IsCurrentUserAllowedToModify())
        {
            btnSetupSave.Enabled = false;
            btnGenerateWebhookAPIKey.Enabled = false;
            btnGetToken.Enabled = false;

            ShowInformation("You are not authorized to setup Intercom integration on this site.");
        }

        if (!RequestContext.IsSSL)
        {
            msgAccessToken.ShowWarning($"Your site might not be accessible over the secured SSL protocol which is required by Intercom authorization.");
        }

        consentSelector.Enabled = radConsent.Checked && radConsent.Enabled;

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        txtWebhookAPIKey.Text = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomAPIKey");

        btnGetToken.Enabled = !String.IsNullOrEmpty(SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomClientID"))
                                && !String.IsNullOrEmpty(SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomClientSecret"));

        ConfigureTokenControl();
    }


    private void SetupPrivacyRadioButtons()
    {
        var shareMode = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomSendContactData");

        switch (shareMode.ToLowerInvariant())
        {
            case "never":
                {
                    radNever.Checked = true;
                    break;
                }
            case "consent":
                {
                    radConsent.Checked = true;
                    break;
                }
            default:
                {
                    radAlways.Checked = true;
                    break;
                }
        }

        if (!AnyConsentExists())
        {
            radConsent.Enabled = false;
        }
    }


    private void SetupConsentSelector()
    {
        var consentName = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomSendContactDataConsent");

        consentSelector = LoadUserControl("~/CMSAdminControls/UI/UniSelector/UniSelector.ascx") as UniSelector;
        consentSelector.Enabled = radConsent.Checked && radConsent.Enabled;
        consentSelector.ObjectType = "cms.consent";
        consentSelector.ID = "consentSelector";
        consentSelector.CheckChanges = true;
        consentSelector.ReturnColumnName = "ConsentName";
        consentSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        consentSelector.IsLiveSite = false;
        consentSelector.AllowEmpty = false;

        if (!String.IsNullOrEmpty(consentName))
        {
            consentSelector.Value = consentName;
        }

        plcConsentSelector.Controls.Clear();
        plcConsentSelector.Controls.Add(consentSelector);
    }


    private static bool AnyConsentExists()
    {
        return ConsentInfo.Provider.Get().Column("ConsentID").TopN(1).HasResults();
    }


    private void ConfigureTokenControl()
    {
        var isTokenConfigured = !String.IsNullOrEmpty(SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.CMSIntercomOAuthToken"));

        var iconTag = isTokenConfigured
                            ? UIHelper.GetAccessibleIconTag("icon-check-circle", additionalClass: "color-green-100")
                            : UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", additionalClass: "color-orange-80");

        var tokenStatus = isTokenConfigured ? "Access token is configured" : "Access token is not configured";

        ltlTokenStatus.Text = $"<div class=\"form-control-text\">{iconTag}<span class=\"TokenStatus\">{tokenStatus}</span></div>";
    }


    protected void ButtonGenerateWebhookAPIKey_Click(object sender, EventArgs e)
    {
        if (IsCurrentUserAllowedToModify())
        {
            SettingsKeyInfoProvider.SetValue("CMSIntercomAPIKey", SiteContext.CurrentSiteName, IntercomSecurityMethods.GenerateAPIKey());

            ShowConfirmation("The new API key was generated.");
        }
    }


    protected void ButtonSetupSave_Click(object sender, EventArgs e)
    {
        if (IsCurrentUserAllowedToModify())
        {
            SettingsKeyInfoProvider.SetValue("CMSIntercomEnabled", SiteContext.CurrentSiteName, chkEnableIntercom.Checked);
            SettingsKeyInfoProvider.SetValue("CMSIntercomAppID", SiteContext.CurrentSiteName, txtAppID.Text);
            SettingsKeyInfoProvider.SetValue("CMSIntercomClientID", SiteContext.CurrentSiteName, txtClientID.Text);
            SettingsKeyInfoProvider.SetValue("CMSIntercomClientSecret", SiteContext.CurrentSiteName, txtClientSecret.Text);
            SettingsKeyInfoProvider.SetValue("CMSIntercomIdentityVerificationSecret", SiteContext.CurrentSiteName, txtIdentityVerificationSecret.Text);

            if (radAlways.Checked)
            {
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactData", SiteContext.CurrentSiteName, "always");
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactDataConsent", SiteContext.CurrentSiteName, null);
            }

            if (radNever.Checked)
            {
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactData", SiteContext.CurrentSiteName, "never");
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactDataConsent", SiteContext.CurrentSiteName, null);
            }

            if (radConsent.Enabled && radConsent.Checked)
            {
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactData", SiteContext.CurrentSiteName, "consent");
                SettingsKeyInfoProvider.SetValue("CMSIntercomSendContactDataConsent", SiteContext.CurrentSiteName, consentSelector.Value);
            }

            ShowConfirmation("Intercom settings saved.");
        }
    }


    protected void ButtonGetToken_Click(object sender, EventArgs e)
    {
        if (IsCurrentUserAllowedToModify())
        {
            var dialogURL = URLHelper.GetAbsoluteUrl("~/CMSModules/Kentico.Xperience.Intercom.Admin/Pages/Intercom_AccessTokenDialog.aspx");
            // Open client dialog script
            var openDialogScript = String.Format($"modalDialog('{dialogURL}', 'IntercomAccessTokenDialog', 600, 600, null, null, null, true);");

            ScriptHelper.RegisterStartupScript(this, GetType(), "IntercomAccessTokenOpenModal" + ClientID, openDialogScript, true);
        }
    }


    private bool IsOnlineMarketingEnabled()
    {
        return settingsService[siteService.CurrentSite?.SiteName + ".CMSEnableOnlineMarketing"].ToBoolean(false);
    }


    private bool IsCurrentUserAllowedToModify()
    {
        return UserInfoProvider.IsAuthorizedPerResource("Kentico.Xperience.Intercom.Admin", "Modify", CurrentSiteName, CurrentUser);
    }
}