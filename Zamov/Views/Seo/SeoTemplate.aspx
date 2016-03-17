<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Seo/Seo.Master" Inherits="System.Web.Mvc.ViewPage<Dictionary<string, Dictionary<string, string>>>" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h2>Template for seo</h2>
<% using (Html.BeginForm())
   {%>    
    <table class="adminTable" style="border:1px dotted #ccc" >
        <tr>
            <th>name template
            </th>
            <th>
                <%=Html.ResourceString("Ukr")%>
            </th>
            <th>
                <%=Html.ResourceString("Rus")%>
            </th>
        </tr>
    <% foreach (var item in Model)
        { %>    
        <tr>
            <td>
                <%=Html.Encode(item.Key)%>
            </td>
            <td>
                <%=Html.TextBox("uk-UA_" + item.Key,
                                   item.Value.ContainsKey("uk-UA") ? item.Value["uk-UA"] : string.Empty)%>
            </td>
            <td>
                <%=Html.TextBox("ru-RU_" + item.Key,
                                   item.Value.ContainsKey("ru-RU") ? item.Value["ru-RU"] : string.Empty)%>
            </td>
        </tr>
        
    <% } %>

    </table>
    <input type="submit" value="<%=Html.ResourceString("Save")%>" />    
<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
