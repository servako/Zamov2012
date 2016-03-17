<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%
    int newOrdersCount = 0;
    using (OrderStorage context = new OrderStorage())
    {
        newOrdersCount = context.Orders.Where(o => o.Status == (int)Statuses.New).Count();
    }
%>

<script type="text/javascript">
    $(function() {
        setInterval(queryOrders, 180000);
    });

        function queryOrders() {
            var webRequest = Sys.Net.WebServiceProxy.invoke(
            "/Services/Tools.asmx",
            "GetNewOrders",
            false,
            null,
            function(response) {
                updateNewOrdersCount(response.NewOrdersCount);
                if (response.NewOrders && response.NewOrders.length > 0) {
                    self.focus();
                    alert('<%= Html.ResourceString("YouHaveNewOrder") %>');
                    location.href = "/Reports/SalesReport";
                }
                if (typeof (updateOrdersTable) !== "undefined")
                    updateOrdersTable(response.NewOrders);
            },
            failureCallback);
        }

        function updateNewOrdersCount(newCount) {
            $("#dealerOrdersCount").html(newCount);
        }
</script>

<div id="dealerOrdersInfo">
    <a href="/Reports/SalesReport">
    <%= Html.ResourceString("YouHaveNewOrders") + ":" %>
    <span id="dealerOrdersCount">
        <%= newOrdersCount %>
    </span>
    </a>
</div>