<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Models" %>
<% 
    int productId = (int)ViewData["productId"];
    bool hasImage = false;
    using (ZamovStorage context = new ZamovStorage())
    {
        hasImage = context.ProductImages.Where(pi => pi.Product.Id == productId).Count() > 0;
    }

    if (hasImage)
    {
        Response.Write("<img alt=\"\" src=\"/Image/ProductImageDefault/" + productId + "\"/>");
    }
    else
    {
        Response.Write("<img alt=\"No Image\" src=\"/Content/img/noImage.png\"/>");
    }
%>

