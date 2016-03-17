<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Zamov.Models.Group>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%
    int level = Convert.ToInt32(ViewData["level"]);
    string marginLeft = level * 20 + "px";
    int dealerId = Convert.ToInt32(ViewData["dealerId"]);
    List<CategoryPresentation> categories = (List<CategoryPresentation>)ViewData["categories"];
%>
    <% foreach (var item in Model) {
           item.LoadNames();
    %>
    <table class="adminTable" style="margin-left:<%= marginLeft %>">
        <%if (level == 0 && ViewData["firstDisplayed"]==null){
              ViewData["firstDisplayed"] = true;
              %>
        <tr>    
                <th>
                    <%= Html.ResourceString("Category") %>
                </th>
                <th>
                    Укр
                </th>
                <th>
                    Рус
                </th>
                <th>
                    <%= Html.ResourceString("Images") %>
                </th>
            <th>
                    <%= Html.ResourceString("ActiveF") %>
            </th>
            <th></th>
        </tr>
        <%} %>
        <tr class="groupLevel<%= level %>">
            <td style="display:none">
                <%= Html.Hidden("itemId_" + item.Id, item.Id)%>
            </td>  
            <% if (level == 0){%>
            <td>
                <%= Html.HierarchicalDropDown("categoryId_" + item.Id, categories, ri=>ri.Children, ri=>ri.Name, ri=>ri.Id.ToString(), ri=>
                {
                    bool result = false;
                    if (item.CategoryReference.EntityKey != null)
                    {
                        int entityKey = Convert.ToInt32(item.CategoryReference.EntityKey.EntityKeyValues[0].Value);
                        result = ri.Id == entityKey;
                    }
                    return result;
                }, null)  %>
            </td>
            <%} %>
            <td>
                <%= Html.TextBox("uk-UA_" + item.Id, item.GetName("uk-UA", false), new { style="width:200px;" })%>
            </td>
            <td>
                <%= Html.TextBox("ru-RU_" + item.Id, item.GetName("ru-RU", false), new { style = "width:200px;" })%>
            </td>
            <td align="center">
                <input type="checkbox" name="displayImages_<%= item.Id %>" <%= (item.DisplayProductImages) ? "checked=\"checked\"" : "" %>" />
            </td>
            <td align="center">
                <input type="checkbox" name="enabled_<%= item.Id %>" <%= (item.Enabled) ? "checked=\"checked\"" : "" %>" />
            </td>
            <td>
                <a href="#" onclick="insertGroup(event, this, <%= item.Id %>)">
                    <%= Html.ResourceString("AddSubGroup") %>
                </a>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteGroup", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>          
        </tr>
        </table>
                <% 
       if (item.Groups.Count > 0)
           Html.RenderAction<Zamov.Controllers.DealerCabinetController>(a => a.GoupList(dealerId, item.Id, level + 1, categories));
       
       } %>