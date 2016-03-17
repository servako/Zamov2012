<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%= ViewData["content"] %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="includes">
<%--    <%= Html.RegisterCss("/Content/noLeftMenu.css")%>--%>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <script type="text/javascript">
        $(function() {
        $(".detailsNewsInMenuLink").fancybox({ frameWidth: 700, hideOnContentClick: false });
        })
    </script>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="leftMenu" runat="server">
<% IEnumerable<NewsPresentation> news = (IEnumerable<NewsPresentation>)ViewData["news"]; %>
    <% Html.RenderAction<PagePartsController>(ac => ac.News(Html.ResourceString("News"), news)); %>
</asp:Content>