<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Zamov.Models.Category>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%
    int level = Convert.ToInt32(ViewData["level"]);
    string marginLeft = level * 20 + "px";
%>
    <% foreach (var item in Model) {
           item.LoadNames();
    %>
    <table class="adminTable" style="margin-left:<%= marginLeft %>">
        <%if (level == 0 && ViewData["firstDisplayed"]==null){
              ViewData["firstDisplayed"] = true;
              %>
        <tr>
            <th style="display:none;">
                Id
            </th>
            <th>
                <%= Html.ResourceString("Ukr") %>
            </th>
            <th>
                <%= Html.ResourceString("Rus") %>
            </th>
            <th align="center" style="width:85px;">
                <%= Html.ResourceString("Image") %>
            </th>
            <th>
                <%= Html.ResourceString("SubCategoryText") %>
            </th>
            <th>
                <%= Html.ResourceString("SubCategoryText") %>
            </th>
            <th align="center">
                <%= Html.ResourceString("Show") %>
            </th>
            <th>
                <%= Html.ResourceString("IndexNumberOutput")%>
            </th>
            <th></th>
            <th></th>
            <th></th>
        </tr>
        <%} %>
        <tr>
            <td style="display:none">
                <%= Html.Hidden("itemId_" + item.Id, item.Id)%>
            </td>
            <td>
                <%= Html.TextBox("uk-UA_" + item.Id, item.GetName("uk-UA", false), new { style="width:120px;" })%>
            </td>
            <td>
                <%= Html.TextBox("ru-RU_" + item.Id, item.GetName("ru-RU", false), new { style = "width:120px;" })%>
            </td>
            <td align="center" style="width:85px;">
                <a href="javascript:openImageIframe(<%= item.Id %>)" style="text-decoration:none;">
                    <%= Html.Image("~/Content/img/productImage.jpg", new {style="border:none" })%>
                </a>
            </td>
            <td>
                <%= Html.TextBox("subuk-UA_" + item.Id, item.GetSubCategoryName("uk-UA"), new { style = "width:120px;" })%>
            </td>
            <td>
                <%= Html.TextBox("subru-RU_" + item.Id, item.GetSubCategoryName("ru-RU"), new { style = "width:120px;" })%>
            </td>
            <td align="center" style="width:75px;">
                <%= Html.CheckBox("Enabled_" + item.Id, item.Enabled, new { onClick = "updateEnables(this, " + item.Id + ")" })%>
            </td>
            <td>
                <%= Html.TextBox("index_" + item.Id, item.IndexNumber, new { style = "width:50px; font-size:10px; text-align:center", onblur = "tableChanged(update, this)", onkeyup = "validateQuantity(this)"})%>                              
            </td>
            <td>
                <a href="/Admin/CategoryDescription/<%= item.Id %>" class="cDescription"><%= Html.ResourceString("Description") %></a>
            </td>
            <td>
                <a href="#" onclick="insertCategory(this, <%= item.Id %>)">
                    <%= Html.ResourceString("AddSubCategory") %>
                </a>
            </td>
            <td>
                <%= Html.ActionLink("SEO", "SeoSettings", new { id = item.Id, seoEntityType = (int)SeoEntityType.Categories })%>
            </td>
            <td>
                <%= Html.ActionLink(Html.ResourceString("Delete"), "DeleteCategory", new { id = item.Id }, new { onclick = "return confirm('" + Html.ResourceString("DeleteCategoryConfirmation") + "')" })%>
            </td>
        </tr>
    


    </table>
    
        <% 
       if (item.Categories.Count > 0)
           Html.RenderAction<Zamov.Controllers.AdminController>(a => a.CategoriesList(item.Id, level + 1));
       
       } %>

