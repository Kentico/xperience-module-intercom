using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json.Linq;

public partial class CMSModules_Intercom_Pages_AccessTokenDialog : CMSModalPage
{
    private const string INTERCOM_GET_AUTH_CODE_URL_FORMAT = "https://app.intercom.com/oauth?client_id={0}&state={1}";
    private const string INTERCOM_TOKEN_URL = "https://api.intercom.io/auth/eagle/token";
    private const string SESSION_KEY = "INTERCOM_TOKENREQUEST";
    private const string AUTHORIZATION_CODE_QUERY_PARAMETER = "code";
    private const string STATE_QUERY_PARAMETER = "state";


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckPermissions("Kentico.Xperience.Intercom.Admin", PermissionsEnum.Modify.ToString());
        if (SiteContext.CurrentSite == null)
        {
            RedirectToResourceNotAvailableOnSite("Kentico.Xperience.Intercom.Admin");
        }

        PageTitle.TitleText = "Intercom access token";
        PageTitle.ShowFullScreenButton = false;
        PageTitle.ShowCloseButton = false;

        if (!QueryHelper.Contains(AUTHORIZATION_CODE_QUERY_PARAMETER))
        {
            BeginAuthorization();
            return;
        }
        else
        {
            var authorizationCode = QueryHelper.GetString(AUTHORIZATION_CODE_QUERY_PARAMETER, String.Empty);
            
            if (String.IsNullOrEmpty(authorizationCode))
            {
                ShowError("Authorization code is missing.");
            }

            var state = QueryHelper.GetString(STATE_QUERY_PARAMETER, String.Empty);
            var expectedState = WindowHelper.GetItem(SESSION_KEY)?.ToString();

            if (String.IsNullOrEmpty(state) || String.IsNullOrEmpty(expectedState) || !String.Equals(state, expectedState))
            {
                ShowError("State parameter is incorrect.");
            }

            CompleteAuthorization(authorizationCode);
        }
    }


    private void CloseDialog()
    {
        var closeDialogScript = "CloseAndRefresh();";
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseDialogScript" + ClientID, closeDialogScript, true);
    }


    /// <summary>
    /// Begins authorization process and redirects client to the Intercom authorization page.
    /// </summary>
    private void BeginAuthorization()
    {
        var sessionKeyValue = Guid.NewGuid().ToString();

        // Store key in session
        WindowHelper.Add(SESSION_KEY, sessionKeyValue);

        var redirectUrl = new Uri(URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL));

        try
        {
            var clientID = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSite.SiteName}.CMSIntercomClientID");

            var authCodeUrl = String.Format(INTERCOM_GET_AUTH_CODE_URL_FORMAT, clientID, sessionKeyValue);

            URLHelper.Redirect(URLHelper.AppendQuery(authCodeUrl, $"redirect_uri={HttpUtility.UrlEncode(redirectUrl.GetLeftPart(UriPartial.Query))}"));
        }
        catch (ThreadAbortException)
        {
            // Reset exception - this exception is expected because client is redirected to external page.
            Thread.ResetAbort();
        }
        catch (Exception exception)
        {
            LogException(exception);
            RemoveSessionEntry();
        }
    }


    /// <summary>
    /// Trades the <paramref name="authorizationCode"/> for Intercom OAuth access token and stores the token in the 'CMSIntercomOAuthToken' setting.
    /// </summary>
    private void CompleteAuthorization(string authorizationCode)
    {
        try
        {
            using (var client = new WebClient())
            {
                var requestParameters = new NameValueCollection()
                {
                    {  "client_id",  SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSite.SiteName}.CMSIntercomClientID") },
                    {  "client_secret",  SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSite.SiteName}.CMSIntercomClientSecret") },
                    {  "code",  authorizationCode }
                };

                var response = client.UploadValues(INTERCOM_TOKEN_URL, requestParameters);

                var responseData = JObject.Parse(Encoding.UTF8.GetString(response, 0, response.Length));

                var oauthToken = responseData.Value<string>("token");

                SettingsKeyInfoProvider.SetValue($"CMSIntercomOAuthToken", SiteContext.CurrentSiteID, oauthToken);

                CloseDialog();
            }
        }
        catch (Exception exception)
        {
            ShowError("Unexpected exception occurred. See event log for more details.");
            LogException(exception);
        }
        finally
        {
            RemoveSessionEntry();
        }
    }


    private static void LogException(Exception exception)
    {
        Service.Resolve<IEventLogService>().LogException("Intercom", "ACCESSTOKEN", exception);
    }


    private static void RemoveSessionEntry()
    {
        SessionHelper.Remove(SESSION_KEY);
    }
}