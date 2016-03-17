using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Helpers;
using Zamov.Models;

namespace Zamov.Controllers
{
    public class SeoController : Controller
    {
        //
        // GET: /Seo/
        [Authorize(Roles="Administrators, Managers")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrators, Managers")]
        public ActionResult ForUrl()
        {
            string lang = SystemSettings.CurrentLanguage;
            ViewData["lng"] = lang;
            using (SeoStorage context = new SeoStorage())
            {
                return View(context.Seo.Where(s => s.Language == lang).ToList());
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForUrl(int? id, string lang, string url, string keywords, string description, string title)
        {
            using (SeoStorage context = new SeoStorage())
            {
                Seo seo;
                if (id != null)
                {
                    seo = context.Seo.Where(s => s.Id == id.Value).First();
                }
                else
                {
                    seo = new Seo();
                    context.AddToSeo(seo);
                }
                seo.Language = lang;
                seo.Url = url;
                seo.Keywords = keywords;
                seo.Description = description;
                seo.Title = title;

                context.SaveChanges();
            }
            return RedirectToAction("ForUrl");
        }

        public ActionResult DeleteSeo(int id)
        {
            using (SeoStorage context = new SeoStorage())
            {
                Seo seo = context.Seo.Where(s => s.Id == id).First();
                context.DeleteObject(seo);
                context.SaveChanges();
            }
            return RedirectToAction("ForUrl");
        }

        public ActionResult SeoTemplate()
        {
            using (SeoStorage context = new SeoStorage())
            {
                Dictionary<string, Dictionary<string, string>>  dictForView = context.GetSeoDictionary();

                return View(dictForView);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeoTemplate(FormCollection form)
        {
            Dictionary<string, Dictionary<string, string>> updates = form.ProcessPostData();

            Dictionary<string, Dictionary<string, string>> dictForView;

            using (SeoStorage context = new SeoStorage())
            {
                List<SeoDictionary> seoDictionaries = context.SeoDictionary.ToList();

                foreach (var updKey in updates)
                {
                    foreach (var updLangValue in updKey.Value)
                    {
                        SeoDictionary seoUpdate = seoDictionaries.Where(sd => sd.Language == updLangValue.Key && sd.SeoKey == updKey.Key).FirstOrDefault();

                        if (seoUpdate == null)
                        {
                            seoUpdate = new SeoDictionary();
                            seoUpdate.Language = updLangValue.Key;
                            seoUpdate.SeoKey = updKey.Key;
                            seoUpdate.SeoValue = updLangValue.Value;
                            context.AddToSeoDictionary(seoUpdate);
                        }
                        else
                        {
                            seoUpdate.SeoValue = updLangValue.Value;
                        }
                        context.SaveChanges();
                    }
                }
         
                HttpContext.Cache.ClearSeoDictionary();
                dictForView = context.GetSeoDictionary();
            }
            return View(dictForView);
        }
    }
}
