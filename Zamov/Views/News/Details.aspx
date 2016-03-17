<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NewsPresentation>" %>
<%@ Import Namespace="Zamov.Models" %>
<div class="newsDetail">
    <div class="title">
        <%= Model.Title %>
    </div>
    <%= Model.LongText %>
</div>