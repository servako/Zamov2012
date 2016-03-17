<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    
<script type="text/javascript">
    $(function() {
        $.fck.config = { path: '<%= VirtualPathUtility.ToAbsolute("~/Controls/fckeditor/") %>', height: 300, config: { SkinPath: "skins/office2003/", DefaultLanguage: "RU", AutoDetectLanguage: false, HtmlEncodeOutput: true} };
        $('textarea#rDescription').fck({ toolbar: "NoHyperlinks", height: 500 });
        $('textarea#uDescription').fck({ toolbar: "NoHyperlinks", height: 500 });
    });
</script>
<%
    Dealer dealer = null;
    if(ViewData["dealer"]!=null)
        dealer = (Dealer)ViewData["dealer"];
    string pageTitle = (dealer == null) ? Html.ResourceString("CreateDealer") : Html.ResourceString("EditDealer");
    string dealerName = "";
    string logoUrl = VirtualPathUtility.ToAbsolute("~/Comtent/img/noLogo");
    string russianName = "";
    string ukrainianName = "";
    string russianDescription = "";
    string ukrainianDescription = "";
    bool enabled = true;
    int dealerId = int.MinValue;
        
    if (dealer != null)
    {
        dealerName = dealer.Name;
        if (dealer.LogoImage.Length > 0)
            logoUrl = Url.Action("ShowLogo", "Image", new { id=dealer.Id });
        russianName = dealer.GetName("ru-RU");
        ukrainianName = dealer.GetName("uk-UA");
        russianDescription = dealer.GetDescription("ru-RU");
        ukrainianDescription = dealer.GetDescription("uk-UA");
        dealerId = dealer.Id;
        enabled = dealer.Enabled;
        this.Title = pageTitle;
    }
%>
    <h2><%= pageTitle %></h2>
    <% using (Html.BeginForm("AddUpdateDealer", "Admin", FormMethod.Post, new{enctype="multipart/form-data"})) {%>
        <%= Html.Hidden("dealerId", dealerId)%>
        ID<br />
        <%= Html.TextBox("name", dealerName) %><br />
        Логотип<br />
        <%= Html.Image(logoUrl) %><br />
        <input type="file" name="logoImage" />
        <table class="adminTable">
            <tr>
                <th></th>
                <td>Укр</td>
                <td>Рус</td>
            </tr>
            <tr>
                <td><%= Html.ResourceString("Name") %></td>
                <td><%= Html.TextBox("uName", ukrainianName, new { style = "width:430px;" })%></td>
                <td><%= Html.TextBox("rName", russianName, new { style="width:430px;"})%></td>
            </tr>
            <tr>
                <td><%= Html.ResourceString("Description") %></td>
                <td><%= Html.TextArea("uDescription", ukrainianDescription) %></td>
                <td><%= Html.TextArea("rDescription", russianDescription) %></td>
            </tr>
        </table>
        <br />
        <%= Html.CheckBox("enabled", enabled) %><%= Html.ResourceString("On") %><br />
        <br />
        <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <% } %>

    <div>
        <%=Html.ActionLink("Назад", "Dealers") %>
    </div>

</asp:Content>