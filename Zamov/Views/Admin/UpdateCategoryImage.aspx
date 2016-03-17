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
                Response.Write(Html.Image("~/Image/CategoryImage/" + ViewData["imageId"]));
            using (Html.BeginForm("UpdateCategoryImage", "Admin", FormMethod.Post, new { enctype = "multipart/form-data" }))
            {%>
                <%= Html.Hidden("id", ViewData["imageId"]) %>
                <%= Html.Hidden("categoryId", ViewData["categoryId"])%>
                <input type="file" name="newImage" />
                <input type="submit" value="<%= Html.ResourceString("Add") %>" />
        <%  }
        %>    
    </body>
</html>
