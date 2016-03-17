<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<html>
    <head>
        <title></title>
    </head>
    <body>
        <%
            if (Convert.ToInt32(ViewData["imageId"]) >= 0)
                Response.Write(Html.Image("~/Image/ProductImage/" + ViewData["imageId"]));
            using (Html.BeginForm("UpdateProductImage", "DealerCabinet", FormMethod.Post, new { enctype = "multipart/form-data" }))
            {%>
                <%= Html.Hidden("id", ViewData["imageId"]) %>
                <%= Html.Hidden("productId", ViewData["productId"])%>
                <input type="file" name="newImage" />
                <input type="submit" value="<%= Html.ResourceString("Add") %>" />
        <%  }
        %>    
    </body>
</html>
