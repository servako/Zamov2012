<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%--<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>--%>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% using(Html.BeginForm()){ %>    
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    
    
    <%= Html.RegisterJS("fckeditorapi.js") %>
    <%= Html.RegisterJS("fckeditor.js") %>
    <%= Html.RegisterJS("fcktools.js") %>
    <%= Html.RegisterJS("jquery.FCKeditor.js") %>
    
        <script type="text/javascript">
            $(function() {
                $.fck.config = { path: '<%= VirtualPathUtility.ToAbsolute("~/Controls/fckeditor/") %>', height: 300, config: { SkinPath: "skins/office2003/", DefaultLanguage: "RU", AutoDetectLanguage: false, HtmlEncodeOutput: true} };
                $('textarea#uaDescription, textarea#ruDescription').fck({ toolbar: "Common", height: 300 });
            });
    </script>
<table>
    <tr>
        <th>Укр</th>
        <th>Рус</th>
    </tr>
    <tr>
        <td>
            <%= Html.TextBox("uaTitle")%>
            <%= Html.TextArea("uaDescription") %>
        </td>
        <td>
            <%= Html.TextBox("ruTitle")%>
            <%= Html.TextArea("ruDescription")%>
        </td>
    </tr>
</table>
<input type="submit" value="Сохранить" /><br />

<%= Html.ResourceActionLink("BackToList", "Categories") %>
<%} %>
</asp:Content>