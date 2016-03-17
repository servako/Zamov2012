<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.ExchangeRate>>" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<%
        string pickerLocale = (SystemSettings.CurrentLanguage == "uk-UA") ? "uk" : "ru";
    %>
	<script type="text/javascript">
		$(function () {
			$.datepicker.setDefaults($.extend({ showMonthAfterYear: false }, $.datepicker.regional['']));
			$("#date, .datepick").datepicker($.datepicker.regional["<%= pickerLocale %>"]);
		});
	</script>
    <h2>Курсы валют</h2>
<% using (Html.BeginForm("UpdateCurrencyRates", "DealerCabinet"))
   { %>
    <table class="adminTable" style="border:1px dotted #ccc" >
        <tr>
            <th style="display:none">
                Id
            </th>
            <th>
                Валюта
            </th>
            <th>
                Курс
            </th>
			<th>
                Дата
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td style="display:none">
                <%= Html.TextBox("currency_" + item.xrID, item.xrID)%>
            </td>
            <td>
                <%= Html.Encode(item.Currencies.Name)%>
            </td>
            <td>
                <%=Html.TextBox("rate_" + item.xrID, String.Format("{0:F}", item.xr_value))%>
            </td>
			<td>
                <%=Html.TextBox("date_" + item.xrID, item.xr_dateBeg.ToShortDateString(), new { @class = "datepick" })%>
            </td>
        </tr>
    
    <% } %>

    </table>
 <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>

<% using (Html.BeginForm("InsertCurrencyRate", "DealerCabinet"))
   { %>
    <table class="adminTable">
        <tr>
            <th>
                Валюта    
            </th>
            <th>
                Курс
            </th>
			<th>
                Дата
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.DropDownList("currencysList", (List<SelectListItem>)ViewData["currencies"])%> 
            </td>
            <td>
                <%= Html.TextBox("rate") %>
            </td>
			<td>
                <%= Html.TextBox("date", DateTime.Now.Date.ToShortDateString())%>
            </td>
        </tr>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Add") %>" />
    <%} %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("ui.datepicker-ru.js")%>
    <%= Html.RegisterJS("ui.datepicker-uk.js")%>
</asp:Content>