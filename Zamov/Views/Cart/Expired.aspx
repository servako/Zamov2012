<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.ResourceString("Attention") %></h2>
    <div style="margin-top:20px;">
        <%= Html.ResourceString("PageExpiredText")%>&nbsp;
        <a href="/">
            <%= Html.ResourceString("ToTheMainPage")%>
        </a>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="dealerLogo" runat="server">
</asp:Content>
