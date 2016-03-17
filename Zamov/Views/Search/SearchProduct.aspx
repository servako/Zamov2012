<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.ProductSearchPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type="text/javascript">
    function AddToCartSuccess(response) {
        $get("totalPrice").innerHTML = response.get_response().get_object().TotalCartPrice;
        $get("orderItemsCount").innerHTML = response.get_response().get_object().TotalCartItems;
        $(".orderCb").each(function() { this.checked = false; })
        $(".orderQuantity").val("");
        $(".orderCheckLink").removeClass("ordered");
        $(".subHeader").css("display", "block");
        
        enableCart();
    }

    $(function() {
        $(".sortable a").click(function(e) {
            e.stopPropagation();
        }
            );

        $("a.productDescription").fancybox({ frameWidth: 700, frameHeight: 500, hideOnContentClick: false });

        $(".sortable")
            .mouseover(function() {
                //this.style.backgroundImage = "url(img/logo.jpg)";
                this.className = "thHover";
                //this.style.border= "1px solid lime";
            })
            .mouseout(function() {
                this.className = "";
            })
            .click(function() {
                location.href = this.firstChild.href;
            })

        $(".orderCheckLink").click(function() {
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

    $(function() {
    $(".productDescription").fancybox({ frameWidth: 700, frameHeight: 500, hideOnContentClick: false });
    })

       
</script>
    <%
        SortFieldNames? sortFieldName = null;
        SortDirection? sortDirection = null;
        SortDirection? invertSortDirection = null;

        if (ViewData["sortFieldName"] != null && ViewData["sortDirection"] != null)
        {
            sortFieldName = (SortFieldNames)ViewData["sortFieldName"];
            sortDirection = (SortDirection)ViewData["sortDirection"];

            if (sortDirection == SortDirection.Ascending)
                invertSortDirection = SortDirection.Descending;
            else
                invertSortDirection = SortDirection.Ascending;
        }
    %>
    
    <%
    using (Ajax.BeginForm("AddToCart", "Search", new AjaxOptions { HttpMethod = "POST", OnSuccess = "AddToCartSuccess" }, new { id = "addToCart" }))
    {%>
    <div style="text-align:right">
    <% if (Model == null || Model.Count() == 0){ %>
        
        <div style="text-align:center">
            <h2>
                Поиск не дал результатов
            </h2>
        </div>    
        
    <%} 
       else{   
        %>
    <input style="margin-bottom:5px" type="submit" value="<%=Html.ResourceString("AddToCart") %>" />  
        <table class="blueHeaderedTable" style="width: 100%; margin: 5px 0;" cellpadding="0"
        cellspacing="0">
        <tr class="blueHeader">
            <th style="width: 20px;" align="center">
                <%= Html.ResourceString("Photo") %>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("Name", "/Search/SearchProduct", "Name", "searchContext=" + ViewData["searchContext"], "")%>
            </th>
            <th class="sortable" align="center">
                <%= Html.SortHeader("Dealer", "/Search/SearchProduct", "Dealer", "searchContext=" + ViewData["searchContext"], "")%>
            </th>
            <th style="width: 20px;">
                <%= Html.ResourceString("MeassureUnit") %>
            </th>
            <th class="sortable"  align="center">
                <%= Html.SortHeader("PriceHrn", "/Search/SearchProduct", "Price", "searchContext=" + ViewData["searchContext"], "")%>
            </th>
            <th style="width: 20px;"  align="center">
                <%= Html.ResourceString("Quantity") %>
            </th>
            <th style="width: 20px;"  align="center">
                <%= Html.ResourceString("ToOrder") %>
            </th>
        </tr>
        <%
            int i = 0;
            foreach (var item in Model)
            {
                string rowClass = ((i % 2 > 0) ? "odd" : "even");
                i++; 
        %>
        <%= Html.Hidden("verProduct_" + item.Id, item.Version)%>
        <tr class="<%= rowClass %>">
            <td align="center">
                <table class="noBorder">
                    <tr>
                        <td style="height: 80px" valign="middle" align="center">
                            <% 
                                switch (item.Version)
                                {
                                    case (int)ProductVersion.First:
                                        ViewData["productId"] = item.Id;
                                        Html.RenderPartial("ProductImageThumbnail");
                                        break;
                                    case (int)ProductVersion.Additional: %>
                                        <%= Html.Image(item.Url, new { @class = "productImageSize" }) %>
                                        <%break;
                                }
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <a class="productDescription" href="/Products/Description/<%= item.Id %>/<%= item.Version %>">
                                <%= Html.ResourceString("Details") %>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" align="left">
                <a class="productDescription" href="/Products/Description/<%= item.Id %>">
                    <%= item.Name %><br />
                </a>
                <div class="productDescription true">
                    <%= item.Description %>
                </div>
            </td>
           <td align="left">
                <%= Html.Encode(item.DealerName)%>
                <%=Html.Hidden("dealer_"+ item.Id,item.DealerId)%>
            </td>
            <td align="center">
                <%= item.Unit %>
            </td>
            <td align="center">
                <%= item.Price.ToString("#.00#") %>
            </td>
            <td align="center" valign="middle">
                <%
                    string mask = "9{4}";
                    // if (item.Unit == "кг." || item.Unit == "кг" || item.Unit == "л" || item.Unit == "л.")
                    //   mask = "99.99";
                %>
                <%= Html.TextBox("quantity_" + item.Id, null, new { @class="orderQuantity", style = "width:24px; font-size:10px; text-align:center", onkeyup = "validateQuantity(this); order(this)" })%>
                <%--<%= Ajax.MaskEdit("", MaskTypes.Number, mask, false, true, "quantity_" + item.Id)%>--%>
            </td>
            <td align="center" valign="middle">
                <a id="orderLink_<%= item.Id %>" class="orderCheckLink <%= SystemSettings.CurrentLanguage %>" rel="<%= item.Id %>"></a>
                <div style="overflow:hidden; width:0; height:0;">
                    <%= Html.CheckBox("order_" + item.Id, false, new { @class = "orderCb", onclick = "order(this)", style = "visibility:hidden; display: block; height: 0px; font-size: 0px;" })%>
                </div>
            </td>
        </tr>
        <%   
            }     
        %>
    </table>
          
    <input style="margin-top:5px" type="submit" value="<%=Html.ResourceString("AddToCart") %>" />  
    <%} %>
</div>
    <%
        } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
    <style type="text/css">
        #leftSide{border:none !important;}
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
</asp:Content>

