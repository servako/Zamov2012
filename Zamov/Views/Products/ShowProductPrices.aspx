<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Zamov.Models.ProductPricePresentation>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<asp:Content ContentPlaceHolderID="titleContent" runat="server"><%= Html.GetSeoTitle() %></asp:Content>
<asp:Content ContentPlaceHolderID="seoContent" runat="server"><%= Html.GetSeo() %></asp:Content>
<asp:Content ContentPlaceHolderID="productPhoto" runat="server">
    <% Html.RenderPartial("ProductPhoto", Model); %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBanner" runat="server">
    <% Html.RenderPartial("Banner");%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".subHeader").css({ 'display': 'block', 'height': '200px' });
            function highlightNo(arg) {
                var caller = arg;
                var number = caller.getCurrentID();
                if (number == 0) {
                    $("#previous").attr("disabled", "disabled");
                    $("#next").removeAttr("disabled");
                } else {
                    $("#next, #previous").removeAttr("disabled");
                }

                $("#counter li").removeClass("active");
                $("#counter li:eq(" + (number + 1) + ")").addClass("active");
            }
            mcarousel = $("#mycarousel").msCarousel({ boxClass: 'div.box', height: 190, width: 250, callback: highlightNo, autoSlide: 0 }).data("msCarousel");
            //add event on number
            $("#counter li:not('.buttons')").click(function () {
                if (mcarousel != undefined) {
                    var no = $(this).html();
                    mcarousel.goto(parseInt(no) - 1);
                }
            });
            $("#counter li, #next, #previous").mouseover(function () {
                if (mcarousel != undefined) {
                    mcarousel.pause();
                }
            });
            $("#counter li, #next, #previous").mouseout(function () {
                if (mcarousel != undefined) {
                    mcarousel.play();
                }
            });
            $("#next").click(function () {
                mcarousel.next();
            });
            $("#previous").click(function () {
                mcarousel.previous();
            });
            $("#next, #previous").attr("disabled", "disabled");
        })

        $(function () {
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

        string sortDealerId = (string)ViewData["sortDealerId"];
        int? groupId = null;
        if (ViewData["groupId"] != null)
            groupId = (int)ViewData["groupId"];

        int? productId = null;
        if (ViewData["productId"] != null)
            productId = (int)ViewData["productId"];

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
    <%--<% Html.RenderPartial("TopProducts"); %>--%>
    <%if (Model.Count > 0)
      { %>
    <%using (Html.BeginForm("AddToCartForProductAdditional", "Products", FormMethod.Post, new { id = "addToCart", style = "margin-bottom:20px;" }))
      { %>
    <%= Html.Hidden("groupId")%>
    <%= Html.Hidden("productId") %>
    <div style="text-align: right; margin-top: -20px;">
        <input type="submit" value="<%= Html.ResourceString("AddToCart") %>" />
    </div>
    <table class="blueHeaderedTable" style="width: 100%; margin: 5px 0;" cellpadding="0"
        cellspacing="0">
        <tr class="blueHeader">
            <th class="sortable" style="width: 150px;" align="center">
                <%= Html.SortHeader("Dealer", "/Products/ShowPrices/" + productId, "Dealer", "", "")%>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("DescriptionProduct", "/Products/ShowPrices/" + productId, "Name", "", "")%>
            </th>
            <th style="width: 20px;">
                <%= Html.ResourceString("MeassureUnit") %>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("Price", "/Products/ShowPrices/" + productId, "Price", "", "")%>
            </th>
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("Quantity") %>
            </th>
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("ToOrder") %>
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
                            <%
    Response.Write(string.Format("<img alt=\"\" src=\"/Image/DealerImage/{0}\"/>", item.DealerId));
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="noBorder">
                                <tr>
                                    <td valign="middle" align="left">
                                        <span class="colorBlue"><a class="productDescription" href="/Dealer/DealerInfo/<%= item.DealerId %>"
                                            target="_new">
                                            <%= Html.ResourceString("AboutDealer")%>
                                        </a></span>
                                    </td>
                                    <td style="width: 20px">
                                    </td>
                                    <td align="right">
                                        <span class="colorBlue"><a class="productDescription" href="/Dealer/DealerDeliveryDetails/<%= item.DealerId %>"
                                            target="_new">
                                            <%= Html.ResourceString("Delivery")%>
                                        </a></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td <%= classAttribute %> valign="top">
                <table class="noBorder" style="vertical-align: 100%;">
                    <tr>
                        <td align="left" valign="middle">
                            <a class="productDescription" href="<%= String.IsNullOrEmpty(item.Url) ? "/Content/img/noImage.png" : item.Url%>" target="_new">
                                <%= Html.Encode(item.dpName)%>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%= Html.Encode(item.dpDescription)%>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="bottom">
                            <span class="colorBlue">
                                <%= string.Format("{0}: ", Html.ResourceString("Availability")) %>
                            </span><span class="colorGrey">
                                <%= item.State %>
                            </span><span class="colorGreen">&nbsp;|&nbsp;</span><span class="colorBlue">
                                <%= string.Format("{0}: ", Html.ResourceString("Quarantee"))%>
                            </span><span class="colorGrey">
                                <%= item.Quarantee %>
                            </span><span class="colorGreen">&nbsp;|&nbsp;</span><span class="colorBlue">
                                <%= string.Format("{0}: ", Html.ResourceString("Date"))%>
                            </span><span class="colorGrey">
                                <%= item.DateBegin.ToShortDateString() %>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="center">
                <%= item.Unit %>
            </td>
            <td align="center">
                <span class="colorBlueMin">
                    <%= string.Format("{0}<br/>({1})", item.PriceHrn.ToString("#.00#"), Html.ResourceString("Hrn")) %>
                </span>
                <%if (item.CurId != 4) //если не грн. то выводим эту валюту
                  { %>
                <span class="colorGreenAvg">
                    <%= string.Format("<br/>{0}<br/>({1})", item.Price.ToString("#.00#"), item.CurSign) %>
                </span>
                <%} %>
            </td>
            <td align="center" valign="middle">
                <%
                  string mask = "9{4}";
                  // if (item.Unit == "кг." || item.Unit == "кг" || item.Unit == "л" || item.Unit == "л.")
                  //   mask = "99.99";
                %>
                <%= Html.TextBox("quantity_" + item.IdDealerProd, null, new { style = "width:24px; font-size:10px; text-align:center", onkeyup = "validateQuantity(this); order(this)" })%>
                <%--<%= Ajax.MaskEdit("", MaskTypes.Number, mask, false, true, "quantity_" + item.Id)%>--%>
            </td>
            <td align="center" valign="middle">
                <a id="orderLink_<%= item.IdDealerProd %>" class="orderCheckLink <%= SystemSettings.CurrentLanguage %>"
                    rel="<%= item.IdDealerProd %>"></a>
                <div style="overflow: hidden; width: 0; height: 0;">
                    <%= Html.CheckBox("order_" + item.IdDealerProd, false, new { onclick = "order(this)", style = "visibility:hidden; display: block; height: 0px; font-size: 0px;" })%>
                    <%= Html.Hidden("dealer_" + item.IdDealerProd, item.DealerId)%>
                </div>
            </td>
        </tr>
        <%   
}     
        %>
    </table>
    <div class="pager">
        <%= Html.PageLinks((int)ViewData["CurrentPage"], (int)ViewData["TotalPages"],
                                                  x => Url.Action("ShowProductPrices", 
                                                      new { idCity = SystemSettings.CityId, id = productId, page = x }))%>
    </div>
    <div>
        <input type="submit" style="float: right" value="<%= Html.ResourceString("AddToCart") %>" />
    </div>
    <%} %>
    <%} %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="bodyTop" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.msCarousel-min.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("dropshadow.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <%= Html.RegisterCss("~/Content/mscarousel.css")%>
    <%= Html.RegisterCss("~/Content/shadows.css")%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="leftMenu" runat="server">
    <% int idCurrentCategory = (int)ViewContext.HttpContext.Items["categoryId"];
       var categories = (List<CategoryPresentation>)ViewData["categories"];

       Html.RenderAction<ProductsController>(pc => pc.LeftMenu(SystemSettings.CityId, categories, idCurrentCategory));%>
</asp:Content>
