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
    public class NewsController : Controller
    {
        [BreadCrumb( ResourceName="News", Url="/News")]
        public ActionResult Index()
        {
            using (NewsStorage context = new NewsStorage())
            {
                List<NewsPresentation> news = (from newsItem in context.News
                                               join title in context.Translations on newsItem.Id equals title.ItemId
                                               join shortText in context.Translations on newsItem.Id equals shortText.ItemId
                                               where title.TranslationItemTypeId == (int)ItemTypes.NewsTitle
                                               && title.Language == SystemSettings.CurrentLanguage
                                               && shortText.TranslationItemTypeId == (int)ItemTypes.NewsDescription
                                               && shortText.Language == SystemSettings.CurrentLanguage
                                               && newsItem.Enabled
                                               orderby newsItem.Date descending
                                               select new NewsPresentation
                                               {
                                                   Id = newsItem.Id,
                                                   Enabled = newsItem.Enabled,
                                                   Date = newsItem.Date,
                                                   Title = title.Translation,
                                                   ShortText = shortText.Translation
                                               }).ToList();


                ViewData["news"] = news.Take(3).ToList();
                return View(news);
            }
        }
        
        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult Details(int id)
        {
            using (NewsStorage context = new NewsStorage())
            {
                NewsPresentation news = (from newsItem in context.News
                                         join title in context.Translations on newsItem.Id equals title.ItemId
                                         join longText in context.Translations on newsItem.Id equals longText.ItemId
                                         where title.TranslationItemTypeId == (int)ItemTypes.NewsTitle
                                         && title.Language == SystemSettings.CurrentLanguage
                                         && longText.TranslationItemTypeId == (int)ItemTypes.NewsText
                                         && longText.Language == SystemSettings.CurrentLanguage
                                         && newsItem.Enabled
                                         && newsItem.Id == id
                                         orderby newsItem.Date descending
                                         select new NewsPresentation
                                         {
                                             Id = newsItem.Id,
                                             Enabled = newsItem.Enabled,
                                             Date = newsItem.Date,
                                             Title = title.Translation,
                                             LongText = longText.Translation
                                         }).First();
                return View(news);
            }
        }

    }
}
