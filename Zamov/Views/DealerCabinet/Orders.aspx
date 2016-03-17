<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Order>>" %>
<%@ Import Namespace="Zamov.Models"%>
<%@ Import Namespace="Zamov.Helpers"%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
        string showOrderUrl = Url.Action("ShowOrder");
    %>
<script type="text/javascript">
        var items = new Array();
        $(function() {


            $(".orderDescription")
            .fancybox(
            {
                frameWidth: 700,
                hideOnContentClick: false
            });
        })

        function updateOrdersTable(response) {
            for (var i in response) {
                var row = document.createElement("tr");
                var col1 = document.createElement("td");
                col1.innerHTML = response[i].Id;
                row.appendChild(col1);

                var col22 = document.createElement("td");
                col22.innerHTML = response[i].OrderDate;
                row.appendChild(col22);


                var col2 = document.createElement("td");
                col2.innerHTML = response[i].DeliveryDate;
                row.appendChild(col2);

                var col3 = document.createElement("td");
                if (response[i].StatusName == "Accepted")
                    col3.innerHTML = response[i].Address + ", " + response[i].ClientName;
                else
                    col3.innerHTML = response[i].Address;
                row.appendChild(col3);

                var col7 = document.createElement("td");
                col7.innerHTML = response[i].Comments + " " + response[i].DiscountCardNumber;
                row.appendChild(col7);

                var col4 = document.createElement("td");
                col4.innerHTML = response[i].TotalPrice;
                $(col4).attr("align", "right");
                row.appendChild(col4);


                var url = '<a href="<%= showOrderUrl%>/{0}" class="orderDescription">{1}</a>';
                var urlLayout = String.format(url, response[i].Id, response[i].Status);
                var col5 = document.createElement("td");
                $(urlLayout).appendTo(col5).fancybox({ frameWidth: 700, hideOnContentClick: false });
                //col5.innerHTML = response[i].Status;
                row.appendChild(col5);

/*
                var url = '<a href="<%= showOrderUrl%>/{0}" class="orderDescription">{1}</a>';
                var urlLayout = String.format(url, response[i].Id, '<%= Html.ResourceString("View")%>');
                var col6 = document.createElement("td");
                $(urlLayout).appendTo(col6).fancybox({ frameWidth: 700, hideOnContentClick: false });
                row.appendChild(col6);
*/


                var tableHeader = $get('orderTable').getElementsByTagName("tr")[0];
                $(row).insertAfter($(tableHeader))
            }
        }
    </script>
    
    
    
    
    <h3><%=Html.ResourceString("Orders")%></h3>

    <div id="div1"></div>

    <table class="commonTable" id="orderTable">
        <tr>
            <th style="width:20px;">
                № <%=Html.ResourceString("OfOrder")%>
            </th>
            <th style="width:40px;">
                <%=Html.ResourceString("OrderDate")%>
            </th>
            <th style="width:40px;">
                <%=Html.ResourceString("OrderDeliveryDateTime")%>
            </th>
            <th style="width:40px;">
                <%=Html.ResourceString("OrderInfo")%>
            </th>
            <th style="width:290px;">
                <%=Html.ResourceString("Comments")%>
            </th>
            <th style="width:20px;">
                <%=Html.ResourceString("Value")%>, грн.
            </th>
            <th style="width:20px;">
                <%=Html.ResourceString("Status")%>
            </th>
        </tr>

    <% foreach (var item in Model) 
       { 
           %>
      <tr class="status<%=(Statuses)item.Status%>">
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.Date)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.DeliveryDate)) %>
            </td>
            <td>
            
            <%=Html.Encode(item.Address)%>
            
            <%if (item.Status == (int)Statuses.Accepted) { %>
             <%=Html.Encode(", " + item.ClientName)%>
             <% } %>
            
            </td>
            <td>
                <div style="width:290px; overflow:hidden; text-overflow:ellipsis;">
                <%=Html.Encode(item.Comments+" ")%>
                <%if (!string.IsNullOrEmpty(item.DiscountCardNumber))
                  {
                      if (item.Status == (int)Statuses.Accepted)
                      {
                      %>
                  <%=Html.ResourceString("CardNumber")%>: <%=Html.Encode(item.DiscountCardNumber)%>
                  
                <%}
                      else { %>
                          <%=Html.ResourceString("CardNumber")%>: ******
                     <% }
                  } %>
                </div>
            </td>
            <td align="right">
                <%= Html.Encode(((decimal)item.OrderItems.Sum(oi=>oi.Quantity * oi.Price)).ToString("N"))%>
            </td>
            <td >
                <%=Html.ActionLink(Html.ResourceString("Status" + (Statuses)item.Status), "ShowOrder", new { id = item.Id }, new { @class = "orderDescription", id = "orderStatus" + item.Id })%>
            </td>
        </tr>
    
    <% } %>

    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
</asp:Content>
