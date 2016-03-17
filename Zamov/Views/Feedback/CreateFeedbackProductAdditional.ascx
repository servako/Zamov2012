<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<script type="text/javascript">
    function feedbackResponse(response) {
        var result = response.get_response().get_object();
        if (result) {
            $("#fancy_ajax").load('/Feedbacks/<%= ViewData["productId"] %>')
        }
        else {
            var feedbackText = $get("text").value;
            alert('<%= Html.ResourceString("IncorrectCaptcha") %>');
            $("#fancy_ajax").load('/Feedbacks/<%= ViewData["productId"] %>', function () { $get("text").value = feedbackText; });
        }
    }
</script>
<div id="createFeedback">
<% using (Ajax.BeginForm("CreateFeedbackProductAdditional", "Feedback", 
       new AjaxOptions { HttpMethod = "POST", OnBegin = "fadeScreenOut", OnComplete = "fadeScreenIn", OnSuccess = "feedbackResponse" }))
   { %>
    <%= Html.TextArea("text", new{@class="feedbackTextInput"}) %>
    <br />
    <%= Html.CaptchaImage(50, 150) %>
    <br />
    <%= Html.TextBox("captcha") %>
    <%= Html.Hidden("productId")%>
    <%= Html.SubmitButton("sendFeedback", Html.ResourceString("CreateFeedback")) %>
<%} %>
</div>