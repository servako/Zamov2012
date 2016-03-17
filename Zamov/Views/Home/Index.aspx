<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CategoryPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ContentPlaceHolderID="TopBanner" runat="server">
<%
    var adverts = (IEnumerable<Advert>) ViewData["advert"];
    Advert advert = adverts.Select(a => a).Where(a => a.Position == (int)BannerPosition.Top).First();
    if (advert != null)
        Html.RenderPartial("TopBanner", advert);
%>
</asp:Content>
 <asp:Content ContentPlaceHolderID="BottomBanner" runat="server">
 <%
     var adverts = (IEnumerable<Advert>)ViewData["advert"];
     if(adverts.Where(b=>b.IsActive&&b.Position!=(int)BannerPosition.Top).Count()>0)
     Html.RenderPartial("BottomBanner",adverts);
 %>
 </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="categoriesHeader">
        <div class="categoriesHeaderTitle">
            <%= Html.ResourceString("Categories") %>
        </div>
    </div>   
    <%foreach (var item in Model)
      {
          string single = (item.Children.Count == 0) ? " single" : "";
          if (item.Name.Length > 20)
              single += " long";
         %>
        <div class="mainCategory<%= single %>">
            <div class="categoryButton">
                 <% if (item.Additional)
                    {%>
                        <%=Html.ActionLink(item.Name, "ShowProductsCategory", "Products", new { idCity = SystemSettings.CityId, id = item.Id }, null)%>
                 <% }
                    else
                    { %>
                        <%=Html.ActionLink(item.Name, "Index", "Dealers", new { id = item.Id }, null)%>
                 <% } %>
            </div>
            <div class="categoryImage">
                 <% if (item.Additional)
                    {%>                                    
                        <%= Html.ActionLink("IMAGE", "ShowProductsCategory", "Products", new { idCity = SystemSettings.CityId, id = item.Id }, null).ToHtmlString().Replace("IMAGE",
                            Html.Image("~/Image/CategoryImageByCategoryId/" + item.Id).ToHtmlString()) %>
                 <% }
                    else
                    { %>
                        <%= Html.ActionLink("IMAGE", "Index", "Dealers", new { id = item.Id }, null).ToHtmlString().Replace("IMAGE",
                            Html.Image("~/Image/CategoryImageByCategoryId/" + item.Id).ToHtmlString()) %>
                 <% } %>
            </div>
            <div class="subCategories">
                <% bool last = true;
                   int i = 0;
                   foreach (var subCategory in item.Children.Take(5))
                   {
                       last = (i == 4 || i==item.Children.Count - 1);
                       %>
                       <span>
                <% if (item.Additional)
                   {%> 
                        <%= Html.ActionLink(subCategory.Name, "ShowProducts", "Products", new { idCity = SystemSettings.CityId, id = subCategory.Id }, null)%>
                <% }
                    else
                   { %>
                        <%= Html.ActionLink(subCategory.Name, "Index", "Dealers", new { id = subCategory.Id }, null)%>
                <% } %>
                    <%if (!last)Response.Write(" / ");%>
                    </span>
                 <%
                    i++;
                 
                 } %>
            </div>
        </div>
    <%} %>
    <div style="clear:both"></div>
     <div class="categoriesHeader">
        <div class="categoriesHeaderTitle">
           <%= Html.ResourceString("AboutUsALittle") %>
        </div>
    </div>  
    <div class="aboutUs">
    <%=HttpUtility.HtmlDecode(ApplicationData.GetAboutUsALittle(SystemSettings.CurrentLanguage)) %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterCss("~/Content/StartPage.css") %>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterCss("~/Content/News.css")%>
    
    <script type="text/javascript">
        $(function() {
        $(".detailsNewsInMenuLink").fancybox({ frameWidth: 700, hideOnContentClick: false });
        })
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTop" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="leftMenu" runat="server">
<div class="NewsAndLeftMenuContainer">
    <% IEnumerable<SelectListItem> cities = (IEnumerable<SelectListItem>)ViewData["cities"]; %>
    <% Html.RenderAction<PagePartsController>(ac => ac.LeftMenu(Html.ResourceString("City"), cities)); %>
    
    <% IEnumerable<NewsPresentation> news = (IEnumerable<NewsPresentation>)ViewData["news"]; %>
    <% Html.RenderAction<PagePartsController>(ac => ac.News(Html.ResourceString("News"), news)); %>
</div>    
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="dealerLogo" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentBottom" runat="server">
</asp:Content>
