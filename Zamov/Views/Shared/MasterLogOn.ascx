<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<div style="padding:10px;">
    <center>
        <% string url = VirtualPathUtility.ToAbsolute("~/Account/Register"); %>
        <a class="registerLink" href=<%= url %>>
            <%= Html.ResourceString("Register").ToUpper() %>
        </a>
    </center>
    <% using (Html.BeginForm("LogOn", "Account", FormMethod.Post)){ %>
    <%= Html.Hidden("returnUrl", Request.Url.AbsoluteUri)%>
    <table cellspacing="5">
        <tr>
            <td>
                Email:<%--<%= Html.ResourceString("Login")%>:--%>
            </td>
            <td>
                <input type="text" id="userName" name="userName" class="logonInput" name="" />
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.ResourceString("Password")%>:  
            </td>
            <td>
                <input type="password" id="password" name="password" class="logonInput" name="" />
            </td>
        </tr>
        <tr>
            <td>&nbsp</td>
            <td>
                <%= Html.CheckBox("rememberMe") + "&nbsp;" + Html.ResourceString("RememberMe")%>
            </td>
        </tr>
        <tr>
            <td></td>
            <td valign="middle" id="logonCell">
                <input type="submit" value="<%= Html.ResourceString("LogOn") %>" /> <%= Html.ResourceActionLink("ForgotPassword", "ResetPassword", "Account") %>
            </td>
        </tr>
    </table>
    <%} %>
</div>