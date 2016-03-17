<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Zamov.Models.ProductPriceRangePresent>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="System.Globalization" %>
<script type="text/javascript">
    var lastCorrectQuantityValues = {};

    function validateCost(el) {
        var rExp = /[0-9]+(?:\,[0-9]*)?$/;
        if (!rExp.test(el.value)) {
            if (el.value.length > 0)
                el.value = (lastCorrectQuantityValues[el.id]) ? lastCorrectQuantityValues[el.id] : "";
        }
        else {
            lastCorrectQuantityValues[el.id] = el.value;
        }
    }
</script>
<div class="menu">
    <div class="menuHead">
        <div>
        <%=Html.ResourceString("Price")%>
        </div>
    </div>
    <div class="menuBody">
    <div style="text-align: center">
        <%=Html.ResourceString("RangePrice")%>
    </div>
    <%--    <%
        using (Html.BeginForm("ProductPriceRange", "Products", FormMethod.Post))
        { %>--%>
    <%
        if (Model.ListProductPriceRange.Count > 0)
        {
            for (int i = 0; i < Model.ListProductPriceRange.Count(); i++)
            {
    %>
    <table cellpadding="0" style="border-collapse: collapse; width: 100%">
        <tr>
            <td align="left" style="width: 15%">
                <%=Html.CheckBox(string.Format("ListProductPriceRange[{0}].IsCheck", i), Model.ListProductPriceRange[i].IsCheck)%>
                <%--<%=Html.Hidden(string.Format("brands[{0}].brandID", i), Model[i].brandID)%>--%>
            </td>
            <td align="left">
                <%=Html.Hidden(string.Format("ListProductPriceRange[{0}].PriceFrom", i), Model.ListProductPriceRange[i].PriceFrom)%>
                <%=Html.Hidden(string.Format("ListProductPriceRange[{0}].PriceTo", i), Model.ListProductPriceRange[i].PriceTo)%>
                <%=Html.Hidden(string.Format("ListProductPriceRange[{0}].Count", i), Model.ListProductPriceRange[i].Count)%>
                <span class="filterPrice">
                    <%=Html.Encode(string.Format("{0} {1} {2} {3} {4} ({5})",
                                                Html.ResourceString("from"), Model.ListProductPriceRange[i].PriceFrom.ToString("#.00#"),
                                                Html.ResourceString("to"), Model.ListProductPriceRange[i].PriceTo.ToString("#.00#"),
                                                Html.ResourceString("Hrn"), Model.ListProductPriceRange[i].Count))%>
                </span>
            </td>
        </tr>
    </table>
    <%
            }
        }
    %>
    <table style="width: 100%">
        <tr>
            <td align="left" style="width: 10%">
                &nbsp;
            </td>
            <td align="left">
                <span class="filterPrice">
                    <%=Html.ResourceString("from") %>
                    <%=Html.TextBox("CustomProductPriceRange.PriceFrom",
                        Model.CustomProductPriceRange.PriceFrom.ToString("#.##"), new { style = "width:45px;", @class = "filterPrice", onkeyup = "validateCost(this);" })%>
                    &nbsp;
                    <%=Html.ResourceString("to") %>
                    <%=Html.TextBox("CustomProductPriceRange.PriceTo",
                        Model.CustomProductPriceRange.PriceTo.ToString("#.##"), new { style = "width:45px;", @class = "filterPrice", onkeyup = "validateCost(this);" })%>
                </span>
            </td>
        </tr>
    </table>
    </div>
</div>
