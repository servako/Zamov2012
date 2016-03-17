<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Zamov.Models.ProductPricePresentation>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Controllers" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<script type="text/javascript">
    $(function () {
        $("a.productDescription").fancybox({ frameWidth: 700, frameHeight: 500, hideOnContentClick: false });
        $(".feedback").fancybox({ frameWidth: 700, hideOnContentClick: false });
    });
</script>
<%
    if (Model.Count > 0)
    { %>
<table>
    <tr>
        <td valign="top">
            <div id="prodPhotoLeft" style="display: block; float: left; width: 300px; height: 200px;">
                <%

        decimal priceMin = Model.Min(p => p.PriceHrn);
        decimal priceMax = Model.Max(p => p.PriceHrn);
        decimal priceAvg = Model.Average(p => p.PriceHrn);
        int count = Model.Count();

        int productId = (int)ViewData["productId"];
        int feedbackCount;
        using (var context = new ZamovStorage())
        {
            feedbackCount = context.ProductsAdditionalFeedback.Where(f => f.ProductId == productId).Count();
        }
        
                %>
                <span style="font-weight: bold; color: #142f4a; font-size: x-small;">
                    <%=string.Format("{0}", Html.ResourceString("YouHaveChosen").ToUpperInvariant())%>
                </span>
                <br />
                <span style="font-weight: bold; color: #265b7d">
                    <%=string.Format("{0}", Model.FirstOrDefault().Name.ToUpperInvariant())%>
                </span>
                <br />
                <br />
                <%=Html.ActionLink(string.Format("{0}: {1}", Html.ResourceString("Feedbacks"), feedbackCount), 
                    "FeedbackProductsAdditional", "Feedback", new { prodId = productId }, new { @class = "feedback" })%>
                <br />
                <br />
                <a class="productDescription" href="/Products/Description/<%= productId %>/<%= (int)ProductVersion.Additional %>">
                    <%= Html.ResourceString("DescriptionProduct")%>
                </a>
                <br />
                <br />
                <span style="padding-top: 5px; display: inline-block; margin-top: 5px; font-size: x-small">
                    <%=string.Format("{0}:", Html.ResourceString("RangePrice"))%>
                    <%=string.Format("<b>{0}-{1} {2}</b>", priceMin.ToString("#.00#"), priceMax.ToString("#.00#"), Html.ResourceString("Hrn"))%>
                </span>
                <br />
                <span style="padding-top: 5px; display: inline-block; margin-top: 5px; font-size: x-small">
                    <%=string.Format("{0}:", Html.ResourceString("AvrCost"))%>
                    <%=string.Format("<b>{0} {1}</b>", priceAvg.ToString("#.00#"), Html.ResourceString("Hrn"))%>
                </span>
                <br />
                <span style="padding-top: 5px; display: inline-block;">
                    <%=string.Format("{0}:", Html.ResourceString("ProposalsChoiceProduct"))%>
                    <%=string.Format("<b>{0}</b>", count)%>
                </span>
            </div>
        </td>
        <td>
            <div id="mycarousel">
                <%
int countItems = 0;
foreach (var item in Model)
{
    countItems++;
                %>
                <div class="box">
                    <%=Html.Image(String.IsNullOrEmpty(item.Url) ? "~/Content/img/noImage.png" : item.Url,  new { style = "max-height:150px;max-width:250px;" })%>
                </div>
                <%
}%>
            </div>
            <%--<div class="coutnerRow">
                <ul id="counter">
                    <li class="buttons">
                        <span id="previous">
                            <a href="#" class="previous"></a>
                        </span>
                    </li>
                    <%
        for (int i = 1; i < countItems + 1; i++)
        {
                    %>
                    <li>
                        <%= i.ToString()%></li>
                    <% } %>
                    <li class="buttons">
                        <span id="next">
                            <a href="#" class="next"></a>
                        </span>
                    </li>
                </ul>
            </div>--%>
        </td>
        <td valign="top">
            <div id="prodPhotoRight" style="display: block; float: right; width: 50px; height: 200px;">
            </div>
        </td>
    </tr>
</table>
<%} %>
<style type="text/css">
    a.previous, a.next
    {
        display: inline-block;
        width: 22px;
        height: 22px;
    }
    
    a.previous
    {
        background: transparent url(img/bLeft.png) 0 0 no-repeat;
    }
    
    a.next
    {
        background: transparent url(img/bRight.png) 0 0 no-repeat;
    }
    
    a.previous:hover, a.next:hover
    {
        background-position: 0 -22px;
    }
    
    a.button
    {
        display: block;
        background-color: transparent;
        background-image: url(cssButtonsFiles/buttonBackground.gif);
        background-repeat: no-repeat;
        width: 132px;
        height: 28px;
        margin: 5px auto;
        padding: 5px 0 0 0;
        text-align: center;
        font-size: 100%;
        font-weight: bold;
        text-decoration: none;
        font-family: Helvetica, Calibri, Arial, sans-serif;
    }
    a.button:link, a.button:visited
    {
        color: #002577;
    }
    a.button:hover, a.button:active
    {
        background-position: 0 -36px;
        color: #FF7200;
    }
    .icon
    {
        background-repeat: no-repeat;
        padding: 0 0 5px 18px;
    }
    a.button:hover .icon, a.button:active .icon
    {
        background-position: 0 -28px;
    }
    /* list of button icons */#buttonOK .icon
    {
        background-image: url(cssButtonsFiles/ok.gif);
    }
    #buttonCancel .icon
    {
        background-image: url(cssButtonsFiles/cancel.gif);
    }
    #buttonImport .icon
    {
        background-image: url(cssButtonsFiles/import.gif);
    }
    .hand
    {
        cursor: pointer;
    }
    .box
    {
        background: #fff;
        padding: 5px;
        height: 190px;
        width: 350px;
        margin: 0 5px 0 0;
        float: left;
    }
    .box h1
    {
        color: #fff;
        background: #000;
    }
    .coutnerRow
    {
        clear: both;
        padding: 5px;
    }
    .coutnerRow ul, .coutnerRow ul li
    {
        list-style: none;
        list-style-image: none;
        margin: 0;
        padding: 0;
    }
    .coutnerRow ul li
    {
        margin-right: 10px;
        display: inline;
        cursor: pointer;
        padding: 5px;
        color: #80858d;
        background-color: #fff;
    }
    .coutnerRow ul li.active
    {
        margin-right: 10px;
        display: inline;
        cursor: pointer;
        padding: 5px;
        color: #324D62;
        background-color: #fff;
    }
    #next, #previous
    {
        text-align: center;
    }
    .mstoplinks
    {
        padding: 3px;
        border-bottom: 2px solid #c3c3c3;
    }
    .mstoplinks a, .mstoplinks a:visited
    {
        color: #003366;
        text-decoration: none;
        border-right: 1px solid #c3c3c3;
        padding: 0 10px;
    }
    .mstoplinks a.active, .mstoplinks a.active:visited
    {
        color: #003366;
        text-decoration: none;
        border-right: 1px solid #c3c3c3;
        padding: 0 10px;
        border-bottom: 1px solid #c3c3c3;
        border-left: 1px solid #c3c3c3;
    }
</style>
