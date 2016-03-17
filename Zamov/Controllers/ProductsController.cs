using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Microsoft.Data.Extensions;
using Zamov.Models;
using System.Data.Objects;
using System.Data;
using Zamov.Helpers;
using System;
using System.Web.UI.WebControls;
using System.Web.Caching;

namespace Zamov.Controllers
{
    public class ProductsController : Controller
    {
        private const int MaxTopProductsNumber = 5;
        //TODO: тут пиздец с быстродейтвием
        public ActionResult Index(string dealerId, int? categoryId, int? groupId, SortFieldNames? sortField, SortDirection? sortOrder, int? productId)
        {
            HttpContext.Items["categoryId"] = categoryId;

            if (categoryId != null)
            {
                BreadCrumbAttribute.ProcessCategory(Convert.ToInt32(categoryId), HttpContext);
            }

            ViewData["sortDirection"] = sortOrder;
            ViewData["sortField"] = (sortField != null) ? sortField.ToString() : null;
            ViewData["sortDealerId"] = dealerId;
            string sortFieldPart = (sortField != null) ? sortField.ToString() : "NoSort";
            string sortOrderPart = (sortOrder != null) ? sortOrder.ToString() : "NoSort";
            string productsCacheKey = "ProductsPage_" + dealerId + "_{0}_" + sortFieldPart + "_" + sortOrderPart;
            using (ZamovStorage context = new ZamovStorage())
            {

                Dealer dealer = context.Dealers.Include("Cities").Where(d => d.Name == dealerId).Select(d => d).First();
                int dealer_Id = dealer.Id; //context.Dealers.Where(d => d.Name == dealerId).Select(d => d.Id).First();
                string dealerPhones = dealer.Phone;
                if (dealerPhones != null)
                    dealerPhones = dealerPhones.Trim().Replace("\r\n", "<br/>");

                BreadCrumbsExtensions.AddBreadCrumb(HttpContext, BreadCrumbAttribute.DealerName(dealer_Id), "/Products/" + dealerId + "/" + categoryId);

                List<Group> groups = (from g in context.Groups.Include("Groups").Include("Dealer").Include("Category") where g.Dealer.Id == dealer_Id && !g.Deleted && g.Enabled select g).ToList();
                ViewData["groups"] = groups;
                ViewData["dealerId"] = dealer_Id;
                ViewData["dealerPhones"] = dealerPhones;
                ViewData["banner"] = dealer.Banner;
                ViewData["bannerEnabled"] = dealer.BannerEnabled;

                ViewData["groupId"] = groupId;
                ViewData["categoryId"] = categoryId;
                

                bool displayGroupImages = false;

                Group currentGroup = null;
                if (groupId != null)
                {
                    currentGroup = groups.Where(g => g.Id == groupId.Value).Select(g => g).SingleOrDefault();
                    displayGroupImages = currentGroup.DisplayProductImages;
                }
                else
                {
                    Category category = context.Categories.Include("Categories").Where(c => c.Id == categoryId).Select(c => c).First();
                    currentGroup = category.Groups.Where(g => g.Dealer.Id == dealer_Id).Select(g => g).FirstOrDefault();

                    if (currentGroup == null)
                        currentGroup = category.Categories.SelectMany(c => c.Groups).Where(g => g.Dealer.Id == dealer_Id).First();

                    ViewData["groupId"] = currentGroup.Id;

                    displayGroupImages = currentGroup.DisplayProductImages;
                }

                productsCacheKey = string.Format(productsCacheKey, currentGroup.Id);

                BreadCrumbAttribute.ProcessGroup(currentGroup.Id, HttpContext);

                ViewData["displayGroupImages"] = displayGroupImages;

                //seo---
                int dealerType = (int)ItemTypes.DealerName;

                Translation dealerNameCurLang = context.Translations
                    .Where(tr => tr.Language == SystemSettings.CurrentLanguage)
                    .Where(tr => tr.ItemId == dealer.Id)
                    .Where(tr => tr.TranslationItemTypeId == dealerType)
                    .FirstOrDefault();
                
                int groupType = (int)ItemTypes.Group;

                Translation groupNameCurLang = context.Translations
                    .Where(tr => tr.Language == SystemSettings.CurrentLanguage)
                    .Where(tr => tr.ItemId == currentGroup.Id)
                    .Where(tr => tr.TranslationItemTypeId == groupType)
                    .FirstOrDefault();

                SeoMetaTags seoMetaTags = new SeoMetaTags();
                seoMetaTags.Title = dealerNameCurLang != null ? dealerNameCurLang.Text : dealer.Name;
                seoMetaTags.AdditionInfo = groupNameCurLang != null ? groupNameCurLang.Text : currentGroup.Name;
                if (SystemSettings.CityId < 1)
                {
                    SystemSettings.CityId = dealer.Cities.Select(c => c.Id).FirstOrDefault();
                }
                SeoHelper.AddInfo(SeoEntityType.Dealers, dealer_Id, seoMetaTags);
                //----

                if (HttpContext.Cache[productsCacheKey] == null)
                //if (1==1)
                {
                    List<Product> products = new List<Product>();
                    if (currentGroup != null)
                    {
                        CollectProducts(products, currentGroup);
                        products = products.Where(p => !p.Deleted && p.Group.Enabled && p.Enabled && !p.Group.Deleted).ToList();
                    }
                    else
                        products =
                            (from product in context.Products
                             where ((groupId == null) || product.Group.Id == groupId)
                             && product.Dealer.Id == dealer_Id
                             && !product.Deleted
                             && product.Group.Enabled
                             && !product.Group.Deleted
                             && product.Enabled
                             select product)
                             .ToList();


                    products.ForEach(p => p.LoadDescriptions());
                    products.ForEach(p => p.CurrenciesReference.Load());
                    products.ForEach(p => p.LoadProductRate());

                    if (sortField != null && sortOrder != null)
                        switch (sortField)
                        {
                            case SortFieldNames.Name:
                                if (sortOrder == SortDirection.Ascending)
                                    products.Sort(new PSortByProductNameAsc());
                                else
                                    products.Sort(new PSortByProductNameDesc());
                                break;
                            case SortFieldNames.Price:
                                if (sortOrder == SortDirection.Ascending)
                                    products.Sort(new PSortByPriceAsc());
                                else
                                    products.Sort(new PSortByPriceDesc());
                                break;
                        }
                    HttpContext.Cache.Add(productsCacheKey, products, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 30, 0), CacheItemPriority.Default, null);
                }
            }

            List<Product> result = (List<Product>)HttpContext.Cache[productsCacheKey];
            ViewData["topProducts"] = GetTopProducts(result);
            result = RemoveTopProducts(result);

            return View(result);
        }

        private List<Product> GetTopProducts(List<Product> source)
        {
            List<Product> allTops = source.Where(p => p.TopProduct).Select(p => p).ToList();

            if (allTops.Count < MaxTopProductsNumber + 1)
                return allTops;

            List<Product> result = new List<Product>();
            Random random = new Random();

            int topCount = allTops.Count;
            for (int i = 0; i < MaxTopProductsNumber; i++)
            {
                Product item = allTops[random.Next(topCount)];
                result.Add(item);
                allTops.Remove(item);
                topCount--;
            }
            return result;
        }

        private List<Product> RemoveTopProducts(List<Product> source)
        {
            return source.Where(p => !p.TopProduct).Select(p => p).ToList();
        }

        [OutputCache(VaryByParam = "*", Duration = 1, NoStore = true)]
        public ActionResult ProductGroups(string dealerId, int categoryId, int? groupId)
        {
            List<GroupResentation> groups = null;
            using (ZamovStorage context = new ZamovStorage())
            {
                groups = (from gr in context.Groups
                          join translation in context.Translations on gr.Id equals translation.ItemId
                          where translation.TranslationItemTypeId == (int)ItemTypes.Group
                             && translation.Language == SystemSettings.CurrentLanguage
                             && gr.Enabled
                             && !gr.Deleted
                             && gr.Dealer.Name == dealerId
                          select new GroupResentation
                              {
                                  Id = gr.Id,
                                  Name = translation.Text,
                                  ParentId = (gr.Parent != null) ? (int?)gr.Parent.Id : null,
                                  DealerName = gr.Dealer.Name,
                                  CategoryId = (gr.Category != null) ? gr.Category.Id : 0
                              }
                              ).ToList();
            }
            int groupToExpand = int.MinValue;
            groups.ForEach(ac => { ac.PickChildren(groups); ac.PickParent(groups); });
            if (groupId != null)
            {
                GroupResentation currentGroup = groups.Where(g => g.Id == groupId.Value).SingleOrDefault();
                while (currentGroup != null)
                {
                    groupToExpand = currentGroup.Id;
                    currentGroup = currentGroup.Parent;
                }
            }
            ViewData["groupToExpand"] = groupToExpand;
            ViewData["dealerId"] = dealerId;
            ViewData["groupId"] = groupId;
            List<GroupResentation> sortedGroups = groups.Select(c => c).Where(c => c.ParentId == null).ToList();
            return View(sortedGroups);
        }

        [OutputCache(VaryByParam = "*", Duration = 1, NoStore = true)]
        public ActionResult ProductGroupsNew(int idCity, int? groupId)
        {
            //string dealerId, int categoryId, int? groupId
            var groups = new List<GroupResentation>();
            using (ZamovStorage context = new ZamovStorage())
            {
                int categoryId = groupId.HasValue ? context.GroupsAdditional.Where(ga => ga.grID == groupId).Select(ga => ga.Categories.Id).FirstOrDefault() : -1;

                object param = new SqlParameter("@catID", categoryId);

                DbCommand command = context.CreateStoreCommand("sp_Groups_Select", CommandType.StoredProcedure, param);

                using (context.Connection.CreateConnectionScope())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var g = new GroupResentation
                                        {
                                            Id = Convert.ToInt32(reader["grID"]),
                                            Name = Convert.ToString(reader["gr_Name"]),
                                            ParentId = reader["gr_ParID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["gr_ParID"]) : null,
                                            CategoryId = Convert.ToInt32(reader["gr_CatID"]),
                                        };
                            string gName = context.Translations.Where(tr => tr.ItemId == g.Id && tr.Language == SystemSettings.CurrentLanguage &&
                                          tr.TranslationItemTypeId == (int) ItemTypes.GroupAdditional).Select(tr => tr.Text).FirstOrDefault();
                            if (!string.IsNullOrEmpty(gName))
                            {
                                g.Name = gName;
                            }
                            groups.Add(g);
                        }
                    }
                }
            }
            int groupToExpand = int.MinValue;
            groups.ForEach(ac => { ac.PickChildren(groups); ac.PickParent(groups); });
            ViewData["groupId"] = groupId;
            SystemSettings.CityId = idCity;
            List<GroupResentation> sortedGroups = groups.Select(c => c).Where(c => c.ParentId == null).ToList();
            
            return View("ProductGroupTreeView", sortedGroups);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddToCart(int dealerId, int categoryId, int? groupId, FormCollection items)
        {
            string dealerName = "";
            using (ZamovStorage context = new ZamovStorage())
            {
                dealerName = context.Dealers.Where(d => d.Id == dealerId).Select(d => d.Name).FirstOrDefault();
            }

            PostData orderItems = items.ProcessPostData("dealerId", "groupId", "categoryId");
            if (orderItems.Count > 0)
            {
                var orderItemList =
                    (from oi in orderItems
                     where oi.Value["order"].ToLowerInvariant().Contains("true")
                     select new OrderItemCart { 
                         ProductId = int.Parse(oi.Key),
                         DealerId = dealerId,
                         Quantity = int.Parse(oi.Value["quantity"].Replace("_", string.Empty)) 
                     })
                     .ToList();

                OrderHelper.AddToCart(orderItemList, ProductVersion.First);

            }
            return RedirectToAction("Index", new { dealerId = dealerName, categoryId = categoryId, groupId = groupId });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddToCartForProductAdditional(int productId, FormCollection items)
        {
            PostData orderItems = items.ProcessPostData("productId", "dealerId", "groupId", "categoryId");
            if (orderItems.Count > 0)
            {
                var orderItemList =
                    (from oi in orderItems
                     where oi.Value["order"].ToLowerInvariant().Contains("true")
                     select new OrderItemCart
                     {
                         ProductId = int.Parse(oi.Key),
                         Quantity = int.Parse(oi.Value["quantity"].Replace("_", string.Empty)),
                         DealerId = int.Parse(oi.Value["dealer"].Replace("_", string.Empty))
                     })
                     .ToList();

                OrderHelper.AddToCart(orderItemList, ProductVersion.Additional);
            }
            return RedirectToAction("ShowProductPrices", new { idCity = SystemSettings.CityId, Id = productId });
        }

        private void CollectProducts(List<Product> products, Group currentGroup)
        {
            if (!currentGroup.Products.IsLoaded)
                currentGroup.Products.Load();
            products.AddRange(currentGroup.Products);
            if (!currentGroup.Groups.IsLoaded)
                currentGroup.Groups.Load();
            if (currentGroup.Groups.Count > 0)
                foreach (var g in currentGroup.Groups)
                    CollectProducts(products, g);
        }

        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult Description(int id, int vers)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                if (vers == (int)ProductVersion.First)
                {
                    ProductPresentation product = (from item in context.Products.Include("ProductImages")
                                                   where
                                                       item.Id == id
                                                   let hasImage = item.ProductImages.Count > 0
                                                   let imageId = (hasImage) ? item.ProductImages.FirstOrDefault().Id : 0
                                                   let description =
                                                       (from d in context.Translations
                                                        where d.Language == SystemSettings.CurrentLanguage
                                                              &&
                                                              d.TranslationItemTypeId ==
                                                              (int)ItemTypes.ProductDescription
                                                              && d.ItemId == item.Id
                                                        select d.Text).FirstOrDefault()
                                                   select new ProductPresentation
                                                              {
                                                                  Name = item.Name,
                                                                  Description = description,
                                                                  HasImage = hasImage,
                                                                  ImageId = imageId,
                                                                  Id = item.Id
                                                              }
                              ).First();
                    return View(product);
                }
                else if (vers == (int)ProductVersion.Additional)
                {
                    ProductByGroupPresent product =
                        (from item in context.ProductsAdditional
                         where item.p_prodID == id
                         select new ProductByGroupPresent
                                    {
                                        Name = item.p_prodName,
                                        Description = item.p_descr,
                                        Url = item.p_photo_url
                                    }).First();
                    return View("DescriptionAdditional", product);
                }
                return null;
            }
        }

        public ActionResult LeftMenu(int idCity, List<CategoryPresentation> categories, int idCurrentCategory)
        {
            ViewData["IdCurrentCategory"] = idCurrentCategory;
            ViewData["idCity"] = idCity;
            return View(categories);
        }

        #region ShowProduct

        public ActionResult ShowProductPrices(int idCity, int? id, int page, SortFieldNames? sortField, SortDirection? sortOrder)
        {
            SystemSettings.CityId = idCity;

            if (id.HasValue)
            {
                BreadCrumbAttribute.ProcessProductAdditional(idCity, id.Value, HttpContext);
            }
            else
            {
                id = -1;   
            }

            var products = new List<ProductPricePresentation>();
            using (var context = new ZamovStorage())
            {
                var z = context.ProductsAdditional.Include("GroupsAdditional").Where(gr => gr.p_prodID == id).
                    Select(gr => new { categId = gr.GroupsAdditional.Categories.Id, gropId = gr.GroupsAdditional.grID }).ToArray().FirstOrDefault();
                HttpContext.Items["categoryId"] = z.categId;
                HttpContext.Items["productVersion"] = ProductVersion.Additional;

                products = context.GetCachedProductsAdditional(z.gropId, idCity);

                products = products.Where(p => p.Id == id).Select(p => p).ToList();

                List<CategoryPresentation> categories = context.GetCachedCategories(idCity, SystemSettings.CurrentLanguage);

                ViewData["categories"] = categories;

                SeoMetaTags seoMetaTags = new SeoMetaTags();
                seoMetaTags.Title = products.FirstOrDefault().Name;
                SeoHelper.AddInfo(SeoEntityType.Product, id.Value, seoMetaTags);
                //IEnumerable<CategoryPresentation> flattenCategories = categories.SelectMany(c => c.Children).Union(categories);
            }

            if (sortField != null && sortOrder != null)
                switch (sortField)
                {
                    case SortFieldNames.Name:
                        if (sortOrder == SortDirection.Ascending)
                            products.Sort(new SortProductPricePresentationByNameAsc());
                        else
                            products.Sort(new SortProductPricePresentationByNameDesc());
                        break;
                    case SortFieldNames.Price:
                        if (sortOrder == SortDirection.Ascending)
                            products.Sort(new SortProductPricePresentationByPriceAsc());
                        else
                            products.Sort(new SortProductPricePresentationByPriceDesc());
                        break;
                    case SortFieldNames.Dealer:
                        if (sortOrder == SortDirection.Ascending)
                            products.Sort(new SortProductPricePresentationByDealerNameAsc());
                        else
                            products.Sort(new SortProductPricePresentationByDealerNameDesc());
                        break;
                }

            ViewData["sortDirection"] = sortOrder;
            ViewData["sortField"] = (sortField != null) ? sortField.ToString() : null;

            int pageSize = ApplicationData.PageSize;
            int countProdPrice = products.Count;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)countProdPrice / pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["productId"] = id;
            return View(products.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public ActionResult ShowProductsCategory(int idCity, int id, int page, SortFieldNames? sortField, SortDirection? sortOrder)
        {
            SystemSettings.CityId = idCity;

            bool onlyNewProductsForShowCategory = true;

            HttpContext.Items["categoryId"] = id;
            BreadCrumbAttribute.ProcessCategory(id, HttpContext);

            var productsId = new List<int>();
            using (ZamovStorage context = new ZamovStorage())
            {
                object param = new SqlParameter("@catID", id);
                DbCommand command = context.CreateStoreCommand("sp_GroupsByCategoryRoot_Select", CommandType.StoredProcedure, param);

                using (context.Connection.CreateConnectionScope())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productsId.Add(Convert.ToInt32(reader["grID"]));
                        }
                    }
                }
                List<CategoryPresentation> categories = context.GetCachedCategories(idCity, SystemSettings.CurrentLanguage);
                ViewData["categories"] = categories;
                ViewData["IdCurrentCategory"] = id;

                SeoMetaTags seoMetaTags = new SeoMetaTags();
                seoMetaTags.Title = categories.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefault();
                SeoHelper.AddInfo(SeoEntityType.Categories, id, seoMetaTags);
            }

            var productDistinct = new List<ProductByGroupPresent>();
            foreach (var prodId in productsId)
            {
                productDistinct.AddRange(GetListProductByGroupPresent(idCity, prodId, onlyNewProductsForShowCategory, sortField, sortOrder));
            }
            int pageSize = ApplicationData.PageSize;
            int countProdPrice = productDistinct.Count;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)countProdPrice / pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["onlyNewProductsForShowCategory"] = onlyNewProductsForShowCategory;
            ViewData["productDistinct"] = productDistinct;
            return View("ShowProducts", productDistinct.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        private List<ProductByGroupPresent> GetListProductByGroupPresent(int idCity, int id, bool onlyNewProducts, SortFieldNames? sortField, SortDirection? sortOrder)
        {
            ViewData["sortDirection"] = sortOrder;
            ViewData["sortField"] = (sortField != null) ? sortField.ToString() : null;

            var productDistinct = new List<ProductByGroupPresent>();

            using (ZamovStorage context = new ZamovStorage())
            {
                int categoryId = context.GroupsAdditional.Where(gr => gr.grID == id).Select(gr => gr.Categories.Id).ToArray().FirstOrDefault();
                HttpContext.Items["categoryId"] = categoryId;
                HttpContext.Items["productVersion"] = ProductVersion.Additional;

                List<int> brandsId = null;
                string brandsIds = "";
                if (Session["FilterBrands"] != null)
                {
                    var dictBrands = (Dictionary<int, List<Brands>>) Session["FilterBrands"];
                    if (dictBrands.ContainsKey(id))
                    {
                        List<Brands> brands = dictBrands[id];
                        brandsId = brands.Where(b => b.IsCheck).Select(b => b.brandID).ToList();
                        List<string> br = brandsId.Select(b => b.ToString()).ToList();
                        if (br.Count > 0)
                        {
                            brandsIds = String.Join(",", br.ToArray());
                        }
                    }
                }
                ProductPriceRangePresent productPriceRange = null;
                if (Session["FilterProductPriceRange"] != null)
                {
                    var dictProdPriceRange = (Dictionary<int, Dictionary<string, ProductPriceRangePresent>>)Session["FilterProductPriceRange"];
                    if (dictProdPriceRange.ContainsKey(id))
                    {
                        if (dictProdPriceRange[id].ContainsKey(brandsIds))
                        {
                            productPriceRange = dictProdPriceRange[id][brandsIds];
                        }
                    }
                }

                List<ProductPricePresentation> product = context.GetCachedProductsAdditional(id, idCity);

                if (brandsId != null && brandsId.Count > 0)
                {
                    product = (from p in product where brandsId.Contains(p.IdBrand.HasValue ? p.IdBrand.Value : -1) select p).ToList();
                }

                if (productPriceRange != null)
                {
                    List<ProductPriceRange> listProductPriceRange = productPriceRange.ListProductPriceRange.Where(ppr => ppr.IsCheck).Select(ppr => ppr).ToList();
                    if (productPriceRange.CustomProductPriceRange.PriceFrom > 0 || productPriceRange.CustomProductPriceRange.PriceTo > 0)
                    {
                        listProductPriceRange.Add(productPriceRange.CustomProductPriceRange);
                    }

                    if (listProductPriceRange.Count > 0)
                    {
                        product = product.Where(p => p.CheckBetween(listProductPriceRange)).Select(p => p).ToList();
                    }
                }

                if (onlyNewProducts)
                {
                    product = product.Where(p => p.New).Select(p => p).ToList();
                }

                productDistinct = ( from p in product 
                                    group p by p.Id into t
                                    select new ProductByGroupPresent
                                    {
                                        Id = t.First().Id,
                                        Name = t.First().Name,
                                        Action = t.First().Action,
                                        Unit = t.First().Unit,
                                        Description = t.First().Description,
                                        Url = t.First().Url,
                                        New = t.First().New,
                                        IdBrand = t.First().IdBrand,
                                        PriceMin = t.Min(p => p.PriceHrn),
                                        PriceAvg = t.Average(p => p.PriceHrn),
                                        PriceMax = t.Max(p => p.PriceHrn),
                                        Proposals = t.Count(),
                                        Prices = t.Select(p => p.PriceHrn).ToList()
                                    }).ToList();

                if (sortField != null && sortOrder != null)
                    switch (sortField)
                    {
                        case SortFieldNames.Name:
                            if (sortOrder == SortDirection.Ascending)
                                productDistinct.Sort(new SortProductByGroupPresentByNameAsc());
                            else
                                productDistinct.Sort(new SortProductByGroupPresentByNameDesc());
                            break;
                        case SortFieldNames.Price:
                            if (sortOrder == SortDirection.Ascending)
                                productDistinct.Sort(new SortProductByGroupPresentByPriceAsc());
                            else
                                productDistinct.Sort(new SortProductByGroupPresentByPriceDesc());
                            break;
                    }
            }

            return productDistinct;
        }

        public ActionResult ShowProducts(int idCity, int? id, int page, SortFieldNames? sortField, SortDirection? sortOrder)
        {
            SystemSettings.CityId = idCity;

            var productDistinct = new List<ProductByGroupPresent>();

            if (id.HasValue)
            {
                BreadCrumbAttribute.ProcessGroupProduct(idCity, id.Value, HttpContext);

                productDistinct = GetListProductByGroupPresent(idCity, id.Value, false, sortField, sortOrder);
                
                using (ZamovStorage context = new ZamovStorage())
                {
                    string gName = context.Translations.Where(tr => tr.ItemId == id && tr.Language == SystemSettings.CurrentLanguage &&
                                  tr.TranslationItemTypeId == (int)ItemTypes.GroupAdditional).Select(tr => tr.Text).FirstOrDefault();

                    if (String.IsNullOrEmpty(gName))
                    {
                        gName = context.GroupsAdditional.Where(ga => ga.grID == id).Select(ga => ga.gr_Name).FirstOrDefault();
                    }
                    SeoMetaTags seoMetaTags = new SeoMetaTags();
                    seoMetaTags.Title = gName;
                    SeoHelper.AddInfo(SeoEntityType.Groups, id.Value, seoMetaTags);
                }
            }

            int pageSize = ApplicationData.PageSize;
            int countProdPrice = productDistinct.Count;
            int totalPages = (int)Math.Ceiling((double)countProdPrice / pageSize);
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;
            ViewData["groupId"] = id;
            ViewData["productDistinct"] = productDistinct;
            ViewData["idCity"] = idCity;

            return View(productDistinct.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public ActionResult Brands(int? groupId, List<ProductByGroupPresent> brandsContext)
        {
			ViewData["groupId"] = groupId;

            if (!Request.IsAjaxRequest())
			{
				if (Session["FilterBrands"] != null)
				{
					var dictBrands = (Dictionary<int, List<Brands>>) Session["FilterBrands"];
					int index = groupId.HasValue ? groupId.Value : -1;
					if (dictBrands.ContainsKey(index))
					{
                        dictBrands[index].ForEach(br => br.BrandCount = (from pa in brandsContext where pa.IdBrand == br.brandID select pa.Proposals).Sum());
                        return View(dictBrands[index]);
					}
				}
				using (ZamovStorage context = new ZamovStorage())
				{
                    List<Brands> brands;
                    if (groupId.HasValue)
                    {
                        int categoryId = context.GroupsAdditional.Where(ga => ga.grID == groupId).Select(ga => ga.Categories.Id).FirstOrDefault();
                        brands = (from br in context.Brands
                                               join pa in context.ProductsAdditional on br.brandID equals pa.p_brandID
                                               join ga in context.GroupsAdditional on pa.GroupsAdditional.grID equals ga.grID
                                               where ga.Categories.Id == categoryId
                                               select br).Distinct().OrderBy(br => br.brandName).Take(10).ToList();
                    }
                    else
                    {
                        brands = (from br in context.Brands
                                  join pa in context.ProductsAdditional on br.brandID equals pa.p_brandID
                                  select br).Distinct().OrderBy(br => br.brandName).Take(10).ToList();                        
                    }
				    brands.ForEach(br => br.BrandCount = (from pa in brandsContext where pa.IdBrand == br.brandID select pa.Proposals).Sum());
				    brands = brands.Where(b => b.BrandCount > 0).ToList();
                    return View(brands);
                }
			}

			// Ajax Request
			using (ZamovStorage context = new ZamovStorage())
			{
			    List<Brands> brands;
                if (groupId.HasValue)
                {
                    int categoryId = context.GroupsAdditional.Where(ga => ga.grID == groupId).Select(ga => ga.Categories.Id).FirstOrDefault();
                    brands = (from br in context.Brands
                              join pa in context.ProductsAdditional on br.brandID equals pa.p_brandID
                              join ga in context.GroupsAdditional on pa.GroupsAdditional.grID equals ga.grID
                              where ga.Categories.Id == categoryId
                              select br).Distinct().OrderBy(br => br.brandName).Skip(10).ToList();
                }
                else
                {
                    brands = (from br in context.Brands
                              join pa in context.ProductsAdditional on br.brandID equals pa.p_brandID
                              select br).Distinct().OrderBy(br => br.brandName).Skip(10).ToList();                    
                }
			    brands.ForEach(br => br.BrandCount = (from pa in brandsContext where pa.IdBrand == br.brandID select pa.Proposals).Sum());
                brands = brands.Where(b => b.BrandCount > 0).ToList();
				return PartialView("BrandsPart", brands);
			}
        }

		//protected string RenderViewToString<T>(string viewPath, T model)
		//{
		//    ViewData.Model = model;
		//    using (var writer = new StringWriter())
		//    {
		//        var view = new WebFormView(viewPath);
		//        var vdd = new ViewDataDictionary<T>(model);
		//        var viewCxt = new ViewContext(ControllerContext, view, vdd, new TempDataDictionary(), writer);
		//        viewCxt.View.Render(viewCxt, writer);
		//        return writer.ToString();
		//    }
		//}

        [HttpPost]
        public ActionResult Filters(int idCity, int? groupId, List<Brands> brands, ProductPriceRangePresent prodPriceRange, FormCollection form)
        {
            if (form["Clear"] == null)
            {
                Session["FilterBrands"] = new Dictionary<int, List<Brands>> { { groupId.HasValue ? groupId.Value : -1, brands } };
                List<string> brandsId = brands.Where(b => b.IsCheck).Select(b => b.brandID.ToString()).ToList();
                string brandsIds = String.Join(",", brandsId.ToArray());
                Session["FilterProductPriceRange"] =
                    new Dictionary<int, Dictionary<string, ProductPriceRangePresent>> { { groupId.HasValue ? groupId.Value : -1, 
                        new Dictionary<string, ProductPriceRangePresent>{ {brandsIds, prodPriceRange} } } };
            }
            else
            {
                Session["FilterBrands"] = null;
                Session["FilterProductPriceRange"] = null;
            }
            return RedirectToAction("ShowProducts", new { idCity = idCity, id = groupId, page = 1 });
        }

        public ActionResult ProductPriceRange(int idCity, int? groupId, List<ProductByGroupPresent> productDistinct)
        {
            int index = groupId.HasValue ? groupId.Value : -1;
            if (Session["FilterProductPriceRange"] != null)
            {
                var dictProdPriceRange = (Dictionary<int, Dictionary<string, ProductPriceRangePresent>>)Session["FilterProductPriceRange"];

                if (dictProdPriceRange.ContainsKey(index))
                {
                    string brandsIds = "";
                    if (Session["FilterBrands"] != null)
                    {
                        var brands = (Dictionary<int, List<Brands>>)Session["FilterBrands"];
                        if (brands.ContainsKey(index))
                        {
                            List<string> brandsId = brands[index].Where(b => b.IsCheck).Select(b => b.brandID.ToString()).ToList();
                            brandsIds = String.Join(",", brandsId.ToArray());
                        }
                    }
                    if (dictProdPriceRange[index].ContainsKey(brandsIds))
                    {
                        ProductPriceRangePresent priceRangePresent = dictProdPriceRange[index][brandsIds];
                        priceRangePresent.ListProductPriceRange.ForEach(p => p.CheckBetween(productDistinct));
                        return View(priceRangePresent);
                    }
                }
            }

            var prodPriceRange = new ProductPriceRangePresent();
            using (ZamovStorage context = new ZamovStorage())
            {
                var param = new List<object>
                                {
                                    new SqlParameter("@grID", groupId.HasValue ? groupId.Value : -1),
                                    new SqlParameter("@cityId", idCity)
                                };

                string brandsIds = "";
                if (Session["FilterBrands"] != null)
                {
                    var dictBrands = (Dictionary<int, List<Brands>>)Session["FilterBrands"];
                    if (dictBrands.ContainsKey(index))
                    {
                        var brands = dictBrands[index];
                        List<string> brandsId = brands.Where(b => b.IsCheck).Select(b => b.brandID.ToString()).ToList();
                        if (brandsId.Count > 0)
                        {
                            brandsIds = String.Join(",", brandsId.ToArray());
                            param.Add(new SqlParameter("@brands", brandsIds));
                        }
                    }
                }

                DbCommand command = context.CreateStoreCommand("sp_ProductPricesDiapazByGroupID", CommandType.StoredProcedure, param.ToArray());

                using (context.Connection.CreateConnectionScope())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
							if (!(reader["pr1"] == DBNull.Value || reader["pr2"] == DBNull.Value))
							{
								var ppr = new ProductPriceRange
								          	{
								          		PriceFrom = Convert.ToDecimal(reader["pr1"]),
								          		PriceTo = Convert.ToDecimal(reader["pr2"]),
								          		Count = Convert.ToInt32(reader["cnt"])
								          	};
								prodPriceRange.ListProductPriceRange.Add(ppr);
							}
                        }
                    }
                }
            }

            return View(prodPriceRange);
        }

        #endregion
    }
}