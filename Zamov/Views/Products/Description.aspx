<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ProductPresentation>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Models" %>
<%@ Import Namespace="Zamov.Controllers" %>

<div>
    <%
        if (Model.HasImage)
        {%>
            <div class="productDescriptionImage">
                <%= Html.Image("~/Image/ProductImageDefault/" + Model.Id + "/400")%>
            </div>
            <%
        }
    %>
    <%= Model.Description %>
</div>