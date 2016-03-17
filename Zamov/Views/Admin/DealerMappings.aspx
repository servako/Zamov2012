<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Dealer>>" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>

    <%
        int id = Convert.ToInt32(ViewData["id"]);
        ItemTypes itemType = (ItemTypes)ViewData["itemType"];
        foreach (var item in Model)
        {
            bool enableCheck = false;
            switch(itemType)
            {
                case ItemTypes.City:
                    enableCheck = item.Cities.Where(c => c.Id == id).Count() > 0;
                    break;
                case ItemTypes.Category:
                    enableCheck = item.Categories.Where(c => c.Id == id).Count() > 0;
                    break;
            }
            
            Response.Write(
                Html.CheckBox("dealerId", enableCheck) +
                item.GetName(Html.CurrentCulture()) +
                "<br />"
            );
        }     
    %>
    
    <% using (Ajax.BeginForm("UpdateDealerMappingsMappings", new AjaxOptions { HttpMethod = "POST", OnSuccess = "closeDelaerMappings" })){ %>
        <input type="submit" value="<%= Html.ResourceString("Save") %>" />
        <input type="button" value="<%= Html.ResourceString("Cancel")  %>" onclick="closeDelaerMappings()" />
    <%} %>

