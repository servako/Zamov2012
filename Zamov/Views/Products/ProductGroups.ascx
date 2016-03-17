<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<GroupResentation>>" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>
<% 
    string dealerId = (string)ViewData["dealerId"];
    int groupToExpand = Convert.ToInt32(ViewData["groupToExpand"]);
    int groupId = (ViewData["groupId"] != null) ? Convert.ToInt32(ViewData["groupId"]) : int.MinValue;
    int categoryId = (int)ViewContext.RouteData.Values["categoryId"];
    string menuHeader = Html.ResourceString("Groups");

    string dealerGroupName = "";
    using (ZamovStorage context = new ZamovStorage())
    {
        dealerGroupName = (from dealer in context.Dealers
                           join groupName in context.Translations on dealer.Id equals groupName.ItemId
                           where groupName.Language == SystemSettings.CurrentLanguage && groupName.TranslationItemTypeId == (int)ItemTypes.GroupName
                           && dealer.Name == dealerId
                           select groupName.Text).FirstOrDefault();
    }
    if (!string.IsNullOrEmpty(dealerGroupName))
        menuHeader = dealerGroupName;
    
%>
<div class="menu">
    <div class="menuHeader">
        <%= menuHeader%>
    </div>
    <div class="menuItems">
        <% foreach (GroupResentation item in Model)
           {
               string className = "menuItem";
               if (item.Name.Length > 20)
                   className += " long";
               %>
        <div class="<%= className %>">
            <%
                int newCategoryId = item.CategoryId;
                if (newCategoryId == 0)
               {
                    GroupResentation groupResentation = item.Parent;
                    while (groupResentation!=null)
                    {
                        newCategoryId = groupResentation.CategoryId;
                        groupResentation = groupResentation.Parent;
                    }
                }
            %>
            <%= Html.ActionLink(item.Name, "Index", new { dealerId = dealerId, categoryId = newCategoryId, groupId = item.Id }, new { @class = (item.Id == groupId || item.Id == groupToExpand) ? "active" : string.Empty })%>
        </div>
        <%      if (item.Id == groupToExpand)
                {
                    List<GroupResentation> subGroups = item.Children;
                    if (subGroups.Count > 0)
                        Response.Write(
                            Html.TreeView("productGroups", subGroups, sg => sg.Children, sg =>
                            {
                                string format = "<div class=\"subMenuItem\">{0}</div>";
                                
                                return string.Format(
                                    format, 
                                    Html.ActionLink(
                                    sg.Name, 
                                    "Index", new 
                                    { 
                                        dealerId = sg.DealerName, 
                                        groupId = sg.Id 
                                    }, 
                                    new 
                                    { 
                                        @class = (sg.Id == groupId || sg.Id == groupToExpand) ? "active" : string.Empty 
                                    }));
                            })
                            );
                }
        %>
        <% } %>
    </div>
    <div class="menuFooter">
    </div>
    <script type="text/javascript">
        $(function() {
            window.setTimeout(expandCurrent, 1000);

        })

        function expandCurrent() {
            $("a.active").parent("div").prev("div.hitarea").click();
            $("a.active").parents("ul").prevAll("div.hitarea").click();
        }
    </script>
</div>