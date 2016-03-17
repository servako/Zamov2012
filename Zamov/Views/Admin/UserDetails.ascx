<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserPresentation>" %>
<%@ Import Namespace="System.Web.Security" %>
<%@ Import Namespace="System.Web.Profile" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>
    
<% 
    string userType = "customers";
    if (!string.IsNullOrEmpty((string)ViewContext.HttpContext.Items["userType"]))
        userType = (string)ViewContext.HttpContext.Items["userType"];
%>

<tr>
    <td>
        <%= Model.Email%>
        <%= Html.Hidden("userName", Model.Email) %>
    </td>
    <td>
        <%= Model.FirstName %>
    </td>
    <td>
        <%= Model.LastName %>
    </td>
    <%if(userType=="dealers"){ %>
    <td>
        <%= Model.DealerName %>
    </td>
    <%} %>
    <td>
        <%= Ajax.ActionLink("[img]", 
            "UpdateUser", 
            new 
            { 
                id = Model.Email,
                userType = ViewContext.HttpContext.Items["userType"],
                sortField = ViewContext.HttpContext.Items["sortField"],
                sortOrder = ViewContext.HttpContext.Items["sortOrder"] 
            }, 
            new AjaxOptions 
            { 
                InsertionMode = InsertionMode.Replace ,
                UpdateTargetId = Model.GetHashCode().ToString(),
                OnSuccess = "userUpdateLoaded",
                OnBegin = "updateUserBegin",
                OnComplete = "fadeScreenIn",
                HttpMethod = "GET",
                OnFailure = "failure"
            }).ToHtmlString().Replace("[img]", Html.Image("~/Content/img/edit.png").ToHtmlString())%>
    </td>
    <td>
        <%= Html.ActionLink("[img]", "DeleteUser", new 
        { 
            id = Model.Email, 
            userType = ViewContext.HttpContext.Items["userType"],
            sortField = ViewContext.HttpContext.Items["sortField"],
            sortOrder = ViewContext.HttpContext.Items["sortOrder"] 
        }, 
        new { onclick = "return confirm('" + Html.ResourceString("AreYouSure") + "?')" })
        .ToHtmlString().Replace("[img]", Html.Image("~/Content/img/delete.png").ToHtmlString())%>
    </td>
</tr>
<tr>
    <td colspan="5" class="updateUser" id="<%= Model.GetHashCode()%>"></td>
</tr>

<script type="text/javascript">
    function updateUserBegin() {
        $(".updateUser").html("").css("display", "none");
        fadeScreenOut();
    }

    function userUpdateLoaded(response) {
        var updateTarget = response.get_updateTarget();
        $(updateTarget).slideDown("fast");
    }

    function failure(attr) {
    }
</script>