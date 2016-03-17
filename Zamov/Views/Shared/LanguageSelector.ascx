<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Helpers" %>
<%@ Import Namespace="Zamov.Controllers" %>
<% 
    string returnUrl = Request.Url.AbsolutePath;
    string currentLanguage = ((string)Session["lang"]) ?? "uk-UA";
    if (currentLanguage == "uk-UA")
        Response.Write(Html.ResourceString("Ukr") + "/" + Html.ResourceActionLink<HomeController>("Rus", c=>c.SetRussian(returnUrl)));
    else
        Response.Write(Html.ResourceActionLink<HomeController>("Ukr", c => c.SetUkrainian(returnUrl)) + "/" + Html.ResourceString("Rus"));
%>
