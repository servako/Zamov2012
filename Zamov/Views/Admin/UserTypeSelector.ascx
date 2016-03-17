<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<% 
    string userType = "customers";
    if (!string.IsNullOrEmpty((string)ViewData["userType"]))
        userType = (string)ViewData["userType"];
%>

<div>
    <%if(userType == "customers"){ %>
        <strong>
            <%= Html.ResourceString("Customers") %>
        </strong>
        &nbsp;|&nbsp;
        <a href="/Admin/Users?userType=dealers">
            <%= Html.ResourceString("Dealers") %>
        </a>
    <%} %>
    <%if(userType == "dealers"){ %>
        <a href="/Admin/Users?userType=customers">
            <%= Html.ResourceString("Customers") %>
        </a>
        &nbsp;|&nbsp;
         <strong>
            <%= Html.ResourceString("Dealers") %>
        </strong>
        
    <%} %>
</div>
