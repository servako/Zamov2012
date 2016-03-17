<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DealerPresentation>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Helpers" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function() {
            $(".deliveryInfo span a")
            .fancybox(
            {
                frameWidth: 700,
                hideOnContentClick: false
            });

            $(".userFeedback span a")
            .fancybox(
            {
                frameWidth: 700,
                hideOnContentClick: false
            });
        })
    </script>
    <div class="subHeader">
        <% Html.RenderPartial("CurrentDealer"); %>
        <div class="deliveryInfo">
            <span class="first">
                <%= Html.ActionLink(Html.ResourceString("DeliveryInfo"), "DeliveryInfo", new { id = Model.Id }, new { @class = "deliveryInfoLinkFirst" })%>
            </span>
            <br />
            <span>
                <%= Html.ActionLink(Html.ResourceString("Delivery"), "DeliveryInfo", new { id = Model.Id }, new { @class = "deliveryInfoLinkSecond" })%>
            </span>
        </div>
        <div class="userFeedback">
            <span class="first">
                <%= Html.ActionLink(Html.ResourceString("Feedback"), "Feedback", new { id = Model.Id }, new { @class = "dealerFeedbackLinkFirst" })%>
            </span>
            <br />
            <span>
                <%= Html.ActionLink(Html.ResourceString("OfTheSiteVisitors"), "Feedback", new { id = Model.Id }, new { @class = "dealerFeedbacLinkSecond" })%>
            </span>

        </div>
    </div>
    <div class="dealerDescription">
        <div class="descriptionHeader">
            <%= Html.ResourceString("DealerInformation")%>
        </div>
        <%= Model.Description %>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="includes" runat="server">
    <%= Html.RegisterCss("~/Content/shadows.css")%>
    <%= Html.RegisterJS("dropshadow.js")%>
    <%= Html.RegisterJS("jquery.easing.js")%>
    <%= Html.RegisterJS("jquery.fancybox.js")%>
    <%= Html.RegisterCss("~/Content/fancy/jquery.fancybox.css")%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="leftMenu" runat="server">
<%
    using(ZamovStorage context = new ZamovStorage())
    {
        string modelId = Model.Id.ToString();

        var items = (from g in context.Groups.Include("Parent")
                     join translation in context.Translations on g.Id equals translation.ItemId
                     where translation.Language == Zamov.Controllers.SystemSettings.CurrentLanguage
                     && translation.TranslationItemTypeId == (int)ItemTypes.Group
                     && g.Enabled
                     && g.Parent == null
                     select new GroupResentation { Name = translation.Text, Id = g.Id }
                     );
        List<SelectListItem> menuItems = new List<SelectListItem>();
        foreach (var item in items)
        {
            menuItems.Add(new SelectListItem { Text = item.Name, Value = "/Products/" + Model.Id + "/" + item.Id });
        }
        Html.RenderAction<Zamov.Controllers.PagePartsController>(ac => ac.LeftMenu(Html.ResourceString("Groups"), menuItems));
    }
%>
</asp:Content>
