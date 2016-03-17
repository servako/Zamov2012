<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>

<% using(Html.BeginForm()){ %>
    <%= Html.Hidden("userType")%>
    <%= Html.Hidden("sortField")%>
    <%= Html.Hidden("sortOrder")%>
    <%= Html.Hidden("email")%>
    
    <table>
        <tr>
            <td colspan="4">
                <%= Html.ResourceString("Manager") %>: 
                <%= Html.CheckBox("dealerEmployee")%>&nbsp;
                <%= Html.DropDownList("dealerId") %>
            </td>
        </tr>
        <tr>
            <td><%= Html.ResourceString("FirstName") %>:</td>
            <td><%= Html.TextBox("firstName") %></td>
            <td><%= Html.ResourceString("LastName") %>:</td>
            <td><%= Html.TextBox("lastName") %></td>
        </tr>
        <tr>
            <td><%= Html.ResourceString("City") %>:</td>
            <td><%= Html.TextBox("city") %></td>
            <td rowspan="3" colspan="2">
                <%= Html.ResourceString("DeliveryAddress") %>:<br />
                <%= Html.TextArea("deliveryAddress", new { rows = 4, style="width:99%;"})%>
            </td>
        </tr>
        <tr>
            <td><%= Html.ResourceString("Phone") %>:</td>
            <td><%= Html.TextBox("mobilePhone") %></td>
        </tr>
        <tr>
            <td><%= Html.ResourceString("Phone") %>:</td>
            <td><%= Html.TextBox("phone") %></td>
        </tr>
    </table>
    <div style="float:right">
        <input type="submit" value="<%= Html.ResourceString("Save") %>" />
        <input type="button" value="<%= Html.ResourceString("Cancel") %>" onclick="$('.updateUser').html('').css('display', 'none');" />
    </div>
<%} %>