<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.DealerBindGroupPresent>>" %>

<%@ Import Namespace="Zamov.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $(".groupName").autocomplete('<%= Url.Action("FindGroups", "Admin") %>');

            $("#groupsHeader input[type='checkbox']").click(function () {
                $("." + this.id).attr("checked", this.checked);
                if (this.id == "create" && this.checked) {
                    $(".bind, #bind").attr("checked", !this.checked);
                }
                else if (this.id == "bind" && this.checked) {
                    $(".create, #create").attr("checked", !this.checked);
                }
            });

            $(".bind, .create").change(chbox);

            function chbox() {
                var idChekbox = this.id.split("_");
                var name = idChekbox[0];
                var id = idChekbox[1];
                if (name == "bind" && this.checked) {
                    $("#create_" + id).attr("checked", !this.checked);
                }
                if (name == "create" && this.checked) {
                    $("#bind_" + id).attr("checked", !this.checked);
                }
            };
        });
    </script>

    <br/>
    <h2>
        Расфасовка групп дилеров</h2>
    <br/>
    <% using (Html.BeginForm())
       { %>
    <table class="adminTable">
        <tr id="groupsHeader">
            <th>
                ID
            </th>
            <th>
                <%= Html.ResourceString("Dealer")%>
            </th>
            <th>
                <%= Html.ResourceString("Group")%>
            </th>
            <th>
                Основная группа
            </th>
            <th>
                Категория
            </th>
            <th>
                <input type="checkbox" id="bind" />
                Привязать
            </th>
            <th>
                <input type="checkbox" id="create" />
                Создать
            </th>
        </tr>
        <% SelectList listCategories = (SelectList)ViewData["Categories"];
            foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%= Html.Encode(item.IdDealer) %>
                <%= Html.Hidden("dpId_" + item.IdDealerGroup) %>
            </td>
            <td>
                <%= Html.Encode(item.NameDealer) %>
            </td>
            <td>
                <%= Html.TextBox("name_" + item.IdDealerGroup, item.NameGroup, new { style = "width:200px;", @class = "groupName" })%>
            </td>
            <td style="max-width:150px;">
                <%= Html.Encode(item.NameGroupMain) %>
            </td>
            <td>
                <%= Html.DropDownList("category_" + item.IdDealerGroup, listCategories, new { style = "width:200px;" })%>
            </td>
            <td>
                <%= Html.CheckBox("bind_" + item.IdDealerGroup, new { @class = "bind" })%>
            </td>
            <td>
                <%= Html.CheckBox("create_" + item.IdDealerGroup, new { @class = "create" })%>
            </td>
        </tr>
        <% } %>
    </table>
    <input type="submit" name="save" value="Применить" />
    <%} %>
    <br/>
    <%= Html.ActionLink("Дальше", "DealersProducts")%>
    <div class="pager">
        <%= Html.PageLinks((int)ViewData["CurrentPage"], (int)ViewData["TotalPages"], x => Url.Action("DealersGroups", new { page = x }))%>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
