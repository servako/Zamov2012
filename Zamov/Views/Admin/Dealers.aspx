<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Dealer>>" %>

<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Models" %>

<asp:Content ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.tablednd_0_5.js")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    $(function () {
        $("#tableDealers").tableDnD();
    $("#dealerFeedback")
                .dialog({
                    autoOpen: false,
                    width: 700,
                    height: 600,
                    minHeight: 360,
                    resizable: false
                });
    })
    function feedback(id) {
        $("#dealerFeedback")
                .html('<iframe frameborder="0" name="feedback" id="feedback" hidefocus="true" style="width:660px; height:500px;" src="/Feedback/ModifyFeedback/' + id + '"></iframe>');
        $("#dealerFeedback").dialog('open');
    }
</script>
<div id="dealerFeedback"></div>
    <h2><%= Html.ResourceString("Dealers") %></h2>
    <% using (Html.BeginForm()){ %>
    <table class="adminTable" id="tableDealers">
        <tr>
            <th>
               #
            </th>
            <th>
                ID
            </th>
            <th>
                <%= Html.ResourceString("Name")%>
            </th>
            <th>
                <%= Html.ResourceString("Top")%>
            </th>
            <th>
                <%= Html.ResourceString("ActiveM")%>
            </th>
            <th>
                <%= Html.ResourceString("OrderLifeTime")%>
            </th>
            <th>
                загрузка XML
            </th>
            <th></th>
            <th></th>
            <th></th>
            <th></th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%= Html.Encode(item.IndexNumber)%>
            </td>
            <td>
                <%= Html.Encode(item.Id)%>
            </td>
            <td>
                <%= Html.Encode(item.GetName(SystemSettings.CurrentLanguage))%>
            </td>
            <td align="center">
                <%= Html.CheckBox("top_" + item.Id, item.TopDealer)%>
            </td>
            <td align="center">
                <%= Html.CheckBox("enabled_" + item.Id, item.Enabled)%>
<%--                <% 
                    if (!item.Enabled)
                        Response.Write(Html.ResourceActionLink("On", "EnableDealer", new { id=item.Id }));
                    else
                        Response.Write(Html.ResourceActionLink("Off", "DisableDealer", new { id = item.Id }));
                %>--%>
            </td>
            <td>
                <%= Html.TextBox("orderLifeTime_" + item.Id, item.OrderLifeTime) %>
            </td>
            <td>
                <%= Html.CheckBox("loadXml_" + item.Id, item.LoadXml)%>
            </td>
<%--            <td>
                <%=Html.ResourceActionLink<AdminController>("Edit", ac => ac.AddUpdateDealer(item.Id))%>
            </td>--%>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Contacts"), "DealerContacts", new { id = item.Id })%>
            </td>
            <td>
                <a href="javascript:feedback(<%= item.Id %>)">
                    <%= Html.ResourceString("Feedback")%>
                </a>
            </td>
            <td>
                <%= Html.ActionLink("SEO", "SeoSettings", new { id = item.Id, seoEntityType = (int)SeoEntityType.Dealers })%>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteDealer", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>
        </tr>
        <% } %>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%} %>
    <p>
        <%=Html.ResourceActionLink<AdminController>("CreateDealer", ac=>ac.AddUpdateDealer(int.MinValue)) %>
    </p>
</asp:Content>
