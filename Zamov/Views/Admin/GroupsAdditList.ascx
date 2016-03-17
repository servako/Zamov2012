<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Zamov.Models.GroupsAdditional>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%
    int level = Convert.ToInt32(ViewData["level"]);
    string marginLeft = level * 20 + "px";
    var categories = (List<CategoryPresentation>)ViewData["categories"];
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
                    <%= Html.ResourceString("IndexNumberOutput")%>
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
                <%= Html.Hidden("itemId_" + item.grID, item.grID)%>
            </td>  
            <% if (level == 0){%>
            <td>
                <%= Html.HierarchicalDropDown("categoryId_" + item.grID, categories, ri=>ri.Children, ri=>ri.Name, ri=>ri.Id.ToString(), ri=>
                {
                    bool result = false;
                    if (item.CategoriesReference.EntityKey != null)
                    {
                        int entityKey = Convert.ToInt32(item.CategoriesReference.EntityKey.EntityKeyValues[0].Value);
                        result = ri.Id == entityKey;
                    }
                    return result;
                }, null)  %>
            </td>
            <%} %>
            <td>
                <%= Html.TextBox("uk-UA_" + item.grID, item.GetName("uk-UA", true), new { style="width:200px;" })%>
            </td>
            <td>
                <%= Html.TextBox("ru-RU_" + item.grID, item.GetName("ru-RU", true), new { style = "width:200px;" })%>
            </td>
            <td>
                <%= Html.TextBox("index_" + item.grID, item.gr_IndexNumber, new { style = "width:50px; font-size:10px; text-align:center", onkeyup = "validateQuantity(this)" })%>                              
            </td>
            <td align="center">
                <input type="checkbox" name="displayImages_<%= item.grID%>" <%= (item.gr_displayProductImages) ? "checked=\"checked\"" : "" %> />
            </td>
            <td align="center">
                <input type="checkbox" name="enabled_<%= item.grID %>" <%= (item.gr_enabled) ? "checked=\"checked\"" : "" %> />
            </td>
            <td>
                <a href="#" onclick="insertGroup(event, this, <%= item.grID %>)">
                    <%= Html.ResourceString("AddSubGroup") %>
                </a>
            </td>
            <td>
                <%= Html.ActionLink("SEO", "SeoSettings", new { id = item.grID, seoEntityType = (int)SeoEntityType.Groups })%>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteGroup", new { id = item.grID }, 
                    new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })%>
            </td>          
        </tr>
        </table>
                <% 
       if (item.GroupsAdditional1.Count > 0)
           Html.RenderAction<Zamov.Controllers.AdminController>(a => a.GroupsAdditList(item.grID, level + 1, categories));
       
       } %>