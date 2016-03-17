<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.GroupsAdditional>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function insertGroup(evt, link, parentId) {
            var y = 0;

            if (window.event) {
                y = window.event.clientY;
            }
            else {
                y = evt.clientY;
            }
            var pos = $(link).offset();
            if (parentId >= 0) {
                $("#parentId").val(parentId);
                //pos.left = pos.left - $("#insertGroup").width();
                $(".categoryCol").hide();
            }
            else {
                pos.left = pos.left + $(link).width();
                $(".categoryCol").show();
            }
            $("#insertGroup").css("top", pos.top).css("left", pos.left).slideDown("fast");
            window.setTimeout(bindBodyClick, 50);
        }

        function bindBodyClick() {
            $("body").click(bodyClick);
        }

        function bodyClick() {
            $("#insertGroup").slideUp("fast");
            $("body").unbind("click", bodyClick);
        }

        $(function () {

            $("#insertGroup").appendTo($(document.body)).click(function (e) { e.stopPropagation(); if (window.event) { window.event.cancelBubbling = true; } });
        }
        );
    </script>
    <%
        var categories = (List<CategoryPresentation>)ViewData["categories"];
    %>
    <h2>
        <%= Html.ResourceString("Groups") %></h2>
    <% 
        using (Html.BeginForm("UpdateGroups", "Admin", FormMethod.Post))
        {
            Html.RenderAction<Zamov.Controllers.AdminController>(ac => ac.GroupsAdditList(null, 0, categories));
            Response.Write("<input type=\"submit\" value=\"" + Html.ResourceString("Save") + "\" /> ");
        }
    %>
    <a style="text-decoration: underline; cursor: pointer" onclick="insertGroup(event, this, <%= int.MinValue %>)">
        <%= Html.ResourceString("AddGroup") %>
    </a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
    <%
        var categories = (List<CategoryPresentation>)ViewData["categories"];
    %>
    <div id="insertGroup" class="greyBorderBox popUpBox">
        <% using (Html.BeginForm("InsertGroup", "Admin"))
           { %>
        <table class="adminTable">
            <tr>
                <th class="categoryCol">
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
            </tr>
            <tr>
                <td class="categoryCol">
                    <%= Html.HierarchicalDropDown("categoryId", categories, ri => ri.Children, ri => ri.Name, ri => ri.Id.ToString(), null, null)%>
                </td>
                <td>
                    <%= Html.TextBox("groupUkrName")%>
                </td>
                <td>
                    <%= Html.TextBox("groupRusName")%>
                </td>
                <td>
                    <%= Html.TextBox("groupIndex", 1, new { style = "width:50px; font-size:10px; text-align:center", onkeyup = "validateQuantity(this)" })%>                              
                </td>
                <td>
                    <%= Html.CheckBox("displayImages", true) %>
                </td>
                <td align="center">
                    <%= Html.CheckBox("enabled", true) %>
                </td>
            </tr>
        </table>
        <%= Html.Hidden("parentId", int.MinValue) %>
        <input type="submit" value="<%= Html.ResourceString("Add") %>" />
        <%} %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
