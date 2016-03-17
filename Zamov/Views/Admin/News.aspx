<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.NewsPresentation>>" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%= Html.ResourceString("News") %>
    </h2>
    <%if (Model.Count() > 0)
      { %>
    <% using (Html.BeginForm())
       { %>
    <table class="adminTable">
        <tr>
            <th><%= Html.ResourceString("Header")%></th>
            <th><%= Html.ResourceString("Date")%></th>
            <th><%= Html.ResourceString("ActiveF")%></th>
            <th></th>
            <th></th>
        </tr>
        <% foreach (NewsPresentation item in Model)
           {%>
        <tr>
            <td><%= item.Title%></td>
            <td><%= item.Date.ToString("dd.MM.yyyy")%></td>
            <td align="center"><%= Html.CheckBox("enabled_" + item.Id, item.Enabled)%></td>
            <td><%= Html.ResourceActionLink("Edit", "AddEditNews", new { id = item.Id })%></td>
            <td><%= Html.ResourceActionLink("Delete", "DeleteNews", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%></td>
        </tr>
        <%} %>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <% } %>
    <br />
    <%} %>
    <%= Html.ResourceActionLink("Add", "AddEditNews")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
