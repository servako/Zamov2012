<%@ Page Title="" Language="C#" MasterPageFile="~/Views/UserCabinet/UserCabinet.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">        
    <%using(Html.BeginForm()){ %>
        <fieldset style="margin: 20px 0px; padding:10px;">
            <legend>
                <%= Html.ResourceString("YourDetails")%></legend>
            <table class="registrationTable">
                <tr>
                    <td>
                        <label for="firstName">
                            <%= Html.ResourceString("FirstName") %>:</label>
                        <%= Html.ValidationMessage("firstName", "*", new { @class = "validationError" })%>
                    </td>
                    <td>
                        <%= Html.TextBox("firstName")%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="lastName">
                            <%= Html.ResourceString("LastName") %>:</label>
                        <%= Html.ValidationMessage("lastName", "*", new { @class = "validationError" })%>
                    </td>
                    <td>
                        <%= Html.TextBox("lastName")%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="city">
                            <%= Html.ResourceString("City") %>:</label>
                        <%= Html.ValidationMessage("city", "*", new { @class = "validationError" })%>
                    </td>
                    <td>
                        <%= Html.TextBox("city")%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="deliveryAddress">
                            <%= Html.ResourceString("DeliveryAddress") %>:</label>
                        <%= Html.ValidationMessage("deliveryAddress", "*", new { @class = "validationError" })%>
                    </td>
                    <td>
                        <%= Html.TextArea("deliveryAddress")%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="mobilePhone">
                            <%= Html.ResourceString("Phone") %>:</label>
                        <%= Html.ValidationMessage("mobilePhone", "*", new { @class = "validationError" })%>
                    </td>
                    <td>
                        <%= Html.TextBox("mobilePhone")%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="phone">
                            <%= Html.ResourceString("Phone") %>:</label>
                    </td>
                    <td>
                        <%= Html.TextBox("phone")%>
                    </td>
                </tr>
            </table>
        </fieldset>
        <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>
</asp:Content>
