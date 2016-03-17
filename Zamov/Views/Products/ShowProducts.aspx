<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.ProductByGroupPresent>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ContentPlaceHolderID="titleContent" runat="server"><%= Html.GetSeoTitle() %></asp:Content>
<asp:Content ContentPlaceHolderID="seoContent" runat="server"><%= Html.GetSeo() %></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBanner" runat="server">
    <% Html.RenderPartial("Banner");%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".menuHead").click(function () {
                $(this).parent('.menu').find('.menuBody').slideToggle(500);
                $(this).parent('.menu').toggleClass("menuHide");
                $(this).parent('.menu').find('.menuHead div').toggleClass("hide");
                return false;
            });

            $(".sortable a").click(function (e) {
                e.stopPropagation();
            }
            );

            $("a.productDescription").fancybox({ frameWidth: 700, frameHeight: 500, hideOnContentClick: false });

            $(".sortable")
            .mouseover(function () {
                //this.style.backgroundImage = "url(img/logo.jpg)";
                this.className = "thHover";
                //this.style.border= "1px solid lime";
            })
            .mouseout(function () {
                this.className = "";
            })
            .click(function () {
                location.href = this.firstChild.href;
            })

            $(".orderCheckLink").click(function () {
                Sys.UI.DomElement.toggleCssClass(this, "ordered");
                var check = $get("order_" + this.getAttribute("rel"));
                check.checked = !check.checked;
                order(check);
            }
            )
        })

        var items = {};
        function order(element) {
            var fieldSegments = element.name.split("_");

            var id = fieldSegments[1];

            if (element.type == "checkbox") {
                var input = $get("quantity_" + id);
                if (element.checked) {
                    if (input.value == "") {
                        input.value = 1;
                    }
                }
                else {
                    input.value = "";
                }
            }
            else {
                var checkbox = $get("order_" + id);
                if (!checkbox.checked) {
                    Sys.UI.DomElement.addCssClass($get("orderLink_" + id), "ordered");
                    checkbox.checked = true;
                }
            }
        }
        
    </script>
    <%
        SortFieldNames? sortFieldName = null;
        SortDirection? sortDirection = null;

        if (ViewData["sortFieldName"] != null && ViewData["sortDirection"] != null)
        {
            sortFieldName = (SortFieldNames)ViewData["sortFieldName"];
            sortDirection = (SortDirection)ViewData["sortDirection"];
        }

        bool onlyNewProductsForShowCategory = ViewData["onlyNewProductsForShowCategory"] != null ? (bool)ViewData["onlyNewProductsForShowCategory"] : false;

        string sortDealerId = (string)ViewData["sortDealerId"];
        int? groupId = null;
        if (ViewData["groupId"] != null)
            groupId = (int)ViewData["groupId"];

        int? categoryId = null;
        if (ViewData["categoryId"] != null) categoryId = (int)ViewData["categoryId"];

        Func<string, string> processDescription = s =>
            {
                string result = s;
                if (s.Length > 240)
                {
                    result = s.Substring(0, 240);
                    result = result.Substring(0, result.LastIndexOf(' '));
                    result += "...";
                }
                return result;
            };
    %>
    <%if (Model.Count > 0)
      { %>
    <span style="color: #324D62"><b>
        <%= onlyNewProductsForShowCategory ? Html.ResourceString("Novelty") : Html.ResourceString("ListOfProposals")%>
    </b></span>
    <table class="blueHeaderedTable" style="width: 100%; margin: 5px 0;" cellpadding="0"
        cellspacing="0">
        <tr class="blueHeader">
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("Photo") %>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("Name", onlyNewProductsForShowCategory ? 
                    "/Products/ShowCategory/" + (int)ViewData["IdCurrentCategory"] : string.Format("/Products/Show/{0}/{1}", SystemSettings.CityId ,groupId), "Name", "", "")%>
            </th>
            <th style="width: 20px;">
                <%= Html.ResourceString("MeassureUnit") %>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("PriceHrn", onlyNewProductsForShowCategory ?
                    "/Products/ShowCategory/" + (int)ViewData["IdCurrentCategory"] : string.Format("/Products/Show/{0}/{1}", SystemSettings.CityId, groupId), "Price", "", "")%>
            </th>
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("Proposals") %>
            </th>
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("Choice") %>
            </th>
        </tr>
        <%
int i = 0;
foreach (var item in Model)
{
    string trClass = "";
    if (item.Action && item.New)
        trClass = "actionNew";
    else if (item.Action)
        trClass = "action";
    else if (item.New)
        trClass = "new";
    string classAttribute = (!string.IsNullOrEmpty(trClass)) ? "class=\"" + trClass + "\"" : "";
    string rowClass = ((i % 2 > 0) ? "odd" : "even");
    i++; 
        %>
        <tr class="<%= rowClass %>">
            <td align="center">
                <table class="noBorder">
                    <tr>
                        <td style="height: 80px" valign="middle" align="center">
                            <%= Html.Image(String.IsNullOrEmpty(item.Url) ? "/Content/img/noImage.png" : item.Url, new { @class = "productImageSize" })%>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a class="productDescription" href="/Products/Description/<%= item.Id %>/<%= (int)ProductVersion.Additional %>">
                                <%= Html.ResourceString("Details") %>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
            <td <%= classAttribute %> valign="top">
                <a class="productDescription" href="/Products/Description/<%= item.Id %>/<%= (int)ProductVersion.Additional %>">
                    <%= item.Name %><br />
                </a>
                <div class="productDescription ">
                    <%= processDescription(item.Description) %>
                </div>
            </td>
            <td align="center">
                <%= item.Unit %>
            </td>
            <td align="center">
                <span class="colorBlueMin">
                    <%= Html.ResourceString("Min") %>
                    <br />
                    <%=string.Format("{0} {1}", (item.PriceMin).ToString("#.00#"), Html.ResourceString("Hrn"))%>
                </span>
                <br />
                <span class="colorGreenAvg">
                    <%= Html.ResourceString("Avg") %>
                    <br />
                    <%=string.Format("{0} {1}", (item.PriceAvg).ToString("#.00#"), Html.ResourceString("Hrn"))%>
                </span>
                <br />
                <span style="color: #b62727">
                    <%= Html.ResourceString("Max") %>
                    <br />
                    <b>
                        <%=string.Format("{0} {1}", (item.PriceMax).ToString("#.00#"), Html.ResourceString("Hrn"))%></b>
                </span>
            </td>
            <td align="center" valign="middle">
                <%= Html.ActionLink(item.Proposals.ToString(), "ShowProductPrices", "Products", new { idCity = SystemSettings.CityId, id = item.Id }, null)%>
            </td>
            <td align="center" valign="middle">
                <%= Html.ResourceActionLink("WhereToOrder", "ShowProductPrices", "Products", new { idCity = SystemSettings.CityId, id = item.Id }, null)%>
            </td>
        </tr>
        <%   
            }     
        %>
    </table>
    <div class="pager">
        <%= Html.PageLinks((int)ViewData["CurrentPage"], (int)ViewData["TotalPages"], 
            x => Url.Action(onlyNewProductsForShowCategory ? "ShowProductsCategory" : "ShowProducts",
                          new { idCity = SystemSettings.CityId, id = onlyNewProductsForShowCategory ? (int)ViewData["IdCurrentCategory"] : groupId, page = x }))%>
    </div>
    <%} %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.treeview.js") %>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterJS("dropshadow.js")%>
    <%= Html.RegisterCss("~/Content/jquery.treeview.css") %>
    <%= Html.RegisterCss("~/Content/GroupsTreeview.css") %>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterCss("~/Content/shadows.css")%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="leftMenu" runat="server">
    <%  bool onlyNewProductsForShowCategory = ViewData["onlyNewProductsForShowCategory"] != null ? (bool)ViewData["onlyNewProductsForShowCategory"] : false;
        if (!onlyNewProductsForShowCategory)
        {
            Html.RenderAction<ProductsController>(c => c.ProductGroupsNew(SystemSettings.CityId, (int?)ViewData["groupId"]));

            using (Html.BeginForm("Filters", "Products", FormMethod.Post, new { idCity = SystemSettings.CityId }))
            {%>
    <div style="text-align: center; color: #324D62;">
        <%= Html.ResourceString("Filters")%></div>
    <%
                List<ProductByGroupPresent> productDistinct = (List<ProductByGroupPresent>)ViewData["productDistinct"];
                Html.RenderAction<ProductsController>(ac => ac.Brands((int?)ViewData["groupId"], productDistinct));
                Html.RenderAction<ProductsController>(pr => pr.ProductPriceRange(SystemSettings.CityId, (int?)ViewData["groupId"], productDistinct));
    %>
    <%
             if (Session["FilterProductPriceRange"] != null || Session["FilterBrands"] != null)
             {%>
    <input type="submit" name="Clear" value="<%=Html.ResourceString("ResetFilters")%>" />
    <br />
    <%
        }%>
    <input type="submit" name="Apply" value="<%=Html.ResourceString("Apply")%>" />
    <%
         }
     }
     else
     {
         int idCurrentCategory = (int)ViewData["IdCurrentCategory"];
         var categories = (List<CategoryPresentation>)ViewData["categories"];

         Html.RenderAction<ProductsController>(pc => pc.LeftMenu(SystemSettings.CityId, categories, idCurrentCategory));
     }%>
</asp:Content>
