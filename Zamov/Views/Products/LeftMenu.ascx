<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<CategoryPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>

    <% 
        int currentCategory = (int)ViewData["IdCurrentCategory"];
        int idCity = (int) ViewData["idCity"];
    %>
    <%-- Func<CategoryPresentation, int, string> selectedStyle = (category, id) =>
       {
           string result = "";
           if (category.Id == id || (category.Children.Where(c => c.Id == id).Count() > 0))
               result = " selected";
           return result;

       }; --%>
	   <% Func<CategoryPresentation, int, string> selectedStyle = (category, id) =>
       {
           string result = "";
           if (category.Id == id || (category.Children.Where(c => c.Id == id).Count() > 0))
               result = " active";
           return result;

       }; %>
    <div class="menu">
        <div class="menuHeader">
            <%= Html.ResourceString("Categories") %>
        </div>
        <div class="menuItems">
            <% foreach (var menuItem in Model)
               {
					string className = "menuItem";
					if (menuItem.Name.Length > 20)
					   className += " long";
            %>
			<%-- selectedStyle(menuItem, currentCategory) --%>
            <div class="<%= className %>">
                <% if (menuItem.Additional)
                   {%>
                        <%=Html.ActionLink(menuItem.Name, "ShowProductsCategory", "Products", new { idCity = idCity, id = menuItem.Id },
                                      new { @class = selectedStyle(menuItem, currentCategory) })%>
                <% }
                   else
                   { %>
                        <%=Html.ActionLink(menuItem.Name, "Index", "Dealers", new { id = menuItem.Id },
                                      new { @class = selectedStyle(menuItem, currentCategory) })%>
                <% } %>
            </div>
            <%if (menuItem.Id == currentCategory && menuItem.Children.Count > 0)
              {
                  foreach (var subItem in menuItem.Children)
                  {%>
			<%-- selectedStyle(subItem, currentCategory) --%>
            <div class="subMenuItem">
                <% if (subItem.Additional)
                   {%>
                        <%= Html.ActionLink(subItem.Name, "ShowProducts", "Products", new { idCity = idCity, id = subItem.Id }, null)%>
                <% }
                   else
                   { %>
                        <%= Html.ActionLink(subItem.Name, "Index", "Dealers", new { id = subItem.Id }, null)%>
                <% } %>
            </div>
            <%   }%>
            <%} %>
            <%} %>
        </div>
    </div>
