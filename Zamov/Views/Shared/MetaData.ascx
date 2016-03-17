<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Models" %>
<% 
    string path = Request.Url.PathAndQuery;
    string description, keywords, title;
    title = keywords = description = string.Empty;
    using(SeoStorage context = new SeoStorage())
	{
		Seo seo = context.Seo.Where(s=>s.Language == Zamov.Controllers.SystemSettings.CurrentLanguage).Where(s=>s.Url == path).FirstOrDefault();
        if(seo!=null)
        {
            keywords = seo.Keywords;
            description = seo.Description;
            title = seo.Title;
        }
	}
    TempData["title"] = title;
%>

 <meta name="Keywords" content="<%= keywords %>" />
 <meta name="Description" content="<%= description %>" />
 <meta name="robots" content="index, follow"/>
 <meta name="revisit-after" content="1 weeks"/>