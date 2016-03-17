<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string pickerLocale = (SystemSettings.CurrentLanguage == "uk-UA") ? "uk" : "ru";
    %>
	<script type="text/javascript">
	    $(function() {
	        $.datepicker.setDefaults($.extend({ showMonthAfterYear: false }, $.datepicker.regional['']));
	        $("#date").datepicker($.datepicker.regional["<%= pickerLocale %>"]);

	        $.fck.config = { path: '<%= VirtualPathUtility.ToAbsolute("~/Controls/fckeditor/") %>', height: 300, config: { SkinPath: "skins/office2003/", DefaultLanguage: "RU", AutoDetectLanguage: false, HtmlEncodeOutput: true} };
	        $('textarea#uaShortText').fck({ toolbar: "Common", height: 150 });
	        $('textarea#ruShortText').fck({ toolbar: "Common", height: 150 });

	        $('textarea#uaLongText').fck({ toolbar: "Common", height: 500 });
	        $('textarea#ruLongText').fck({ toolbar: "Common", height: 500 });
	    });
	</script>

    <h2><%= (ViewData["id"] == null) ? Html.ResourceString("EditNews") : Html.ResourceString("CreateNews")%></h2>
    <p>
        <%= Html.ValidationSummary(Html.ResourceString("NorAllProperties"))%>
    </p>
    <% using(Html.BeginForm())
       { %>
    <%= Html.TextBox("date") %>
    <table class="adminTable">
        <tr>
            <th></th>
            <th><%= Html.ResourceString("Ukr") %></th>
            <th><%= Html.ResourceString("Rus") %></th>
        </tr>
        <tr>
            <td>
                <%= Html.ResourceString("Header") %>
                <%= Html.ValidationMessage("title", "*", new { @class="validationError" })%>
            </td>
            <td>
                <%= Html.TextBox("uaTitle", ViewData["uaTitle"], new { style="width:430px" })%>
            </td>
            <td>
                <%= Html.TextBox("ruTitle", ViewData["ruTitle"], new { style = "width:430px" })%>
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.ResourceString("ShortText") %>
                <%= Html.ValidationMessage("shortText", "*", new { @class="validationError" })%>
            </td>
            <td>
                <%= Html.TextArea("uaShortText") %>
            </td>
            <td>
                <%= Html.TextArea("ruShortText")%>
            </td>
        </tr>
        <tr>
            <td>
                <%= Html.ResourceString("LongText") %>
                <%= Html.ValidationMessage("longText", "*", new { @class="validationError" })%>
            </td>
            <td>
                <%= Html.TextArea("uaLongText")%>
            </td>
            <td>
                <%= Html.TextArea("ruLongText")%>
            </td>
        </tr>
    </table>
    <%= Html.CheckBox("enabled") + " " + Html.ResourceString("ActiveF") %>
    <br />
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
     <%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    <%= Html.RegisterJS("ui.datepicker-ru.js")%>
    <%= Html.RegisterJS("ui.datepicker-uk.js")%>
</asp:Content>
