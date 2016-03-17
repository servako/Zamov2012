<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%
    int newOrdersCount = 0;
    using(OrderStorage context = new OrderStorage())
    {
        newOrdersCount = context.Orders.Where(o => o.Status == (int)Statuses.New).Count();
    }
%>

<div id="dealerOrdersInfo">
    <a href="/Reports/SalesReport">
    <%= Html.ResourceString("YouHaveNewOrders") + ":" %>
    <span id="dealerOrdersCount">
        <%= newOrdersCount %>
    </span>
    </a>
</div>