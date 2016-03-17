<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    if (Request.IsAuthenticated)
        Html.RenderPartial("LogOnName");
    else
        Html.RenderPartial("MasterLogOn");
%>
