<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<List<UserPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Models" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% 
        string userType = "customers";
        if (!string.IsNullOrEmpty((string)ViewData["userType"]))
            userType = (string)ViewData["userType"];
    %>

    <h2><%= Html.ResourceString("Users") %></h2>
    <br />
    
    <% Html.RenderPartial("UserTypeSelector"); %>
    
    <table class="adminTable">
        <tr>
            <th><%= Html.ResourceSortHeader("Username", "/Admin/Users/", "Email", "userType=" + ViewData["userType"]) %></th>
            <th><%= Html.ResourceSortHeader("FirstName", "/Admin/Users/", "FirstName", "userType=" + ViewData["userType"])%></th>
            <th><%= Html.ResourceSortHeader("LastName", "/Admin/Users/", "LastName", "userType=" + ViewData["userType"])%></th>
            <%if(userType == "dealers"){ %>
            <th><%= Html.ResourceSortHeader("Dealer", "/Admin/Users/", "DealerName", "userType=" + ViewData["userType"])%></th>
            <%} %>
            <th></th>
            <th></th>
        </tr>
        <tr>
            <%
                foreach (UserPresentation user in Model)
                {
                    Html.RenderAction<AdminController>(ac=>ac.UserDetails(user));
                }    
            %>
        </tr>
    </table>
        
</asp:Content>