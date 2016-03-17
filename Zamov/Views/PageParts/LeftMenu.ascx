<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>
<div class="menu">
    <div class="menuHeader">
        <%= ViewData["caption"] %>
    </div>
    <div class="menuItems">
        <% foreach (var item in Model)
           {
               string url = item.Value;
               string text = item.Text;
               string selected = "";
               if (item.Selected)
                   selected = " selected";
               %>
            <div class="menuItem<%= selected %>">
                <a href="<%= url %>">
                    <%= text %>
                </a>
            </div>           
          <% } 
        %>
    </div>
    <div class="menuFooter">
    
    </div>
</div>

