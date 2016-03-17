<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Zamov.Models.NewsPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%= Html.RegisterCss("~/Content/News.css")%>
<div class="menu">
    <div class="menuHeader">
        <%= ViewData["caption"] %>
    </div>
        <% foreach (var item in Model)
           {
               string url = "/news/details/"+item.Id;
               string text = HttpUtility.HtmlDecode(item.ShortText);
               string[] x = text.Split(new string[] {"<div>", "</div>"}, StringSplitOptions.RemoveEmptyEntries);
               text = x[0];
               text = (text.Length > 50 ? text.Substring(0, 50) : text);
               text += "...";
               
               %>
            <div class="detailsNewsInMenuLinkDiv">
                <table class="dNewsHeader" cellpadding="0" cellpadding="0" style="border-collapse:collapse">
                <tr>
                    <td class="dNewsTitle" align="left"><%=item.Title%></td>
                    <td class="dNewsDate" align="right"><%=item.Date.ToShortDateString()%></td>
                </tr>
                </table>
                
               
                <div class="dNewsShortText"><%=text%></div>
                <div class="dNewsDetailsLink">
                    <a href="<%= url %>" class="detailsNewsInMenuLink">Подробнее</a>
                </div>
            </div>           
          <% } 
        %>
    <div class="newsFooter"><a href="/news">все новости</a></div>
</div>
