using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using System.Web.Security;
using System.Data.Objects;
using Zamov.Helpers;
using System.Text;

namespace Zamov.Controllers
{
    public class PagePartsController : Controller
    {
        //
        // GET: /PageParts/

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult HeaderSelectors()
        {
            int categoryId = Convert.ToInt32(HttpContext.Items["categoryId"]);
            var productVersion = (ProductVersion)Convert.ToInt32(HttpContext.Items["productVersion"]); 

            using (ZamovStorage context = new ZamovStorage())
            {
                string currentLanguage = SystemSettings.CurrentLanguage;
                List<SelectListItem> citiesList = context.GetCitiesFromContext(SystemSettings.CurrentLanguage);
                int cityId = int.MinValue;
                if (citiesList.Count > 0)
                {
                    SystemSettings.InitializeCity(int.Parse(citiesList[0].Value));
                    if (citiesList.Where(cl => cl.Selected).Count() == 0)
                    {
                        if (SystemSettings.CityId > 0)
                        {
                            citiesList.ForEach(c => c.Selected = c.Value == SystemSettings.CityId.ToString());
                            // ƒоп проверка дл€ случа€ неправильного сити айди
                            if (citiesList.Where(cl => cl.Selected).Count() == 0)
                            {
                                SystemSettings.CityId = int.Parse(citiesList[0].Value);
                                citiesList.ForEach(c => c.Selected = c.Value == SystemSettings.CityId.ToString());
                            }
                        }
                        else
                        {
                            citiesList[0].Selected = true;
                        }
                    }
                    cityId = (from cl in citiesList where cl.Selected select int.Parse(cl.Value)).First();
                }
                List<SelectListItem> categoriesList = context.GetCachedCategories(cityId, SystemSettings.CurrentLanguage)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Additional ? String.Format("/Products/ShowCategory/{0}/{1}", SystemSettings.CityId, c.Id) : "/Dealers/" + c.Id.ToString(),
                        Selected = productVersion == ProductVersion.Additional ?
                            (c.Additional && c.Id == categoryId) || (c.Additional && c.Children.Where(ch => ch.Id == categoryId).Count() > 0) :
                            (!c.Additional && c.Id == categoryId) || (!c.Additional && c.Children.Where(ch => ch.Id == categoryId).Count() > 0)
                    })
                    .ToList();
                categoriesList.Insert(0, new SelectListItem { Selected = true, Text = "--" + ResourcesHelper.GetResourceString("SelectCategory") + "--", Value = "" });
                ViewData["citiesList"] = citiesList;
                ViewData["categoriesList"] = categoriesList;
                return View();
            }
        }

        public ActionResult MainMenu()
        {
            ViewData["authenticated"] = User.Identity.IsAuthenticated;
            if (User.Identity.IsAuthenticated)
            {
                ViewData["dealer"] = Roles.IsUserInRole("Dealers");
                ViewData["admin"] = Roles.IsUserInRole("Administrators");
                ViewData["customer"] = Roles.IsUserInRole("Customers");
                ViewData["manager"] = Roles.IsUserInRole("Managers");
            }
            return View();
        }

        public ActionResult LeftMenu(string caption, IEnumerable<SelectListItem> items)
        {
            ViewData["caption"] = caption;
            return View(items);
        }

        public ActionResult News(string caption, IEnumerable<NewsPresentation> items)
        {
            ViewData["caption"] = caption;
            return View(items);
        }

        public ActionResult GetCityCategories(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
                return Json(context.GetCategories(id));
        }

        public ActionResult ShowOrder(Order order, bool cartMode, string redirectUrl)
        {
            ViewData["redirectUrl"] = redirectUrl;
            if (cartMode)
                ViewData["cartMode"] = cartMode;
            return View(order);
        }

        public ActionResult CurrentDealer(int id)
        {
            ViewData["dealerId"] = id;
            return View();
        }

        public ActionResult TranslatedText(string formAction, string formController, string richEditorPanel)
        {
            ViewData["formAction"] = formAction;
            ViewData["formController"] = formController;
            ViewData["richEditorPanel"] = richEditorPanel;
            return View();
        }
    }
}
