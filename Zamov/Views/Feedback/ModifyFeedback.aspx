<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.FeedbackPresentation>>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ModifyFeedback</title>
</head>
<body>
    <div>
        <%
            foreach (var item in Model)
            {%>
        <div class="feedbackItem">
            <div class="itemHeader">
                <a href="mailto:<%= item.Email %>">
                    <%= item.FirstName %>
                </a>
                <div>
                    <%using (Html.BeginForm("DeleteFeedback", "Feedback", FormMethod.Post)){ %>
                        <%= Html.Hidden("feedbackId", item.Id) %>
                        <%= Html.Hidden("dealerId") %>
                        <input type="submit" value='<%= Html.ResourceString("delete") %>' />
                    <%} %>
                </div>
            </div>
            <div class="itemText">
                <%= item.Text %>
            </div>
        </div>
        <%  }
        %>
    </div>
</body>
</html>
