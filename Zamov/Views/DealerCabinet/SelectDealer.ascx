<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Zamov.Models" %>
<%
    using (ZamovStorage context = new ZamovStorage())
    {
        List<Dealer> dealers = (from dealer in context.Dealers orderby dealer.IndexNumber select dealer).ToList();
        List<SelectListItem> items = (from d in dealers select new SelectListItem { Text = d.GetName(Zamov.Controllers.SystemSettings.CurrentLanguage), Value = d.Id.ToString() }).ToList();
        if (Zamov.Controllers.SystemSettings.CurrentDealer == null)
        {
            if (items.Count > 0)
            {
                items[0].Selected = true;
                Zamov.Controllers.SystemSettings.CurrentDealer = int.Parse(items[0].Value);
                Response.Redirect(Request.Url.AbsolutePath);
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (item.Value == Zamov.Controllers.SystemSettings.CurrentDealer.Value.ToString())
                    item.Selected = true;
            }
        }
        using (Html.BeginForm("SelectDealer", "DealerCabinet", FormMethod.Get))
        {
        %>
            <%= Html.Hidden("redirectTo", Request.Url.AbsoluteUri) %>
            <%= Html.DropDownList("currentDealerId", items, new { onchange="$(this).parents('form').submit();"})%>
        <%
        }
    }
%>
