<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Zamov.Models.Order>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%
    bool cartMode = (ViewData["cartMode"] != null);
%>

<script type="text/javascript">
    if (!showOrdershadowsDeclared) {
        $(function() {
            applyDropShadows(".dealerImageLogo", "shadow3");
            $(".productDescription").fancybox({ frameWidth: 700, frameHeight: 500, frameHeight: 500 });
        })
        showOrdershadowsDeclared = true;
    }

    function order(element) {
        var fieldSegments = element.name.split("_");

        var id = fieldSegments[1];

        if (element.type == "checkbox") {
            var input = $get("quantity_" + id);
            if (input.value == "") {
                input.value = 1;
            }
        }
        else {
            var checkbox = $get("order_" + id);
            if (!checkbox.checked) {
                checkbox.checked = true;
            }
        }
    }
    
</script>
<div style="height:120px;">
    <% Html.RenderAction<PagePartsController>(ac => ac.CurrentDealer((int)Model.DealerReference.EntityKey.EntityKeyValues[0].Value)); %>
<%--<%= Html.Image("~/Image/ShowLogo/" + Model.DealerReference.EntityKey.EntityKeyValues[0].Value, new { style="border:1px solid #ccc;", @class="dealerImageLogo" })%>--%>
</div>

<table class="blueHeaderedTable">
    <tr class="blueHeader">
        <th>
            <%=Html.ResourceString("Title")%>
        </th>
        <th>
            <%=Html.ResourceString("Photo")%> / <%=Html.ResourceString("Description")%>
        </th>
        <th style="width:10px; padding-left:2px; padding-right:2px;">
            <%=Html.ResourceString("Unit")%>
        </th>
        <th>
            <%=Html.ResourceString("Price")%>, грн.
        </th>
        <th>
            <%=Html.ResourceString("Quantity")%>
        </th>
        <th>
            <%=Html.ResourceString("Cost")%>, грн.
        </th>
        <%if (cartMode){ %>
        <th></th>
        <%} %>
    </tr>
    <%
        
        decimal total = 0;
        foreach (OrderItem orderItem in Model.OrderItems)
        {
            decimal sum = orderItem.Quantity * orderItem.Price;
            total += sum;
    %>
    <tr>
        <td>
            <%=Html.Encode(orderItem.Name)%>
        </td>
        <td align="center">
            <a class="productDescription" style="text-decoration:none" href="/Products/Description/<%= orderItem.ProductId %><%= orderItem.Additional ? string.Format("/{0}", (int)ProductVersion.Additional) : "" %>"
            <%= (orderItem.ProductId==null)?"disabled":"" %>>
                <%= Html.Image("~/Content/img/productImage.JPG", new { style="border:none;" })%> / <span class="productDescriptionLink">i</span>
            </a>
        </td>
        <td align="center">
            <%=Html.Encode(orderItem.Unit)%>
        </td>
        <td align="center">
            <%=Html.Encode(orderItem.Price.ToString("N"))%>
        </td>
        <td <%= (cartMode) ? "align=\"center\"" : "align=\"right\""%>>
            <%= (cartMode) ? Html.TextBox("quantity_" + orderItem.HashCode, orderItem.Quantity, new { style= "width:12px; font-size:10px;", onblur="tableChanged(items, this)" }).ToHtmlString() : Html.Encode(orderItem.Quantity.ToString("N"))%>
        </td>
        <td  align="center">
            <%=Html.Encode(sum.ToString("N"))%>
        </td>
        <%if (cartMode)
          { %>
          <td>
            <%= Html.ResourceActionLink<CartController>("Delete", ac => ac.RemoveOrderItem(orderItem.HashCode, (string)ViewData["redirectUrl"]))%>
          </td>
        <%} %>
    </tr>
    <%
        }
            
    %>
    <tr>
        <td colspan="3">
        </td>
        <td align="center" style="font-weight:bold">
            <%=Html.ResourceString("Total")+":" %>
        </td>
        <td align="center" style="font-weight:bold">
            <%=Html.Encode(Model.OrderItems.Sum(oi=>oi.Quantity).ToString("N"))%>
        </td>
        <td align="right" style="font-weight:bold">
            <%=Html.Encode(total.ToString("N"))%>
        </td>
        <%if (cartMode){ %>
        <td></td>
        <%} %>
    </tr>
</table>
