<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.Order>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var items = {};
</script>

<%
    if (Model != null)
    {
        foreach (var order in Model)
            Html.RenderAction<PagePartsController>(ac => ac.ShowOrder(order, true, (string)ViewData["redirectUrl"]));
    }
%>    
<center>
   <% using(Html.BeginForm("Recalculate", "Cart", FormMethod.Post)){ %>
        <%= Html.Hidden("updates") %>
        <%= Html.Hidden("redirectUrl", (string)ViewData["redirectUrl"])%>
        <%= Html.SubmitButton("recalculate", Html.ResourceString("Recalculate"), new { onclick = "collectChanges(items, 'updates');" })%>
    <%} %>
</center>
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
     List<CategoryPresentation> categories = (List<CategoryPresentation>)ViewData["categories"];
     List<SelectListItem> items = new List<SelectListItem>();
     foreach (var item in categories)
     {
         SelectListItem listItem = new SelectListItem { Text = item.Name, Value = "/Dealers/" + item.Id };
         items.Add(listItem);
     }
     Html.RenderAction<PagePartsController>(ac => ac.LeftMenu(Html.ResourceString("Categories"), items));
 %>

</asp:Content>
