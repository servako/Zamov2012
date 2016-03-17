<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<DealerPresentation>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ContentPlaceHolderID="titleContent" runat="server"><%= Html.GetSeoTitle() %></asp:Content>
<asp:Content ContentPlaceHolderID="seoContent" runat="server"><%= Html.GetSeo() %></asp:Content>
<asp:Content ContentPlaceHolderID="TopBanner" runat="server">
    <%
        var advert = (Advert)ViewData["advert"];

        if (advert != null)
            Html.RenderPartial("TopBanner", advert); 
    %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% int currentCategory = Convert.ToInt32(ViewContext.RouteData.Values["id"]); %>
    <script type="text/javascript">
        var intervalId = 0;
        var intervalCount = 0;
        $(function () {
            applyDropShadows(".dealerLogoLink", "shadow3");
            if ($.browser.msie && $.browser.version !== "8.0") {
                $(".topDealers").width($(mainContent).width() - 30);
                $(window).resize(function () { $(".topDealers").width($(mainContent).width() - 30); });
                $(".dalerOnline").css("margin-left", -40);

            }
            else {
                $(".topDealers").css({ width: "97%", float: "left" });
            }
        })

        function alignDealerOnline() {
            $(".dalerOnline").each(function () {
                var dealerImage = this.nextSibling;
                this.style.marginLeft = $(dealerImage.firstChild).width();
            })
            if (intervalCount > 100) {
                window.clearInterval(intervalId);
            }
            intervalCount++;
        }
    </script>
    <div class="categoriesHeader">
        <div class="categoriesHeaderTitle">
            <%= Html.ResourceString("Dealers") %>
        </div>
    </div>
    <%
        var topDealers = Model.Where(d => d.TopDealer);
        var dealers = Model.Where(d => !d.TopDealer);

        if (topDealers.Count() > 0)
        {%>
    <div class="topDealers">
        <%      foreach (var item in topDealers)
                {%>
        <div class="dealerLogo">
            <%if (item.OnLine)
              { %>
            <div class="dalerOnline">
            </div>
            <%} %>
            <a class="dealerLogoLink" href="/Products/<%= item.StringId %>/<%= currentCategory %>">
                <%= Html.Image("~/Image/ShowLogo/" + item.Id, new { style="border:1px solid #ccc;" })%>
            </a>
            <br />
            <a style="clear: both; display: block;" href="/Products/<%= item.StringId %>/<%= currentCategory %>">
                <%= item.Name %>
            </a>
        </div>
        <%}%>
    </div>
    <%  }
        foreach (var item in dealers)
        {%>
    <div class="dealerLogo">
        <%if (item.OnLine)
          { %>
        <div class="dalerOnline">
        </div>
        <%} %>
        <a class="dealerLogoLink" href="/Products/<%= item.StringId + "/" + currentCategory  %>">
            <%= Html.Image("~/Image/ShowLogo/" + item.Id, new { style="border:1px solid #ccc;" })%>
        </a>
        <br />
        <a style="clear: both; display: block;" href="/Products/<%= item.StringId %>/<%= currentCategory %>">
            <%= item.Name %>
        </a>
    </div>
    <%}
    %>
    <div style="clear: both">
    </div>
    <div class="categoriesHeader" style="margin-top: 20px;">
        <div class="categoriesHeaderTitle">
            <%= ViewData["categoryDescriptionTitle"] %>
        </div>
        <div class="categoryDescription">
            <%= ViewData["categoryDescription"] %>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterCss("~/Content/shadows.css")%>
    <%= Html.RegisterCss("~/Content/Dealers.css")%>
    <%= Html.RegisterJS("dropshadow.js")%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
    <% int expandedGroup = (int)ViewData["expandedGroup"]; %>
    <% int currentCategory = Convert.ToInt32(ViewContext.RouteData.Values["id"]); %>
    <% List<CategoryPresentation> categories = (List<CategoryPresentation>)ViewData["categories"]; %>
    <% Func<CategoryPresentation, int, string> selectedStyle = (category, id) =>
       {
           string result = "";
           if (!category.Additional && category.Id == id || (!category.Additional && category.Children.Where(c => c.Id == id).Count() > 0))
			   result = "active";
           return result;

       }; %>
    <% Func<CategoryPresentation, int, string> selectedStyleSubmenu = (category, id) =>
       {
           string result = "";
           if (!category.Additional && category.Id == id || (!category.Additional && category.Children.Where(c => c.Id == id).Count() > 0))
           //if (category.Id == id || (category.Children.Where(c => c.Id == id).Count() > 0))
			   result = "selected";
           return result;

       }; %>
    <div class="menu">
        <div class="menuHeader">
            <%= Html.ResourceString("Categories") %>
        </div>
        <div class="menuItems">
            <% foreach (var menuItem in categories)
               {
				   string className = "menuItem";
				   if (menuItem.Name.Length > 20)
					   className += " long";
				   %>
			<%--menuItem = selectedStyle(menuItem, currentCategory) --%>
            <div class="<%=className %>">
                <% if (menuItem.Additional)
                   {%>
                        <%=Html.ActionLink(menuItem.Name, "ShowProductsCategory", "Products", new { idCity = SystemSettings.CityId, id = menuItem.Id} ,
                                      new { @class = selectedStyle(menuItem, currentCategory) })%>
                <% }
                   else
                   {%>
                        <%=Html.ActionLink(menuItem.Name, "Index", "Dealers", new {id = menuItem.Id},
                                       new { @class = selectedStyle(menuItem, currentCategory) })%>
                <% } %>
            </div>
            <%if (menuItem.Id == expandedGroup && menuItem.Children.Count > 0)
              {
                  foreach (var subItem in menuItem.Children)
                  {%>
			<div class="subMenuItem <%= selectedStyleSubmenu(subItem, currentCategory) %>">
                <% if (subItem.Additional)
                   {%>
                        <%= Html.ActionLink(subItem.Name, "ShowProducts", "Products", new { idCity = SystemSettings.CityId, id = subItem.Id }, null)%>
                <% }
                   else
                   {%>
                        <%= Html.ActionLink(subItem.Name, "Index", "Dealers", new { id = subItem.Id }, null)%>
                <% } %>
            </div>
            <%   }%>
            <%} %>
            <%} %>
        </div>
    </div>
</asp:Content>
