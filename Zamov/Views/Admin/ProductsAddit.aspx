<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.ProductsAdditional>>" %>

<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm("ProductsAddit", "Admin", FormMethod.Post))
       {%>
    <br />
    	<%-- Html.DropDownList("idGroup", (List<SelectListItem>) ViewData["groups"], new {onchange = "groupSelected()"})--%>
    <%= Html.HierarchicalDropDown("idGroup", (List<GroupResentation>)ViewData["groups"], g => g.Children, g => g.Name, g => g.Id.ToString(), null, new { onchange = "groupSelected()" }, true)%>
    <%
           if (Model.Count > 0)
           {
    %>
    <table>
        <tr>
            <th>
                <%=Html.ResourceString("Product")%>
            </th>
            <th>
                <%=Html.ResourceString("UnitShort")%>
            </th>
            <th>
                <%=Html.ResourceString("UrlPhotoProduct")%>
            </th>
            <th>
                <%=Html.ResourceString("Description")%>
            </th>
            <th>
                <%=Html.ResourceString("Enabled")%>
            </th>
            <th>
                <%=Html.ResourceString("New")%>
            </th>
            <th>
                <%=Html.ResourceString("ActiveM")%>
            </th>
            <th>
                <%=Html.ResourceString("Top")%>
            </th>
            <th>
                <%=Html.ResourceString("Deleted")%>
            </th>
            <th>
            </th>
        </tr>
        <%
               for (int i = 0; i < Model.Count; i++)
               {%>
        <tr>
            <td>
                <%=Html.Hidden(string.Format("prodAddit[{0}].p_prodID", i), Model[i].p_prodID)%>
                <%=Html.TextBox(string.Format("prodAddit[{0}].p_prodName", i), Model[i].p_prodName, new { style = "width:250px;" })%>
            </td>
            <td>
                <%=Html.TextBox(string.Format("prodAddit[{0}].p_unit", i), Model[i].p_unit, new { style = "width:50px;" })%>
            </td>
            <td>
                <%=Html.TextBox(string.Format("prodAddit[{0}].p_photo_url", i), Model[i].p_photo_url, new { style = "width:150px;" })%>
            </td>
            <td>
                <%=Html.TextArea(string.Format("prodAddit[{0}].p_descr", i), Model[i].p_descr, new { style = "width:250px;" })%>
            </td>
            <td>
                <%=Html.CheckBox(string.Format("prodAddit[{0}].p_enabled", i), Model[i].p_enabled)%>
            </td>
            <td>
                <%=Html.CheckBox(string.Format("prodAddit[{0}].p_new", i), Model[i].p_new)%>
            </td>
            <td>
                <%=Html.CheckBox(string.Format("prodAddit[{0}].p_action", i), Model[i].p_action)%>
            </td>
            <td>
                <%=Html.CheckBox(string.Format("prodAddit[{0}].p_top", i), Model[i].p_top)%>
            </td>
            <td>
                <%=Html.CheckBox(string.Format("prodAddit[{0}].p_deleted", i), Model[i].p_deleted)%>
            </td>
        </tr>
        <%
               }%>
    </table>
    <input type="submit" value="<%= Html.ResourceString("Save") %>" />
    <%
           }
       }%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <script type="text/javascript">
        function groupSelected() {
            $("#idGroup option").each(function(i) {
                if (this.selected)
                    location.href = "/Admin/Products/" + this.value;
            }
            );
        }
    </script>
</asp:Content>
