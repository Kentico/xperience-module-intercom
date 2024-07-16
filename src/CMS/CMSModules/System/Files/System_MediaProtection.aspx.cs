using System;

using CMS.Core;
using CMS.Core.Internal;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_System_Files_System_MediaProtection : GlobalAdminPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        btnProtectUrl.Click += btnProtectUrl_Click;
        btnProtectUrl.Text = GetString("administration-system.files.mediaprotection.submit");
        lblProtectedUrl.Text = string.Empty;

        tipMediaProtection.ExpandedHeader = GetString("administration-system.files.mediaprotection.smarttip.title");
        tipMediaProtection.Content = GetString("administration-system.files.mediaprotection.smarttip");
    }


    protected void btnProtectUrl_Click(object sender, EventArgs e)
    {
        var protectionService = Service.Resolve<IMediaProtectionService>();

        var protectedUrl = protectionService.GetProtectedUrl(txtUrl.Text);
        URLHelper.GetQueryValue(protectedUrl, MediaProtectionConstants.MEDIA_PROTECTION_HASH_QUERY_KEY, out var hashAdded);

        lblProtectedUrlResult.Text = hashAdded ? HTMLHelper.HTMLEncode(protectedUrl) : GetString("administration-system.files.mediaprotection.unknownformat");

        if (hashAdded)
        {
            lblProtectedUrl.Text = GetString("administration-system.files.mediaprotection.protectedurl");
        }
        else
        {
            lblProtectedUrl.Text = string.Empty;
        }
    }
}