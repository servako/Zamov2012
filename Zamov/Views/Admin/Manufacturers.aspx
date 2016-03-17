<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Manufacturer>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.ResourceString("Manufacturers") %></h2>

    <table class="adminTable" style="border:1px dotted #ccc" >
        <tr>
            <th style="display:none">
                Id
            </th>
            <th>
                Название
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
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteManufacturer", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
                
            </td>
        </tr>
    
    <% } %>

    </table>

    


<% using (Html.BeginForm("InsertManufacturer", "Admin"))
   { %>
    <table class="adminTable">
        <tr>
            <th>
                Название
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.TextBox("manufacturerName") %>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    <%} %>


</asp:Content>


