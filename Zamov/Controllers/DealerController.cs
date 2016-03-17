using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using Zamov.Helpers;

namespace Zamov.Controllers
{
    public class DealerController : Controller
    {
        //
        // GET: /Dealer/

        public ActionResult Index(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                DealerPresentation dealerProperties = (from dealer in context.Dealers
                                                       join translation in context.Translations on dealer.Id equals translation.ItemId
                                                       join nameTranslations in context.Translations on dealer.Id equals nameTranslations.ItemId
                                                       where translation.TranslationItemTypeId == (int)ItemTypes.DealerDescription
                                                            && translation.Language == SystemSettings.CurrentLanguage
                                                            && nameTranslations.TranslationItemTypeId == (int)ItemTypes.DealerName
                                                            && nameTranslations.Language == SystemSettings.CurrentLanguage
                                                            && dealer.Id == id
                                                       orderby dealer.IndexNumber
                                                       select new DealerPresentation { Id = id, Name = nameTranslations.Text, Description = translation.Text }
                                                ).First();
                ViewData["description"] = dealerProperties;
                ViewData["dealerId"] = id;
                return View(dealerProperties);
            }
        }

        public ActionResult DealerInfo(int id)
        {
            using(ZamovStorage context = new ZamovStorage())
            {
                DealerPresentation dealerDescription = (from dealer in context.Dealers
                                                        join translation in context.Translations on dealer.Id equals translation.ItemId
                                                        join nameTranslations in context.Translations on dealer.Id equals nameTranslations.ItemId
                                                        where dealer.Id == id
                                                        && translation.Language == SystemSettings.CurrentLanguage
                                                        && nameTranslations.Language == SystemSettings.CurrentLanguage
                                                        && translation.TranslationItemTypeId == (int)ItemTypes.DealerDescription
                                                        && nameTranslations.TranslationItemTypeId == (int)ItemTypes.DealerName
                                                        select new DealerPresentation { Name = nameTranslations.Text, Description = translation.Text }).First();
                return View(dealerDescription);
           }
        }

        public ActionResult Feedback(int id)
        {
            return View();
        }

        public ActionResult DealerDeliveryDetails(int id)
        {
            int dealerId = id;
            string dealerDeliveryInfo = "";
            using (ZamovStorage context = new ZamovStorage())
            {
                var query = (from translation in context.Translations
                             where translation.Language == SystemSettings.CurrentLanguage
                             && translation.ItemId == dealerId
                             && translation.TranslationItemTypeId == (int)ItemTypes.DealerDeliveryInfo
                             select translation.Text);
                if (query.Count() > 0)
                    dealerDeliveryInfo = query.First();
            }
            ViewData["dealerDeliveryInfo"] = dealerDeliveryInfo;
            return View();
        }
    }
}
