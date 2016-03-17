<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%
    var banner = (string)ViewData["banner"];
    var bannerEnabled = false;
    if (ViewData["bannerEnabled"] != null)
        bannerEnabled = (bool)ViewData["bannerEnabled"];
    if (bannerEnabled)
    {
%>
<div id="bannerTopOuter">
    <div id="bannerTopInner">
        <%=Html.RegisterFlashScript(banner, "b_1", BannerPosition.Top)%>
    </div>
</div>
<%} %>
