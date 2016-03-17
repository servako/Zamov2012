<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <%= Html.ResourceString("EnterEmail") %>
    </p>
    <% using(Html.BeginForm()){ %>
    <%= Html.ValidationSummary() %>
    <%= Html.TextBox("email") %>
    <%= Html.ValidationMessage("email", "*", new { @class = "validationError" })%>
    <br />
    <table>
        <tr valign="middle">
            <td>
                <%= Html.TextBox("captcha") %>
                <%= Html.ValidationMessage("captchaCheck", "*", new { @class = "validationError" })%>
            </td>
            <td>
                <%= Html.CaptchaImage(50, 150)%><br />
            </td>
        </tr>
    </table>
    <br />
    <input type="submit" value="<%= Html.ResourceString("Send") %>" />
    <%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>
