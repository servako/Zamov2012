<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterCss("~/Content/register.css") %>
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.ResourceString("LogOn") %></h2>
     <% using (Html.BeginForm()) { %>
    
    <%= Html.ResourceString("LoginHeader") %>
    <div>
        <%= Html.ValidationSummary(Html.ResourceString("LoginUnsucsessful"))%>
        <fieldset>
        <legend>
            <%= Html.ResourceString("AccountInfo") %>
        </legend>
         <table class="registrationTable">
         <tr>
            <td>
                <label style="width: 300px" for="username">
                    <%= Html.ResourceString("Username")%>:</label>
                <%= Html.ValidationMessage("username", "*", new { @class = "validationError" })%>
            </td>
            <td>
                <%= Html.TextBox("username")%>
            </td>
        </tr>
        <tr>
            <td>
                <label for="password">
                    <%= Html.ResourceString("Password") %>:</label>
                <%= Html.ValidationMessage("password", "*", new { @class = "validationError" })%>
            </td>
            <td>
                <%= Html.Password("password") %>
            </td>
        </tr>
         </table>
        </fieldset>
        
    </div>
<br />

   <%= Html.CheckBox("rememberMe") %> <label class="inline" for="rememberMe"><%= Html.ResourceString("RememberMe") %></label><br />
   <input type="submit" value="<%= Html.ResourceString("LogOn") %>" />
    <% } %>
</asp:Content>
