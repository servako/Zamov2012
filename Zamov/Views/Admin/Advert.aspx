<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Advert>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    var enables = {};
    function updateEnables(check, id) {
        if (check.checked) {
            enables[id] = true;
        }
        else {
            enables[id] = false;
        }
    }

    function collectCategoryChanges() {
        var enablities = $get("enablities");
        enablities.value = Sys.Serialization.JavaScriptSerializer.serialize(enables);
        return true;
    }
</script>

    
    
    <h2>Реклама</h2>
    
    <%using(Html.BeginForm("UpdateAdvertList","Admin", FormMethod.Post))
{
  
 %>
  <%= Html.Hidden("enablities") %>
  <input type="submit" value="<%= Html.ResourceString("Save") %>" onclick="return collectCategoryChanges();" />   
    <table class="adminTable">
        <tr>
            
            <th> <%=Html.ResourceString("Category")%></th>
            <th>
                <%=Html.ResourceString("Location")%>
            </th>
            <th>
                <%=Html.ResourceString("ActiveM")%>
            </th>
            <th>
                Баннер
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td></td>
            <td>
                <%= Html.Encode(Helpers.GetPositionTitle((BannerPosition)item.Position)) %>
            </td>
            <td>
                <%=Html.CheckBox("cb_" + item.Id, item.IsActive, new { onblur = "updateEnables(this, " + item.Id + ")" })%>
            </td>
            <td>
                <%=Html.RegisterFlashScript(item.ImageSource, "b_" + item.Id, (BannerPosition)item.Position)%>
            </td>
            <td>
                <%= Html.ActionLink("Редактировать", "EditAdvert", new { id = item.Id })%>
            </td>
        </tr>
    
    <% }

       var categories = (IEnumerable<Category>) ViewData["categories"];
        
        foreach (var category in categories)
        {
          %>
          <tr>
           
            <td><%=category.Name %></td>
            <%if (category.Advert != null)
              {%>
            <td><%= Html.Encode(Helpers.GetPositionTitle((BannerPosition)category.Advert.Position))%></td>
            <td>
            <%=Html.CheckBox("cb_" + category.Advert.Id, category.Advert.IsActive, new { onblur = "updateEnables(this, " + category.Advert.Id + ")" })%>
             </td>
            <td><%=Html.RegisterFlashScript(category.Advert.ImageSource, "b_" + category.Advert.Id, (BannerPosition)category.Advert.Position)%></td>
            <%
                }
              else
            {%>
             <td colspan="3"></td>
            <%
            }%>
            <td>
                <%= Html.ActionLink("Редактировать", "EditAdvert", new { categoryId = category.Id, id = category.Advert != null ? (int?)category.Advert.Id : null })%>
            </td>
          </tr>
          <%
        }
    %>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" onclick="return collectCategoryChanges();" />
    <%} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="includes" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="bodyTop" runat="server">
 
</asp:Content>

