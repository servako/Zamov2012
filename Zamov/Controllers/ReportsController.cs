using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;

namespace Zamov.Controllers
{

    [Authorize(Roles = "Administrators, Managers")]
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SalesReport(int? dealerId, string userName, string city, Statuses? orderState, string after, string before, string sortField, SortDirection? sortOrder)
        {
            StringBuilder filterString = new StringBuilder();
            if (dealerId != null)
                filterString.AppendFormat("&dealerId={0}", dealerId);
            if (!string.IsNullOrEmpty(userName))
                filterString.AppendFormat("&userName={0}", userName);
            if (!string.IsNullOrEmpty(city))
                filterString.AppendFormat("&city={0}", city);
            if (orderState != null)
                filterString.AppendFormat("&orderState={0}", orderState);
            if (!string.IsNullOrEmpty(after))
                filterString.AppendFormat("&after={0}", after);
            if (!string.IsNullOrEmpty(before))
                filterString.AppendFormat("&before={0}", before);

            string filter = filterString.ToString();
            if (filter.Length > 0)
                filter = filter.Substring(1);

            ViewData["filterString"] = filter;

            List<SelectListItem> dealers = null;
            int translationType = (int)ItemTypes.DealerName;

            using (ZamovStorage context = new ZamovStorage())
            {
                var tmp = context.Dealers.Join(context.Translations
                    .Where(tr => tr.Language == "uk-UA" && tr.TranslationItemTypeId == translationType),
                    d => d.Id, t => t.ItemId,
                    (d, t) => new { Id = d.Id, Name = t.Text }).ToList();

                dealers = tmp.Select(d => new SelectListItem { Selected = (dealerId != null && d.Id == dealerId.Value), Text = d.Name, Value = d.Id.ToString() }).ToList();
            }
            dealers.Insert(0, new SelectListItem { Text = string.Empty, Value = string.Empty });
            ViewData["dealerId"] = dealers;

            List<SelectListItem> states = new List<SelectListItem>();
            states.Add(new SelectListItem { Text = "", Value = "" });
            states.Add(new SelectListItem { Text = "Прийнятий", Value = "Accepted", Selected = (orderState != null && orderState == Statuses.Accepted) });
            states.Add(new SelectListItem { Text = "Вiдхилений", Value = "Canceled", Selected = (orderState != null && orderState == Statuses.Canceled) });
            states.Add(new SelectListItem { Text = "Новий", Value = "New", Selected = (orderState != null && orderState == Statuses.New) });
            states.Add(new SelectListItem { Text = "Доставлений", Value = "Complited", Selected = (orderState != null && orderState == Statuses.Complited) });

            ViewData["orderState"] = states;

            HttpContext.Items["sortField"] = ViewData["sortField"] = sortField;
            SortDirection sortDirection = (sortOrder == SortDirection.Ascending || sortOrder == null) ? SortDirection.Ascending : SortDirection.Descending;
            HttpContext.Items["sortDirection"] = ViewData["sortDirection"] = sortDirection;

            DateTime? dateAfter = null;
            DateTime? dateBefore = null;

            if (after == null && before == null)
            {
                dateAfter = DateTime.Today;
                dateBefore = DateTime.Today.AddDays(1);
                ViewData["after"] = dateAfter.Value.ToString("dd.MM.yyyy");
                ViewData["before"] = dateBefore.Value.ToString("dd.MM.yyyy");
            }


            if (!string.IsNullOrEmpty(after))
            {
                dateAfter = DateTime.Parse(after, CultureInfo.GetCultureInfo("uk-UA"));
            }
            if (!string.IsNullOrEmpty(before))
            {
                dateBefore = DateTime.Parse(before, CultureInfo.GetCultureInfo("uk-UA"));
            }

            using (Reports context = new Reports())
            {
                int? orderStatus = (orderState.HasValue) ? (int)orderState.Value : (int?)null;
                var orders = context.SalesReport.OrderBy(o => o.OrderId)
                    .Where(o => (dealerId == null || o.DealerId == dealerId))
                    .Where(o => (userName == null || userName == string.Empty || o.UserName.Contains(userName)))
                    .Where(o => (city == null || city == string.Empty || o.City.Contains(city)))
                    .Where(o => (orderStatus == null || o.Status == orderStatus))
                    .Where(o => (dateAfter == null || o.OrderDate >= dateAfter.Value))
                    .Where(o => (dateBefore == null || o.OrderDate <= dateBefore.Value))
                    .OrderByDescending(o => o.OrderId)
                    .Select(o => o).ToList();

                if (!string.IsNullOrEmpty(sortField))
                {
                    int direction = (sortOrder == SortDirection.Ascending || sortOrder == null) ? direction = 1 : direction = -1;

                    orders.Sort(delegate(SalesReportItem a, SalesReportItem b)
                    {
                        return Utils.CompareObjectFields(a, b, sortField, direction);
                    });
                }

                return View(orders);
            }
        }

    }
}
