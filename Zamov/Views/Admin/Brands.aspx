<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.Brands>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.ResourceString("Brands")%></h2>
<% using (Html.BeginForm("Brands", "Admin"))
   { %>
    <table class="adminTable" style="border:1px dotted #ccc" >
        <tr>
            <th>
                Id
            </th>
            <th>
                Название
            </th>
            <th></th>
        </tr>

    <% for (int i = 0; i < Model.Count(); i++)
                {    %>
        <tr>
            <td >
                <%=Html.Hidden(string.Format("brands[{0}].brandID", i), Model[i].brandID)%> 
                <%=Html.Label(Model[i].brandID.ToString())%>
            </td>
            <td>
                <%= Html.TextBox(string.Format("brands[{0}].brandName", i), Model[i].brandName)%>
            </td>
        </tr>
    
    <% } %>

    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
<%} %>

<% using (Html.BeginForm("InsertBrand", "Admin"))
   { %>
    <table class="adminTable">
        <tr>
            <th>
                Название
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.TextBox("brandName") %>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    <%} %>


</asp:Content>


