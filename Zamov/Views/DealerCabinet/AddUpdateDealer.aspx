<%@ Page Title="" Language="C#" MasterPageFile="~/Views/DealerCabinet/Cabinet.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
<script type="text/javascript">
    $(function() {
        $.fck.config = { path: '<%= VirtualPathUtility.ToAbsolute("~/Controls/fckeditor/") %>', height: 300, config: { SkinPath: "skins/office2003/", DefaultLanguage: "RU", AutoDetectLanguage: false, HtmlEncodeOutput: true} };
        $('textarea#rDescription').fck({ toolbar: "Common", height: 600 });
        $('textarea#uDescription').fck({ toolbar: "Common", height: 600 });
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
    string russianGroupName = "";
    string ukrainianGroupName = "";
    bool enabled = true;
    string banner="";
    bool bannerEnabled = true;
    int dealerId = int.MinValue;
    string phone = "";
        
    if (dealer != null)
    {
        dealerName = dealer.Name;
        if (dealer.LogoImage.Length > 0)
            logoUrl = Url.Action("ShowLogo", "Image", new { id=dealer.Id });
        russianName = dealer.GetName("ru-RU");
        ukrainianName = dealer.GetName("uk-UA");
        russianDescription = dealer.GetDescription("ru-RU");
        ukrainianDescription = dealer.GetDescription("uk-UA");
        russianGroupName = dealer.GetGroupName("ru-RU");
        ukrainianGroupName = dealer.GetGroupName("uk-UA");
        
        dealerId = dealer.Id;
        enabled = dealer.Enabled;
        this.Title = pageTitle;
        phone = dealer.Phone;
        bannerEnabled = dealer.BannerEnabled;
        banner = dealer.Banner;
    }
%>
    <h2><%= pageTitle %></h2>
    <% using (Html.BeginForm("AddUpdateDealer", "DealerCabinet", FormMethod.Post, new{enctype="multipart/form-data"})) {%>
        <div id="dealerIdDiv">
            ID<br />
            <%= Html.TextBox("name", dealerName) %><br />
        </div>
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
                <td><%= Html.TextBox("uName", ukrainianName, new { style = "width:240px;" })%></td>
                <td><%= Html.TextBox("rName", russianName, new { style = "width:240px;" })%></td>
            </tr>
            <tr>
                <td><%= Html.ResourceString("Description") %></td>
                <td><%= Html.TextArea("uDescription", ukrainianDescription) %></td>
                <td><%= Html.TextArea("rDescription", russianDescription) %></td>                
            </tr>
            <tr>
                <td><%= Html.ResourceString("DealerGroupName") %></td>
                <td><%= Html.TextBox("uGroupName", ukrainianGroupName, new { style = "width:240px;" })%></td>
                <td><%= Html.TextBox("rGroupName", russianGroupName, new { style = "width:240px;" })%></td>
            </tr>
            <tr>
                <td><%= Html.ResourceString("Phone")%></td>
                <td colspan="2">
                    <%=Html.TextArea("phone", phone, 5, 50, null)%>
                </td>
            </tr>
            
            <%
                if (Roles.IsUserInRole("Administrators"))
                {
            %>
            <tr>
                <td><%=Html.ResourceString("Banner")%></td>
                <td>Файл<br />
                 <%=Html.RegisterFlashScript(banner, "b_1", BannerPosition.Top)%>
                 <input type="file" name="banner" />
                </td>
                <td><%=Html.ResourceString("ActiveM")%>
                <%=Html.CheckBox("bannerEnabled", bannerEnabled)%>
                </td>
            </tr>
            <%
                }%>
        </table>
        
        <br />
        <%= Html.CheckBox("hasDiscounts") %><%= Html.ResourceString("HasDiscounts") %>
        
        <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <% } %>

    <div>
        <%=Html.ActionLink("Назад", "Dealers") %>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    
    <%if(User.IsInRole("Administrators")){ %>
        <style type="text/css">
            #dealerIdDiv{display:block;}
        </style>
    <%} %>
    <%else{ %>
        <style type="text/css">
            #dealerIdDiv{display:none;}
        </style>
    <%} %>
</asp:Content>