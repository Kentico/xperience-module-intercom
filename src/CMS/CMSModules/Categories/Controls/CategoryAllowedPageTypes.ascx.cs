using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Categories_Controls_CategoryAllowedPageTypes : CMSAdminEditControl
{
    #region "Variables"
    protected string currentValues = string.Empty;

    private int mUserID;
    private int mSiteId = -1;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets CssClass of the panel wrapping edit form.
    /// </summary>
    public string PanelCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Component name.
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return base.ComponentName;
        }
        set
        {
            headerActions.ComponentName = value;
            base.ComponentName = value;
        }
    }


    /// <summary>
    /// Header actions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            if (Category != null)
            {
                return Category.CategoryID;
            }
            return 0;
        }
        set
        {
            pnlContext.UIContext.EditedObject = CategoryInfo.Provider.Get(value);
        }
    }


    /// <summary>
    /// Edited category object.
    /// </summary>
    public CategoryInfo Category
    {
        get
        {
            return (CategoryInfo)pnlContext.UIContext.EditedObject;
        }
        set
        {
            pnlContext.UIContext.EditedObject = value;
        }
    }


    /// <summary>
    /// ID of the site to create categories for.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId < 0)
            {
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// ID of the user to create/edit category for.
    /// </summary>
    public int UserID
    {
        get
        {
            if ((Category != null) && (Category.CategoryIsPersonal))
            {
                // Return current category's user ID
                return Category.CategoryUserID;
            }

            return mUserID;
        }
        set
        {
            mUserID = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            EnsureChildControls();
            base.StopProcessing = value;
        }
    }


    /// <summary>
    /// Editing form control.
    /// </summary>
    public UIForm EditingForm
    {
        get
        {
            return editElem;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editElem.StopProcessing = true;
        editElem.OnAfterDataLoad += EditElem_OnAfterDataLoad;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing && Visible)
        {
            editElem.OnBeforeValidate += EditElem_OnBeforeValidate;
            editElem.OnAfterSave += EditElem_OnAfterSave;

            LoadData();
        }
    }
    

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(PanelCssClass))
        {
            pnlEdit.CssClass = PanelCssClass;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the category data in the control.
    /// </summary>
    public override void ReloadData()
    {
        if (!StopProcessing)
        {
            base.ReloadData();
            headerActions.ReloadData();

            LoadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads the data.
    /// </summary>
    private void LoadData()
    {
        // Get information on current category
        if (Category != null)
        {
            editElem.EditedObject = Category;
            editElem.ReloadData();
        }
    }


    /// <summary>
    /// OnBeforeValidate event handler.
    /// </summary>
    protected void EditElem_OnBeforeValidate(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!RaiseOnCheckPermissions("Modify", this))
        {
            // Check User's Modify permission
            if ((UserID > 0) && (UserID != MembershipContext.AuthenticatedUser.UserID) && (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify")))
            {
                editElem.ShowError(String.Format(GetString("general.permissionresource"), "Modify", "CMS.Users"));
                editElem.StopProcessing = true;
            }
        }

        if (UserID == 0)
        {
            // Need Modify or GlobalModify permission to edit non-personal categories
            string permission = Category.CategorySiteID > 0 ? "Modify" : "GlobalModify";
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", permission))
            {
                ShowError(String.Format(GetString("general.permissionresource"), permission, "CMS.Categories"));
                editElem.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// OnAfterDataLoad event handler.
    /// </summary>
    protected void EditElem_OnAfterDataLoad(object sender, EventArgs e)
    {
        if ((Category != null) && (Category.CategoryID > 0))
        {
            UniSelector usClasses = (UniSelector)editElem.FieldControls["CategoryClasses"];

            // Assign selected category site ID for filtering
            if (!Category.IsGlobal)
            {
                usClasses.WhereCondition = "ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + Category.CategorySiteID + ")";
            }

            // Load current bindings
            DataSet ds = CategoryClassInfoProvider.ProviderObject.Get()
                .Columns(nameof(CategoryClassInfo.ClassID))
                .WhereEquals(nameof(CategoryClassInfo.CategoryID), Category.CategoryID);

            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ClassID"));

            usClasses.Value = currentValues;
            usClasses.Reload(true);
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void EditElem_OnAfterSave(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(editElem.FieldControls["CategoryClasses"].Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                CategoryClassInfoProvider.ProviderObject.Remove(Category.CategoryID, ValidationHelper.GetInteger(item, 0));
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                CategoryClassInfoProvider.ProviderObject.Add(Category.CategoryID, ValidationHelper.GetInteger(item, 0));
            }
        }

        editElem.ReloadData();
    }

    #endregion
}