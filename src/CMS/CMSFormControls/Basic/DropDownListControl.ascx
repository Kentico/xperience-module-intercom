<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="DropDownListControl.ascx.cs"
    Inherits="CMSFormControls_Basic_DropDownListControl" %>
    <cms:CMSDropDownList ID="dropDownList" runat="server" CssClass="DropDownField" />
<asp:Panel ID="autoComplete" runat="server" CssClass="autocomplete">
    <cms:CMSTextBox ID="txtCombo" runat="server" Visible="false" CssClass="autocomplete-textbox" />
    <i runat="server" id="btnAutocomplete" class="autocomplete-icon icon-ellipsis" Visible="false" />
</asp:Panel>