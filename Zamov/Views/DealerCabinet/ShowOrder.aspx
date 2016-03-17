<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Zamov.Models.Order>" %>
<%@ Import Namespace="Zamov.Helpers"%>
<%@ Import Namespace="Zamov.Models"%>

<script type="text/javascript">
    var orderId = '<%= Model.Id %>';
    var orderStatus = '<%= Html.ResourceString("StatusAccepted") %>';

    function orderAccepted(response) {
        fadeScreenOut();
        $("#fancy_ajax").load("/DealerCabinet/ShowOrder/" + orderId, null, function() { fadeScreenIn(); });
        $("#orderStatus" + orderId).html(orderStatus);
        $("#orderStatus" + orderId).parent().parent().attr("class", "statusAccepted");
    }
    
</script>
<div align="center">
<div align="left"  style="padding-left:30px; padding-top:10px;">
    <p>
    <b><%=Html.ResourceString("Orders")%> ¹ <%=Html.Encode(Model.Id) %></b>
   </p>
<%if (Model.Status == (int)Statuses.Accepted)
  { %>
        <p>
            <%=Html.ResourceString("Client")%>:
            <%=Html.Encode(Model.ClientName)%>
        </p>
        <p>
            <%=Html.ResourceString("Phone")%>:
            <%=Html.Encode(Model.Phone)%>
        </p>
        <p>
            <%=Html.ResourceString("Email")%>:
            <%=Html.Encode(Model.Email)%>
        </p>
<%} %>
        <%if (!string.IsNullOrEmpty(Model.DiscountCardNumber))
          { %>
        <p>
            <%=Html.ResourceString("VoucherNumber")%>:
            <%
                if (Model.Status == (int)Statuses.Accepted)
                {
            %>
                <%=Html.Encode(Model.DiscountCardNumber)%>
                  
            <%  }
                else
                { 
                    %>******<% 
                }
            %>
        </p>
        <%} %>
        <p>
            <%=Html.ResourceString("PaymentType")%>:
            <%=Html.ResourceString(((PaymentTypes)Model.PaymentType).ToString())%>
        </p>
        <p>
            <%=Html.ResourceString("OrderDeliveryDateTime") %>:
            <%= Html.Encode(String.Format("{0:g}", Model.DeliveryDate)) %>
        </p>
        <p>
            <%=Html.ResourceString("City") %>:
            <%= Html.Encode(Model.City) %>
        </p>
        <p>
            <%=Html.ResourceString("DeliveryAddress") %>:
            <%= Html.Encode(Model.Address) %>
        </p>

    <br />
</div>    
    <table class="commonTable" style="width:650px;">
        <tr>
            <th>¹ ï.ï.</th>
            <th><%=Html.ResourceString("PartNumber") %></th>
            <th><%=Html.ResourceString("Title") %></th>
            <th><%=Html.ResourceString("Quantity") %></th>
            <th><%=Html.ResourceString("Price") %></th>
        </tr>
    <%
    int cnt = 0;
    decimal total = 0;
    foreach (OrderItem orderItem  in Model.OrderItems)
    {
        decimal sum = orderItem.Price * orderItem.Quantity;
        total += sum;
        %>
        <tr>
            <td><%=Html.Encode(++cnt)%></td>
            <td><%=Html.Encode(orderItem.PartNumber)%></td>
            <td><%=Html.Encode(orderItem.Name)%></td>
            <td><%=Html.Encode(orderItem.Quantity)%></td>
            <td align="right"><%=Html.Encode(sum.ToString("N"))%></td>
        </tr>
        <%
    }
%>
<tr>
    <td colspan="3"></td>
    <td><%=Html.ResourceString("Total") %></td>
    <td align="right"><%=Html.Encode(total.ToString("N"))%></td>
</tr>
</table>
<br />


<%if (Model.Status == (int)Statuses.New)
  { %>
<table>
<tr>
    <td>
    <%
        using (Ajax.BeginForm("AcceptOrder", "DealerCabinet", new AjaxOptions { OnSuccess = "orderAccepted", OnFailure = "failureCallback", OnBegin = "fadeScreenOut", OnComplete = "fadeScreenIn" }))
        {%>
            <input type="submit" value="<%=Html.ResourceString("AcceptOrder") %>" />
            <%=Html.Hidden("orderId", Model.Id)%>
            
      <%}%>
    </td>
    <td>
    <%
    using (Html.BeginForm("CancelOrder", "DealerCabinet", FormMethod.Post))
    {%>
            <input type="submit" value="<%=Html.ResourceString("CancelOrder")%>" />
            <%=Html.Hidden("orderId", Model.Id)%>
            
      <%
        
    }%>
    </td>
</tr>
</table>
<%}%>
</div>
