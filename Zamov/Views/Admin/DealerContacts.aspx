<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.DealerContacts>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        var dealer = (Dealer)ViewData["dealer"];
        string dealerName = dealer.GetName(SystemSettings.CurrentLanguage);
        string logoUrl = VirtualPathUtility.ToAbsolute("~/Comtent/img/noLogo");
        int dealerId = dealer.Id;
        if (dealer.LogoImage.Length > 0)
            logoUrl = Url.Action("ShowLogo", "Image", new { id = dealer.Id });
    %>
    <h2>
        Контакты продавца
        <%= dealerName %>
        <br />
        <%= Html.Image(logoUrl) %><br />
    </h2>
    <% using (Html.BeginForm(new { enctype = "multipart/form-data" }))
       { %>
    <%= Html.Hidden("dealerId", dealerId)%>
    <table class="adminTable" style="border: 1px dotted #ccc">
        <tr>
            <th style="display: none">
                Id
            </th>
            <th>
                Тип
            </th>
            <th>
                Значение
            </th>
            <th>
                Активный
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td style="display: none">
                <%= Html.TextBox("id_" + item.Id, item.Id)%>
            </td>
            <td>
                <%= Html.DropDownList("type_" + item.Id, ((ContactType)item.ContactType).ToSelectList())%>
            </td>
            <td>
                <%=Html.TextBox("contact_" + item.Id, String.Format("{0}", item.Value))%>
            </td>
            <td align="center">
                <%= Html.CheckBox("enabled_" + item.Id, item.Enabled)%>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteDealerContact", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>
        </tr>
        <% } %>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>
    <br />
    <% using (Html.BeginForm("InsertDealerContact", "Admin"))
       { %>
    <%= Html.Hidden("dealerId", dealerId)%>
    <table class="adminTable">
        <tr>
            <th>
                Тип
            </th>
            <th>
                Значение
            </th>
            <th>
                Активный
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.DropDownList("type", (ContactType.PhoneMobile.ToSelectList()))%>
            </td>
            <td>
                <%= Html.TextBox("contact")%>
            </td>
            <td align="center">
                <%= Html.CheckBox("enabled", true)%>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    <%} %>
    <div>
        <%=Html.ActionLink("Назад", "Dealers") %>
    </div>
</asp:Content>
