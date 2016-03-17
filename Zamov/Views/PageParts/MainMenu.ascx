<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%
    bool authenticated = (bool)ViewData["authenticated"];
    bool dealer = false;
    bool admin = false;
    bool customer = false;
    bool manager = false;
    if (authenticated)
    {
        dealer = (bool)ViewData["dealer"];
        admin = (bool)ViewData["admin"];
        customer = (bool)ViewData["customer"];
        manager = (bool)ViewData["manager"];
    }
    if (authenticated && (manager || admin))
        Response.Write(Html.ResourceActionLink("Reports", "Index", "Reports").ToLower() + "&nbsp;|&nbsp;");
    if (authenticated && admin)
        Response.Write(Html.ResourceActionLink("Administration", "Index", "Admin").ToLower() + "&nbsp;|&nbsp;");
    if (authenticated && admin)
        Response.Write(Html.ActionLink("Seo", "Index", "Seo").ToHtmlString().ToLower() + "&nbsp;|&nbsp;");
    if ((authenticated && dealer) || (authenticated && admin))
        Response.Write(Html.ResourceActionLink("DealerCabinet", "Index", "DealerCabinet").ToLower() + "&nbsp;|&nbsp;");
    if ((authenticated && customer))
        Response.Write(Html.ResourceActionLink("PersonalCabinet", "Index", "UserCabinet").ToLower() + "&nbsp;|&nbsp;");
%>
                    <%= Html.ResourceActionLink("News", "Index", "News").ToLower()%>
                |
                <%= Html.ResourceActionLink("Contacts","Contacts","Home").ToLower()%>
                |
                <%= Html.ResourceActionLink("AboutUs","About","Home").ToLower()%>