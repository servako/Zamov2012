<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Zamov.Models.DealerPresentation>" %>
<div style="margin:10px;">
<h2>
<%= Model.Name %>
</h2>

<div style="padding-top:10px;">
    <%= Model.Description %>
</div>
</div>