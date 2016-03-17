using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zamov.Controllers;
using Zamov.Models;

namespace Zamov.Helpers
{
    public static class SeoHelper
    {
        private static SeoMetaTags _seoMetaTags = new SeoMetaTags();

        public static void AddInfo(SeoEntityType seoEntityType, int entityID, SeoMetaTags defaultSeoMetaTags)
        {
            string cityName = "";
            _seoMetaTags = new SeoMetaTags();

            using (ZamovStorage context = new ZamovStorage())
            {
                cityName = (from c in context.Cities
                               join tran in context.Translations on c.Id equals tran.ItemId
                               where tran.TranslationItemTypeId == (int)ItemTypes.City
                                   && tran.Language == SystemSettings.CurrentLanguage
                                   && c.Id == SystemSettings.CityId
                               select tran.Text).First();
            }

            using (SeoStorage context = new SeoStorage())
            {
                int typeEntity = (int) seoEntityType;
                SeoAdditional seoAdditional = context.SeoAdditional.
                    Where(sa => sa.IdEntity == entityID && sa.TypeEntity == typeEntity && sa.Language == SystemSettings.CurrentLanguage).FirstOrDefault();

                if (seoAdditional != null)
                {
                    _seoMetaTags.Title = !String.IsNullOrEmpty(seoAdditional.Title) ? seoAdditional.Title : defaultSeoMetaTags.Title;
                    _seoMetaTags.Description = !String.IsNullOrEmpty(seoAdditional.Description) ? seoAdditional.Description : defaultSeoMetaTags.Description;
                    _seoMetaTags.Keywords = !String.IsNullOrEmpty(seoAdditional.Keywords) ? seoAdditional.Keywords : defaultSeoMetaTags.Keywords;
                    _seoMetaTags.MetaTags = !String.IsNullOrEmpty(seoAdditional.MetaTags) ? seoAdditional.Keywords : defaultSeoMetaTags.MetaTags;
                }
                else
                {
                    _seoMetaTags = defaultSeoMetaTags;
                }

                string afterTitle = context.GetSeoDictionary().ContainsKey(SeoDictionaryType.AfterTitle.ToString())
                                        ? context.GetSeoDictionary()[SeoDictionaryType.AfterTitle.ToString()].ContainsKey(SystemSettings.CurrentLanguage)
                                              ? context.GetSeoDictionary()[SeoDictionaryType.AfterTitle.ToString()][SystemSettings.CurrentLanguage]
                                              : ""
                                        : "";

                cityName = String.Format(" {0}.", cityName);

                switch (seoEntityType)
                {
                    case SeoEntityType.Categories:
                        _seoMetaTags.Title = String.Format("{0}{1}{2}{3}{4}{5}",
                            context.GetDictionaryValue(SeoDictionaryType.BeforeCategory),
                            _seoMetaTags.Title,
                            context.GetDictionaryValue(SeoDictionaryType.AfterCategory),
                            context.GetAdditionalInfo(defaultSeoMetaTags),
                            cityName,
                            afterTitle);
                        break;
                    case SeoEntityType.Dealers:
                        _seoMetaTags.Title = String.Format("{0}{1}{2}{3}{4}{5}",
                            context.GetDictionaryValue(SeoDictionaryType.BeforeDealer),
                            _seoMetaTags.Title,
                            context.GetDictionaryValue(SeoDictionaryType.AfterDealer),
                            context.GetAdditionalInfo(defaultSeoMetaTags),
                            cityName,
                            afterTitle);
                        break;
                    case SeoEntityType.Groups:
                        _seoMetaTags.Title = String.Format("{0}{1}{2}{3}{4}{5}",
                            context.GetDictionaryValue(SeoDictionaryType.BeforeGroup),
                            _seoMetaTags.Title,
                            context.GetDictionaryValue(SeoDictionaryType.AfterGroup),
                            context.GetAdditionalInfo(defaultSeoMetaTags),
                            cityName,
                            afterTitle);
                        break;
                    case SeoEntityType.Product:
                        _seoMetaTags.Title = String.Format("{0}{1}{2}{3}{4}{5}",
                            context.GetDictionaryValue(SeoDictionaryType.BeforeProduct),
                            _seoMetaTags.Title,
                            context.GetDictionaryValue(SeoDictionaryType.AfterProduct),
                            context.GetAdditionalInfo(defaultSeoMetaTags),
                            cityName,
                            afterTitle);
                        break;
                }
            }
        }

        private static string GetAdditionalInfo(this SeoStorage context, SeoMetaTags defaultSeoMetaTags)
        {
            return String.IsNullOrEmpty(defaultSeoMetaTags.AdditionInfo)
                       ? ""
                       : String.Format(" {0}{1}{2} ", 
                            context.GetDictionaryValue(SeoDictionaryType.BeforeAdditionInfo), 
                            defaultSeoMetaTags.AdditionInfo, 
                            context.GetDictionaryValue(SeoDictionaryType.AfterAdditionInfo));
        }

        private static string GetDictionaryValue(this SeoStorage context, SeoDictionaryType seoDictionaryType)
        {
            return context.GetSeoDictionary().ContainsKey(seoDictionaryType.ToString())
                       ? context.GetSeoDictionary()[seoDictionaryType.ToString()].ContainsKey(SystemSettings.CurrentLanguage)
                             ? context.GetSeoDictionary()[seoDictionaryType.ToString()][SystemSettings.CurrentLanguage]
                             : ""
                       : "";
        }

        public static string GetSeoTitle(this HtmlHelper helper)
        {
            StringBuilder layout = new StringBuilder();

            layout.AppendFormat("{0}", _seoMetaTags.Title);
            _seoMetaTags.Title = "";

            return layout.ToString();
        }

        public static string GetSeo(this HtmlHelper helper)
        {
            StringBuilder layout = new StringBuilder();

            if (!String.IsNullOrEmpty(_seoMetaTags.Keywords))
            {
                layout.AppendFormat(@"<meta name=""Keywords"" content=""{0}"" />", _seoMetaTags.Keywords);
                _seoMetaTags.Keywords = "";
            }

            if (!String.IsNullOrEmpty(_seoMetaTags.Description))
            {
                layout.AppendFormat(@"<meta name=""Description"" content=""{0}"" />", _seoMetaTags.Description);
                _seoMetaTags.Description = "";
            }
            
            if (!String.IsNullOrEmpty(_seoMetaTags.MetaTags))
            {
                layout.AppendFormat(@"{0}", _seoMetaTags.MetaTags);
                _seoMetaTags.MetaTags = "";
            }

            return layout.ToString();
        }
    }
}