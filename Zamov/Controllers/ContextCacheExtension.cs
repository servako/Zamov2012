using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.Data.Extensions;
using Zamov.Models;
using System.Web.Caching;
using System.Collections;
using System.Web.Mvc;

namespace Zamov.Controllers
{
    public static class ContextCache
    {
        public const string CategoriesCachePrefix = "CityCategoriesPresentation_";
        public const string CategoryChainCachePrefix = "CategoriesChain_";
        public const string CitiesCachePrefix = "Cities_";
        public const string ProductsAdditionalByGroupAndCityCachePrefix = "ProductsAdditionalByGroupAndCity_";
        public const string ProductsAdditionalByGroupAndDealerCachePrefix = "ProductsAdditionalByGroupAndDealer_";
        public const string SeoDictionaryCachePrefix = "SeoDictionary";

        private static Cache Cache { get { return HttpContext.Current.Cache; } }

        public static List<CategoryPresentation> GetCachedCategoryChain(this ZamovStorage context, int categoryId, string language)
        {
            string cacheKey = CategoryChainCachePrefix + categoryId + "_" + language;

            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                var initialCategories = context.GetTranslatedCategories(language, true, null, false);
                List<CategoryPresentation> categories = initialCategories
                    .Where(item => item.Entity.Id == categoryId)
                    .Union(
                        initialCategories.Where(c => c.Entity.Categories.Any(cat => cat.Id == categoryId))
                    )
                    .Select(tc => new CategoryPresentation
                    {
                        Id = tc.Entity.Id,
                        Name = tc.Translation.Text,
                        ParentId = tc.Entity.Parent.Id
                    }).ToList();
                HttpContext.Current.Cache[cacheKey] = categories;
            }
            return (List<CategoryPresentation>)HttpContext.Current.Cache[cacheKey];
        }

        public static List<ProductPricePresentation> GetCachedProductsAdditional(this ZamovStorage context, int groupId, int cityId)
        {
            string cacheKey = ProductsAdditionalByGroupAndCityCachePrefix + groupId + "_" + cityId;
            
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                var prod = new List<ProductPricePresentation>();
                var parameters = new object[] { new SqlParameter("@grID", groupId), 
                                                new SqlParameter("@cityId", cityId) };

                DbCommand command = context.CreateStoreCommand("sp_ProductPricesByGroupAndCity_Select", CommandType.StoredProcedure, parameters);

                using (context.Connection.CreateConnectionScope())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var d = new ProductPricePresentation
                                        {
                                            Id = Convert.ToInt32(reader["p_prodID"]),
                                            IdDealerProd = Convert.ToInt32(reader["dpID"]),
                                            Name = Convert.ToString(reader["p_prodName"]),
                                            dpName = Convert.ToString(reader["dp_prodName"]),
                                            DealerName = Convert.ToString(reader["dealerName"]),
                                            DealerId = Convert.ToInt32(reader["dp_dealerID"]),
                                            PriceHrn = Convert.ToDecimal(reader["pd_price_hrn"]),
                                            Unit = Convert.ToString(reader["p_unit"]),
                                            dpUnit = Convert.ToString(reader["dp_unit"]),
                                            Description = Convert.ToString(reader["p_descr"]),
                                            dpDescription = Convert.ToString(reader["dp_descr"]),
                                            Url = Convert.ToString(reader["p_photo_url"]),
                                            dpUrl = Convert.ToString(reader["dp_photoUrl"]),
                                            DateBegin = Convert.ToDateTime(reader["pd_dateBeg"]),
                                            State = Convert.ToString(reader["pd_state"]),
                                            New = Convert.ToBoolean(reader["p_new"]),
                                            Quarantee = Convert.ToInt32(reader["dp_guarantee"]),
                                            CurId = Convert.ToInt32(reader["pd_curID"]),
                                            CurSign = Convert.ToString(reader["cur_sign"]),
                                            Price = Convert.ToDecimal(reader["pd_price"]),
                                            IdGroup = Convert.ToInt32(reader["p_grID"]),
                                            IdBrand = reader["p_brandID"] != DBNull.Value ? Convert.ToInt32(reader["p_brandID"]) : (int?)null
                                        };
                            prod.Add(d);
                        }
                    }
                }
                HttpContext.Current.Cache.Add(cacheKey, prod, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            return (List<ProductPricePresentation>)HttpContext.Current.Cache[cacheKey];
        }

        public static List<ProductPricePresentation> GetCachedProductsAdditionalByGroupAndDealer(this ZamovStorage context, int groupId, int dealerId)
        {
            string cacheKey = ProductsAdditionalByGroupAndDealerCachePrefix + groupId + "_" + dealerId;

            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                var prod = new List<ProductPricePresentation>();
                var parameters = new object[] { new SqlParameter("@grID", groupId), 
                                                new SqlParameter("@dealerId", dealerId) };

                DbCommand command = context.CreateStoreCommand("sp_ProductPricesByGroupAndDealer_Select", CommandType.StoredProcedure, parameters);

                using (context.Connection.CreateConnectionScope())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var d = new ProductPricePresentation
                            {
                                Id = Convert.ToInt32(reader["p_prodID"]),
                                IdDealerProd = Convert.ToInt32(reader["dpID"]),
                                dpName = Convert.ToString(reader["dp_prodName"]),
                                DealerName = Convert.ToString(reader["dealerName"]),
                                DealerId = Convert.ToInt32(reader["dp_dealerID"]),
                                PriceHrn = Convert.ToDecimal(reader["pd_price_hrn"]),
                                dpUnit = Convert.ToString(reader["dp_unit"]),
                                dpDescription = Convert.ToString(reader["dp_descr"]),
                                dpUrl = Convert.ToString(reader["dp_photoUrl"]),
                                DateBegin = Convert.ToDateTime(reader["pd_dateBeg"]),
                                State = Convert.ToString(reader["pd_state"]),
                                Quarantee = Convert.ToInt32(reader["dp_guarantee"]),
                                CurId = Convert.ToInt32(reader["pd_curID"]),
                                CurSign = Convert.ToString(reader["cur_sign"]),
                                Price = Convert.ToDecimal(reader["pd_price"]),
                                IdGroup = Convert.ToInt32(reader["dp_grID"]),
                                IdBrand = reader["dp_brandID"] != DBNull.Value ? Convert.ToInt32(reader["dp_brandID"]) : (int?)null,
                                dpEnable = Convert.ToBoolean(reader["dp_enable"]),
								IdProductByDealer = Convert.ToInt32(reader["pdID"])
                            };
                            prod.Add(d);
                        }
                    }
                }
                HttpContext.Current.Cache.Add(cacheKey, prod, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            return (List<ProductPricePresentation>)HttpContext.Current.Cache[cacheKey];
        }

        public static List<CategoryPresentation> GetCachedCategories(this ZamovStorage context, int cityId, string language)
        {
            string cacheKey = CategoriesCachePrefix + cityId + "_" + language;
            HttpContext.Current.Cache.ClearCategoriesCache();//
            if (HttpContext.Current.Cache[cacheKey] == null)
            {
                List<CategoryPresentation> categories = context.GetTranslatedCategories(language, true, cityId, false)
                        .Select(tc => new CategoryPresentation
                        {
                            Id = tc.Entity.Id,
                            Name = tc.Translation.Text,
                            ParentId = tc.Entity.Parent.Id,
                            Additional = tc.Entity.GroupsAdditional.Any(g => !g.gr_deleted && g.gr_enabled),
                            IndexNumber = tc.Entity.IndexNumber
                        })
                        .OrderBy(cp => cp.IndexNumber)
                        .ToList();
                categories.ForEach(c => c.PickChildren(categories));
                categories = categories.Where(c => c.ParentId == null).ToList();
                foreach (var catPresent in categories)
                {
                    if (catPresent.Additional)
                    {
                        var cpAddit = new List<CategoryPresentation>();
                        var param = new List<object>
                                {
                                    new SqlParameter("@catID", catPresent.Id),
                                    new SqlParameter("@cityID", cityId)
                                };
                        DbCommand command = context.CreateStoreCommand("sp_GroupsByCategoryRootAndCityAndHasProduct_Select", CommandType.StoredProcedure, param.ToArray());

                        using (context.Connection.CreateConnectionScope())
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var d = new CategoryPresentation
                                                {
                                                    Id = Convert.ToInt32(reader["grID"]),
                                                    Name = Convert.ToString(reader["gr_Name"]),
                                                    Additional = true,
                                                    IndexNumber = Convert.ToInt32(reader["gr_IndexNumber"])
                                                };
                                    string gName = context.Translations.Where(tr => tr.ItemId == d.Id && tr.Language == SystemSettings.CurrentLanguage &&
                                        tr.TranslationItemTypeId == (int)ItemTypes.GroupAdditional).Select(tr => tr.Text).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(gName))
                                    {
                                        d.Name = gName;
                                    }
                                    cpAddit.Add(d);
                                }
                            }
                        }
                        catPresent.SetChildrenAdditional(cpAddit);
                    }
                }
                categories = categories.Where(c => c.Children.Count > 0).ToList();
                HttpContext.Current.Cache.Add(cacheKey, categories, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            return (List<CategoryPresentation>)HttpContext.Current.Cache[cacheKey];
        }

        public static List<SelectListItem> GetCitiesFromContext(this ZamovStorage context, string language)
        {
            if (HttpContext.Current.Items[CitiesCachePrefix + language] == null)
            {
                HttpContext.Current.Items[CitiesCachePrefix + language] =
                     context.Cities.Where(c => c.Enabled).Join(context.Translations.Where(t => t.TranslationItemTypeId == (int)ItemTypes.City && t.Language == language),
                         c => c.Id, t => t.ItemId, (c, i) => new { Id = c.Id, Text = i.Text, index = c.IndexNumber }).ToList().OrderBy(c => c.index).Select(kvp => new SelectListItem { Value = kvp.Id.ToString(), Text = kvp.Text }).ToList();
            }
            return (List<SelectListItem>)HttpContext.Current.Items[CitiesCachePrefix + language];
        }
        
        public static Dictionary<string, Dictionary<string, string>> GetSeoDictionary(this SeoStorage context)
        {
            if (HttpContext.Current.Items[SeoDictionaryCachePrefix] == null)
            {
                var seoDictionary = new Dictionary<string, Dictionary<string, string>>();
                List<SeoDictionary> listSeoDict = context.SeoDictionary.ToList().OrderBy(sd => sd.SeoKey).ToList();
                
                foreach (SeoDictionary seo in listSeoDict)
                {
                    if (seoDictionary.ContainsKey(seo.SeoKey))
                    {
                        seoDictionary[seo.SeoKey].Add(seo.Language, seo.SeoValue);
                    }
                    else
                    {
                        seoDictionary.Add(seo.SeoKey, new Dictionary<string, string>{{seo.Language, seo.SeoValue}});
                    }
                }
                HttpContext.Current.Items[SeoDictionaryCachePrefix] = seoDictionary;
            }
            return (Dictionary<string, Dictionary<string, string>>)HttpContext.Current.Items[SeoDictionaryCachePrefix];
        }

        public static void ClearSeoDictionary(this Cache cache)
        {
            cache.ClearCache(key => key.StartsWith(SeoDictionaryCachePrefix));
        }

        public static void ClearCategoriesCache(this Cache cache)
        {
            cache.ClearCache(key => key.StartsWith(CategoriesCachePrefix));
        }

        public static void ClearProductAdditionalCache(this Cache cache)
        {
            cache.ClearCache(key => key.StartsWith(ProductsAdditionalByGroupAndDealerCachePrefix));
            cache.ClearCache(key => key.StartsWith(ProductsAdditionalByGroupAndCityCachePrefix));
        }

        public static void ClearCache(this Cache cache, Func<string, bool> cacheKeyCondition)
        {
            List<string> keysToClear = new List<string>();

            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (cacheKeyCondition(enumerator.Key.ToString()))
                    keysToClear.Add(enumerator.Key.ToString());
            }
            foreach (string key in keysToClear)
            {
                cache.Remove(key);
            }
        }
    }
}
