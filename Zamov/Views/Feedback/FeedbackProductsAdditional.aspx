<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.FeedbackPresentation>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<div id="feedbackConteiner">
    <%
        if (Model.Count() == 0)
        {
            Response.Write("<center>" + Html.ResourceString("EmptyFeedbackForProduct") + "</center>");
        }  
    %>
    <% foreach (var item in Model)
       {%>
    <div class="feedbackItem">
        <div class="itemHeader">
            <%=item.Date.ToShortDateString() %>&nbsp;
            <a href="mailto:<%= item.Email %>">
                <%= item.FirstName %>
            </a>
        </div>
        <div class="itemText">
            <%= item.Text %>
        </div>
    </div>
    <% } %>
</div>
<% 
    if (Request.IsAuthenticated)
    {
        Html.RenderAction("CreateFeedbackProductAdditional", "Feedback", new { id = ViewData["productId"] });
    }
    else
    { %>
<div class="registerToFeedback">
    <%= Html.ResourceActionLink("RegisterToFeedback", "Register", "Account") %>
</div>
<%  } 
%>