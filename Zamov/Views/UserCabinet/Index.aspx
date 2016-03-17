<%@ Page Title="" Language="C#" MasterPageFile="~/Views/UserCabinet/UserCabinet.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Order>>" %>
<%@ Import Namespace="Zamov.Controllers"%>
<%@ Import Namespace="Zamov.Helpers"%>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
        var items = new Array();
        $(function() {
            $(".cartDescription")
            .fancybox(
            {
                frameWidth: 700,
                //frameHeight: 500,
                hideOnContentClick: false
            });
        })



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
    <%=Html.ResourceString("OrdersHistory") %>
    
   
    <table class="commonTable">
        <tr>
<%--            <th>№ <%=Html.ResourceString("OfCart")%></th>--%>
            <th><%=Html.ResourceString("Date")%></th>
            <th>№ <%=Html.ResourceString("OfOrder")%></th>
            <th><%=Html.ResourceString("Dealer")%></th>
            <th><%=Html.ResourceString("Cost")%>, грн</th>
            <th><%=Html.ResourceString("OrderStatus")%></th>
            <th></th>
            <th></th>
        </tr>
    <%

        int PrevId=0;
        foreach (Order order in Model) 
        {
        int NextId=order.Cart.Id;
        int c = order.Cart.Orders.Count; //order.OrderItems.Count;
        if (PrevId != NextId)
        {
        %>
        <tr class="status<%=((Statuses)order.Status) %>">
            <%--<td rowspan="<%=Html.Encode(c)%>"><%=Html.Encode(order.Cart.Id)%></td>--%>
            <td rowspan="<%=Html.Encode(c)%>"><%=Html.Encode(String.Format("{0:dd.MM.yyyy HH:mm}", order.Cart.Date))%></td>
            <td><%=Html.Encode(order.Id)%></td>
            <td><%=Html.Encode(order.Dealer.GetName(SystemSettings.CurrentLanguage))%></td>
            <td><%=Html.Encode(order.OrderItems.Sum(oi => oi.Price*oi.Quantity))%></td>
            <td><%=Html.ActionLink(Html.ResourceString("Status" + (Statuses)order.Status), "ShowCart", new { id = order.Cart.Id/*, caller = "userCabinet"*/ }, new { @class = "cartDescription" })%></td>
            <td rowspan="<%=Html.Encode(c)%>"><%=Html.ActionLink(Html.ResourceString("Delete"), "DeleteCart", new { id = order.Cart.Id }, new {@class="redLink", onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%></td>
            <td><%=Html.ActionLink(Html.ResourceString("RepeatOrder"), "AddToCart", new { id = order.Id})%></td>
        </tr>
        <%
        }
        else
        {
            %>
            <tr>
                <td><%=Html.Encode(order.Id)%></td>
                <td><%=Html.Encode(order.Dealer.GetName(SystemSettings.CurrentLanguage))%></td>
                <td><%=Html.Encode(order.OrderItems.Sum(oi => oi.Price * oi.Quantity))%></td>
                <td><%=Html.ActionLink(Html.ResourceString("Status" + (Statuses)order.Status), "ShowCart", new { id = order.Cart.Id/*, caller = "userCabinet"*/ }, new { @class = "cartDescription" })%></td>
                <td><%=Html.ActionLink(Html.ResourceString("RepeatOrder"), "AddToCart", new { id = order.Id})%></td>            
            </tr>
            
            <%
        }
            PrevId = NextId;
    }   %>
    </table>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterCss("~/Content/shadows.css")%>
    <%= Html.RegisterJS("dropshadow.js")%>
</asp:Content>