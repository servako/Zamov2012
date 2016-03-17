<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<NewsPresentation>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterCss("~/Content/News.css")%>
    
    
    <script type="text/javascript">
        $(function() {
            $(".detailsLink").fancybox({ frameWidth: 700, hideOnContentClick: false });
        })
        
    </script>
    
    <script type="text/javascript">
            $(function() {
            $(".detailsNewsInMenuLink").fancybox({ frameWidth: 700, frameHeight: 500, hideOnContentClick: false });
            })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <center>
        <strong><%= Html.ResourceString("News").ToUpper() %></strong>
    </center>
    
    <div class="newsContainer">
<%
    foreach (NewsPresentation item in Model)
    {%>
        <div class="item">
            <div class="title">
                <span class="newsDate"><%= item.Date.ToString("dd.MM.yyyy") %></span>&nbsp;<%= item.Title %>
            </div>
            <%= HttpUtility.HtmlDecode(item.ShortText) %>
            <div class="itemFooter">
                <%= HttpUtility.HtmlDecode(Html.ResourceActionLink("Details", "Details", new { id = item.Id }, new { @class = "detailsLink" }))%>
            </div>
        </div>        
  <%}
%>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
<% IEnumerable<NewsPresentation> news = (IEnumerable<NewsPresentation>)ViewData["news"]; %>
    <% Html.RenderAction<PagePartsController>(ac => ac.News(Html.ResourceString("News"), news)); %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="dealerLogo" runat="server">
</asp:Content>
