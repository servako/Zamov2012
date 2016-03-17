<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Zamov.Models.Advert>" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Models" %>


 <%if (Model.IsActive)
          {%>
<div id="bannerTopOuter">
        <div id="bannerTopInner">
            <%=Html.RegisterFlashScript(Model.ImageSource, "b_" + Model.Id, (BannerPosition)Model.Position)%>
        </div>
</div>
 <%}%>