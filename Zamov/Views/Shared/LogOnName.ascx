<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>

<div class="logOnProperties">
    <div class="logOnName">
        <%= Html.ResourceString("Welcome") %>,<br />
        <%= Profile.FirstName %> <%= Profile.LastName %>!
    </div>
    <%= Html.ResourceActionLink("ChangePassword", "ChangePassword","Account") %>
    &nbsp;
    <%= Html.ResourceActionLink("LogOff", "LogOff", "Account") %>
</div>

