<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Currencies>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.ResourceString("Currencies") %></h2>

    <table class="adminTable" style="border:1px dotted #ccc" >
        <tr>
            <th style="display:none">
                Id
            </th>
            <th>
                Название
            </th>
            <th>
                Значек
            </th>
            <th>
                Сокращение
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td style="display:none">
                <%= Html.Hidden("itemId_" + item.Id, item.Id)%>
            </td>
            <td>
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <%= Html.Encode(item.Sign) %>
            </td>
            <td>
                <%= Html.Encode(item.ShortName) %>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteCurrency", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>
        </tr>
    
    <% } %>

    </table>

   <% using (Html.BeginForm("InsertCurrency", "Admin"))
   { %>
    <table class="adminTable">
        <tr>
            <th>
                Название
            </th>
            <th>
                Значек
            </th>
            <th>
                Сокращение
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.TextBox("currencyName") %>
            </td>
            <td>
                <%= Html.TextBox("currencySign") %>
            </td>    
            <td>
                <%= Html.TextBox("currencyShortName") %>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    <%} %>

</asp:Content>
