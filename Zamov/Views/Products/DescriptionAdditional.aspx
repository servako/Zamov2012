<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ProductByGroupPresent>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>
<div>
    <div class="productDescriptionImage">
        <%= Html.Image(String.IsNullOrEmpty(Model.Url) ? "~/Content/img/noImage.png" : Model.Url, new { style = "max-height:400px;max-width:400px;" })%>
    </div>
    <%= Model.Description %>
</div>