<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Admin/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.DealerBindBrandPresent>>" %>

<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $(".brandName").autocomplete('<%= Url.Action("FindBrands", "Admin") %>');

            $("#brandsHeader input[type='checkbox']").click(function () {
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
        Расфасовка брэндов дилеров</h2>
    <br/>
    <% using (Html.BeginForm())
       { %>
    <table class="adminTable">
        <tr id="brandsHeader">
            <th>
                ID
            </th>
            <th>
                <%= Html.ResourceString("Dealer")%>
            </th>
            <th>
                <%= Html.ResourceString("Brand")%>
            </th>
            <th>
                <%= Html.ResourceString("MainBrand") %>
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
                <%= Html.Hidden("dbId_" + item.IdDealerBrand)%>
            </td>
            <td>
                <%= Html.Encode(item.NameDealer) %>
            </td>
            <td>
                <%= Html.TextBox("name_" + item.IdDealerBrand, item.NameBrand, new { style = "width:150px;", @class = "brandName" })%>
            </td>
            <td>
                <%= Html.Encode(item.NameBrandMain) %>
            </td>
            <td>
                <%= Html.CheckBox("bind_" + item.IdDealerBrand, new { @class = "bind" })%>
            </td>
            <td>
                <%= Html.CheckBox("create_" + item.IdDealerBrand, new { @class = "create" })%>
            </td>
        </tr>
        <% } %>
    </table>
    <input type="submit" name="save" value="Применить" />
    <%} %>
    <br/>
    <%= Html.ActionLink("Дальше", "DealersGroups")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
</asp:Content>
