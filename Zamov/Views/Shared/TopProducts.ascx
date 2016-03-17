<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Zamov.Models.Product>>" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<% 
    List<Product> topProducts = ViewData["topProducts"] as List<Product>;
    if (topProducts.Count > 0)
    {
%>

<script type="text/javascript">
    var i = 1;
    var intervalId = null;
    $(function() {
        $(".topProductLink").fancybox({ frameWidth: 700, frameHeight:500, hideOnContentClick: false });
        intervalId = window.setInterval(alignImages, 200);
    })

    function alignImages() {
        $("table.topProducts tr td div img").each(function() {
            this.style.marginTop = (98 - $(this).height()) / 2 + "px";
        })
        i++;
        if (i == 100) {
            window.clearInterval(intervalId);
        }
    }
</script>

<div class="topProducts" style="margin-bottom:25px;>
        <span class="header">
            <%= Html.ResourceString("TopProducts").ToUpper() %>
        </span>
        <table class="topProducts">
            <tr class="topProduct" align="center" valign="top">
                <%foreach (Product item in topProducts)
                  {%>
                  <td>
    <%using (Html.BeginForm("AddToCart", "Products", FormMethod.Post)){
          int dealerId = (int)ViewData["dealerId"];
          %>
                    <input type="hidden" name="dealerId" value="<%= dealerId %>" />
                    <%= Html.Hidden("groupId")%>            
                    <%= Html.Hidden("categoryId") %> 
                    <%= Html.CheckBox("order_" + item.Id, true, new { style="margin-left:-8000px; position:absolute;" })%>
                    <%= Html.Hidden("quantity_" + item.Id, 1)%>
                    <div class="topImage">
                        <a href="/Products/Description/<%= item.Id %>" class="topProductLink">
                            <%= Html.Image("~/Image/ProductImageDefault/" + item.Id + "/98", item.Name, new { style = "display:block; border:none;" })%>
                        </a>
                    </div>
                    <div class="topText" title="<%= item.Name %>">
                        <%= item.Name %><br />

                    </div>
                    <span>
                            <%= item.Price.ToString("F") %> грн.
                    </span>
                    <br />
                    <input style="font-size:0.9em" type="submit" value="<%= Html.ResourceString("ToOrder") %>" /><%} %>
                  </td>
                <%
                    } %>
            </tr>
        </table>
</div>
<%} %>
