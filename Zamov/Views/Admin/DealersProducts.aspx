<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.DealerBindProductPresent>>" %>
<%@ Import Namespace="Zamov.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $(".prodName").autocomplete('<%= Url.Action("FindProducts", "Admin") %>');

            $("#productsHeader input[type='checkbox']").click(function () {
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
    <h2>Расфасовка продуктов дилеров</h2>
    <br/>
    <% using (Html.BeginForm())
       { %>
    <table class="adminTable">
        <tr id="productsHeader">
            <th>
                ID
            </th>
            <th>
                <%= Html.ResourceString("Dealer")%>
            </th>
            <th>
                <%= Html.ResourceString("Product")%>
            </th>
            <th>
                Основной продукт
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
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%= Html.Encode(item.IdDealer) %>
                <%= Html.Hidden("dpId_" + item.IdDealerProduct) %>
            </td>
            <td>
                <%= Html.Encode(item.NameDealer) %>
            </td>
            <td>
                <%= Html.TextBox("name_" + item.IdDealerProduct, item.NameProduct, new { style = "width:250px;", @class = "prodName" })%>
            </td>
            <td>
                <%= Html.Encode(item.NameProductMain) %>
            </td>
            <td>
                <%= Html.CheckBox("bind_" + item.IdDealerProduct, new { @class = "bind" })%>
            </td>
            <td>
                <%= Html.CheckBox("create_" + item.IdDealerProduct, new { @class = "create" })%>
            </td>
        </tr>
        <% } %>
    </table>
    <input type="submit" name="save" value="Применить" />
    <%} %>
    <div class="pager">
        <%= Html.PageLinks((int)ViewData["CurrentPage"], (int)ViewData["TotalPages"], x => Url.Action("DealersProducts", new { page = x }))%>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
