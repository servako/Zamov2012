<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Zamov.Models.FeedbackPresentation>>" %>

<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<div id="feedbackConteiner">
    <%
        if (Model.Count() == 0)
        {
            Response.Write("<center>" + Html.ResourceString("EmptyFeedback") + "</center>");
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
        Html.RenderAction("CreateFeedback", "Feedback", new { id = ViewData["dealerId"] });
    }
    else
    { %>
<div class="registerToFeedback">
    <%= Html.ResourceActionLink("RegisterToFeedback", "Register", "Account") %>
</div>
<%  } 
%>