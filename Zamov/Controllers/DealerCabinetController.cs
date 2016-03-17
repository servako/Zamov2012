using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Xml;
using System.Xml.Linq;
using Excel;
using Zamov.Models;
using System.Web.Security;
using System.IO;
using System.Web.Script.Serialization;
using Zamov.Helpers;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Objects;
using System.Globalization;
using Microsoft.Data.Extensions;

namespace Zamov.Controllers
{
    [Authorize(Roles = "Administrators, Dealers, Managers")]
    [BreadCrumb(ResourceName = "DealerCabinet", Url = "/DealerCabinet")]
    [UpdateCurrentDealer]
    public class DealerCabinetController : Controller
    {
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult Index()
        {
            return View();
        }

        #region Currency rates

        [Authorize(Roles = "Administrators, Dealers")]
        [BreadCrumb(ResourceName = "CurrencyRates", Url = "/DealerCabinet/CurrencyRates")]
        public ActionResult CurrencyRates()
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                List<Currencies> currencies = context.Currencies.Select(c => c).ToList();
                List<SelectListItem> currencysList = new List<SelectListItem>();
                foreach (var item in currencies)
                {
                    currencysList.Add(new SelectListItem { Text = item.Name + " [" + item.ShortName + "]", Value = item.Id.ToString() });
                }
                ViewData["currencies"] = currencysList;

                int dealerId = Security.GetCurentDealerId(User.Identity.Name);
				List<ExchangeRate> dealerCurrencyRates = (from dcr in context.ExchangeRate.Include("Currencies").Include("Dealers") where dcr.Dealers.Id == dealerId select dcr).ToList();
                return View(dealerCurrencyRates);
            }
        }

        [Authorize(Roles = "Administrators, Dealers")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertCurrencyRate(FormCollection form)
        {
            int currencyId = 0;
            decimal rate = 0;
			DateTime dateBegin = DateTime.Now;

            int dealerId = Security.GetCurentDealerId(User.Identity.Name);

        	DateTime.TryParse(form["date"], out dateBegin);
            if (int.TryParse(form["currencysList"], out currencyId))
                if (decimal.TryParse(form["rate"].Replace(".", ","), out rate))
                {
                    using (ZamovStorage context = new ZamovStorage())
                    {
                        Dealer currentDealer = context.Dealers.Select(d => d).Where(d => d.Id == dealerId).FirstOrDefault();
                        Currencies currentCurrency = context.Currencies.Select(c => c).Where(c => c.Id == currencyId).FirstOrDefault();
						ExchangeRate dealerExchangeRate = new ExchangeRate();
                        dealerExchangeRate.Dealers = currentDealer;
                        dealerExchangeRate.Currencies = currentCurrency;
                        dealerExchangeRate.xr_value = rate;
						dealerExchangeRate.xr_dateBeg = dateBegin;
                        context.AddToExchangeRate(dealerExchangeRate);
                        context.SaveChanges();
                    }
                }
            return RedirectToAction("CurrencyRates");
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UpdateCurrencyRates(FormCollection form)
        {
            PostData postData = form.ProcessPostData();
            PostData updates = new PostData();
            foreach (var item in postData)
            {
                updates.Add(item.Key, item.Value.ToDictionary(v => v.Key, v => v.Value));
            }
            int dealerId = Security.GetCurentDealerId(User.Identity.Name);
            foreach (var item in updates)
            {
                item.Value["rate"] = item.Value["rate"].Replace(".", ",");
            }

            using (ZamovStorage context = new ZamovStorage())
            {
                List<ExchangeRate> exchangeRates = (from dcr in context.ExchangeRate.Include("Currencies").Include("Dealers") 
                                                    where dcr.Dealers.Id == dealerId select dcr).ToList();
																	 //Include("Currencies").Include("Dealers") where dcr.DealerId == dealerId select dcr).ToList();
                foreach (var item in exchangeRates)
                {
                    item.xr_value = Convert.ToDecimal(updates[item.xrID.ToString()]["rate"]);
                    item.xr_dateBeg = Convert.ToDateTime(updates[item.xrID.ToString()]["date"]);
                }
                context.SaveChanges();
            }
            return RedirectToAction("CurrencyRates");
        }

        #endregion

        #region Groups
        [BreadCrumb(ResourceName = "Groups", Url = "/DealerCabinet/Groups")]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult Groups()
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                int dealerId = int.MinValue;
                dealerId = Security.GetCurentDealerId(User.Identity.Name);
                ViewData["dealerId"] = dealerId;
                List<CategoryPresentation> categories = context.GetTranslatedCategories(SystemSettings.CurrentLanguage, true, null, false)
                    .Select(c => new CategoryPresentation
                    {
                        Id = c.Entity.Id,
                        Name = c.Translation.Text,
                        ParentId = c.Entity.Parent.Id
                    }).ToList();
                categories.ForEach(c => c.PickChildren(categories));
                categories = categories.Where(c => c.ParentId == null).ToList();
                ViewData["categories"] = categories;
                return View();
            }
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult GoupList(int dealerId, int? id, int level, List<CategoryPresentation> categories)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                dealerId = Security.GetCurentDealerId(User.Identity.Name);
                ViewData["dealerId"] = dealerId;
                List<Group> groups = context.Groups.Select(g => g).Where(g => g.Dealer.Id == dealerId && !g.Deleted && g.Name != "TRASH").ToList();
                if (id == null)
                    groups = groups.Select(g => g).Where(g => g.Parent == null).ToList();
                else
                    groups = groups.Select(g => g).Where(g => g.Parent != null && g.Parent.Id == id.Value).ToList();
                ViewData["level"] = level;
                ViewData["categories"] = categories;
                return View(groups);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult InsertGroup(string groupName, string groupUkrName, string groupRusName, bool displayImages, bool enabled, int parentId, int categoryId)
        {
            ClearGroupCache();

            using (ZamovStorage context = new ZamovStorage())
            {
                int dealerId = Security.GetCurentDealerId(User.Identity.Name);
                Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == dealerId).First();
                Group parent = null;
                if (parentId >= 0)
                    parent = context.Groups.Select(c => c).Where(c => c.Id == parentId).First();
                Group group = new Group();
                group.Parent = parent;
                group.Dealer = dealer;
                group.Name = groupUkrName;
                group.Names.Clear();
                group.Names["ru-RU"] = groupRusName;
                group.Names["uk-UA"] = groupUkrName;
                group.Enabled = enabled;
                group.DisplayProductImages = displayImages;
                if (parentId < 0)
                    group.CategoryReference.EntityKey = new EntityKey("ZamovStorage.Categories", "Id", categoryId);
                context.AddToGroups(group);
                context.SaveChanges();
                context.UpdateTranslations(group.NamesXml);
            }
            return RedirectToAction("Groups");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UpdateGroups(FormCollection form)
        {
            ClearGroupCache();

            Dictionary<string, Dictionary<string, string>> updates = form.ProcessPostData();
            List<TranslationItem> translationItems = new List<TranslationItem>();
            using (ZamovStorage context = new ZamovStorage())
            {
                foreach (string key in updates.Keys)
                {
                    int itemId = int.Parse(key);
                    Dictionary<string, string> translations = (from item in updates[key] where item.Key != "itemId" && item.Key != "enabled" && item.Key != "categoryId" select item).ToDictionary(i => i.Key, i => i.Value);
                    translationItems.AddRange((from tr in translations select new TranslationItem { ItemId = itemId, ItemType = ItemTypes.Group, Language = tr.Key, Translation = tr.Value }).ToList());

                    bool enabled = (updates[key].ContainsKey("enabled") && (updates[key]["enabled"].Contains("true") || updates[key]["enabled"] == "on"));
                    bool displayImages = (updates[key].ContainsKey("displayImages") && (updates[key]["displayImages"].Contains("true") || updates[key]["displayImages"] == "on"));
                    int? categoryId = null;
                    if (updates[key].ContainsKey("categoryId"))
                        categoryId = Convert.ToInt32(updates[key]["categoryId"]);

                    Group group = context.Groups.Select(g => g).Where(g => g.Id == itemId).First();
                    group.Enabled = enabled;
                    group.DisplayProductImages = displayImages;
                    if (categoryId != null)
                    {
                        group.CategoryReference.EntityKey = new EntityKey("ZamovStorage.Categories", "Id", categoryId);
                    }
                }
                string translationXml = Utils.CreateTranslationXml(translationItems);
                context.SaveChanges();
                context.UpdateTranslations(translationXml);
            }
            return RedirectToAction("Groups");
        }

        private void ClearGroupCache()
        {
            string cacheKey = "dId_" + SystemSettings.CurrentDealer;
            List<string> cacheKeys = new List<string>();
            IDictionaryEnumerator enumerator = HttpContext.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().StartsWith(cacheKey))
                    cacheKeys.Add(enumerator.Key.ToString());
            }
            foreach (string key in cacheKeys)
                HttpContext.Cache.Remove(key);
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult DeleteGroup(int id)
        {
            ClearGroupCache();

            using (ZamovStorage context = new ZamovStorage())
            {
                Group group = context.Groups.Select(g => g).Where(g => g.Id == id).First();
                group.Deleted = true;
                context.SaveChanges();
            }
            return RedirectToAction("Groups");
        }
        #endregion

        #region Dealer
        [Authorize(Roles = "Administrators, Dealers")]
        [BreadCrumb(ResourceName = "EditDealer", Url = "/DealerCabinet/AddUpdateDealer")]
        public ActionResult AddUpdateDealer()
        {

            int id = Security.GetCurentDealerId(User.Identity.Name);
            if (id > 0)
            {
                using (ZamovStorage context = new ZamovStorage())
                {
                    Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
                    ViewData["dealer"] = dealer;
                    ViewData["hasDiscounts"] = dealer.HasDiscounts;
                }
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult AddUpdateDealer(FormCollection form)
        {
            int dealerId = Security.GetCurentDealerId(User.Identity.Name);
            Dealer dealer = null;
            using (ZamovStorage context = new ZamovStorage())
            {
                if (dealerId >= 0)
                    dealer = context.Dealers.Select(d => d).Where(d => d.Id == dealerId).First();
                else
                    dealer = new Dealer();
                dealer.Name = form["name"];
                dealer.Phone = form["phone"];
                if (form["bannerEnabled"] != null)
                    dealer.BannerEnabled = form["bannerEnabled"].Contains("true");
                dealer.Names["ru-RU"] = form["rName"];
                dealer.Names["uk-UA"] = form["uName"];
                dealer.Descriptions["ru-RU"] = Server.HtmlDecode(form["rDescription"]);
                dealer.Descriptions["uk-UA"] = Server.HtmlDecode(form["uDescription"]);
                dealer.GroupNames["ru-RU"] = form["rGroupName"];
                dealer.GroupNames["uk-UA"] = form["uGroupName"];
                dealer.HasDiscounts = form["hasDiscounts"].Contains("true");
                if (!string.IsNullOrEmpty(Request.Files["logoImage"].FileName))
                {
                    HttpPostedFileBase file = Request.Files["logoImage"];
                    dealer.LogoType = file.ContentType;
                    BinaryReader reader = new BinaryReader(file.InputStream);
                    dealer.LogoImage = reader.ReadBytes((int)file.InputStream.Length);
                }


                string bannerFile = (Request.Files["banner"] != null) ? Request.Files["banner"].FileName : string.Empty;
                if (!string.IsNullOrEmpty(bannerFile))
                {
                    string newFileName = IOHelper.GetUniqueFileName("~/Content/Banners", bannerFile);
                    string filePath = Path.Combine(Server.MapPath("~/Content/Banners"), newFileName);
                    Request.Files["banner"].SaveAs(filePath);
                    dealer.Banner = newFileName;
                }

                if (dealerId < 0)
                    context.AddToDealers(dealer);
                context.SaveChanges();
                context.UpdateTranslations(dealer.NamesXml);
                context.UpdateTranslations(dealer.DescriptionsXml);
                context.UpdateTranslations(dealer.GroupNamesXml);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult DeliveryInfo()
        {
            int id = Security.GetCurentDealerId(User.Identity.Name);
            if (id > 0)
            {
                using (ZamovStorage context = new ZamovStorage())
                {
                    var deliveryInfo = (from ruTran in context.Translations
                                        from uaTran in context.Translations
                                        where ruTran.ItemId == id && uaTran.ItemId == id
                                        && ruTran.Language == "ru-RU" && uaTran.Language == "uk-UA"
                                        && ruTran.TranslationItemTypeId == (int)ItemTypes.DealerDeliveryInfo
                                        && uaTran.TranslationItemTypeId == (int)ItemTypes.DealerDeliveryInfo
                                        select new { RuDeliveryInfo = ruTran.Text, UaDeliveryInfo = uaTran.Text }).SingleOrDefault();
                    if (deliveryInfo != null)
                    {
                        ViewData["deliveryDetailsRus"] = deliveryInfo.RuDeliveryInfo;
                        ViewData["deliveryDetailsUkr"] = deliveryInfo.UaDeliveryInfo;
                    }
                }
            }
            return View();
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public void UpdateDeliveryDetails(string deliveryDetailsUkr, string deliveryDetailsRus)
        {
            TranslationItem ukrItem = new TranslationItem
            {
                ItemId = SystemSettings.CurrentDealer.Value,
                ItemType = ItemTypes.DealerDeliveryInfo,
                Language = "uk-UA",
                Translation = Server.HtmlDecode(deliveryDetailsUkr)
            };

            TranslationItem rusItem = new TranslationItem
            {
                ItemId = SystemSettings.CurrentDealer.Value,
                ItemType = ItemTypes.DealerDeliveryInfo,
                Language = "ru-RU",
                Translation = Server.HtmlDecode(deliveryDetailsRus)
            };

            List<TranslationItem> items = new List<TranslationItem>();
            items.Add(ukrItem);
            items.Add(rusItem);
            using (ZamovStorage context = new ZamovStorage())
                context.UpdateTranslations(Utils.CreateTranslationXml(items));
            Response.Write(Helpers.Helpers.CloseParentScript("DeliveryDetails"));
        }
        #endregion

        #region ProductAdditional

        [Authorize(Roles = "Administrators, Dealers")]
        [BreadCrumb(ResourceName = "ProductsAdditional", Url = "/DealerCabinet/ProductsAdditional")]
        public ActionResult ProductsAdditional(int idGroup)
        {
            var products = new List<ProductPricePresentation>();
            int dealerId = Security.GetCurentDealerId(User.Identity.Name);
			var groupAdditionalAll = new List<GroupResentation>();
            using (ZamovStorage context = new ZamovStorage())
            {
            	products = context.GetCachedProductsAdditionalByGroupAndDealer(idGroup, dealerId);

				groupAdditionalAll = (from gr in context.GroupsAdditional
					                    join translation in context.Translations on gr.grID equals translation.ItemId
                                        join d in context.DealersGroups on gr.grID equals d.GroupsAdditional.grID
					                    where d.dg_dealerID == dealerId &&
                                            translation.TranslationItemTypeId == (int) ItemTypes.GroupAdditional
					                        && translation.Language == SystemSettings.CurrentLanguage
					                        && gr.gr_enabled
					                        && !gr.gr_deleted
					                    orderby gr.gr_IndexNumber
					                    select new GroupResentation
					                            {
					                             	Id = gr.grID,
					                             	Name = translation.Text,
					                             	ParentId = (gr.Parent != null) ? (int?) gr.Parent.grID : null,
					                             	CategoryId = gr.Categories.Id
					                            }
					                    ).ToList();
				groupAdditionalAll.ForEach(ac => ac.PickChildren(groupAdditionalAll));

				groupAdditionalAll = groupAdditionalAll.Select(c => c).Where(c => c.ParentId == null).ToList();

				ViewData["groupId"] = idGroup;
				ViewData["groups"] = groupAdditionalAll;

				List<Currencies> currencies = context.Currencies.Select(c => c).ToList();
                ViewData["currencies"] = currencies;
            }
            HttpContext.Cache.ClearProductAdditionalCache();
			return View(products);
        }

		[Authorize(Roles = "Administrators, Dealers")]
		public ActionResult DeleteProductAdditional(int productDealerId, int? groupId)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				DealersProducts curDealerProduct = (from dealerProduct in context.DealersProducts where dealerProduct.dpID == productDealerId select dealerProduct).First();
				curDealerProduct.dp_delete = true;
				context.SaveChanges();
			}
			string url = Url.Action("ProductsAdditional", "DealerCabinet", new { id = groupId });
			return Redirect(url);
		}

		[Authorize(Roles = "Administrators, Dealers")]
		public ActionResult UpdateProductsAdditional(FormCollection form)
		{
			PostData postData = form.ProcessPostData("groupId", "groups");
			PostData updates = new PostData();
			foreach (var item in postData)
				updates.Add(item.Key, item.Value.Where(v => v.Key != "moveTo").ToDictionary(v => v.Key, v => v.Value));
			int groupId = int.Parse(form["groupId"]);

			foreach (var item in updates)
			{
				item.Value["price"] = item.Value["price"].Replace(",", ".");
			}

			using (ZamovStorage context = new ZamovStorage())
			{
				context.UpdateProductsAdditional(updates.CreateUpdatesXml());
			}
            HttpContext.Cache.ClearProductAdditionalCache();
            return Redirect("~/DealerCabinet/ProductsAdditional/" + groupId);
		}

        #endregion

        #region Products
        [Authorize(Roles = "Administrators, Dealers")]
        [BreadCrumb(ResourceName = "Products", Url = "/DealerCabinet/Products")]
        public ActionResult Products(int? id)
        {
            ZamovStorage context = new ZamovStorage();
            int dealerId = Security.GetCurentDealerId(User.Identity.Name);
            List<Product> products = new List<Product>();
            if (id != null && id > 0)
            {
                products = (from product in context.Products.Include("Manufacturer").Include("Currencies")
                            where product.Group.Id == id.Value && product.Dealer.Id == dealerId && !product.Deleted && !product.Group.Deleted
                            select product).ToList();
            }
            else if (id == 0)
            {
                products = (from product in context.Products
                            where product.Dealer.Id == dealerId && !product.Deleted && product.Group.Name != "TRASH" && !product.Group.Deleted
                            select product).ToList();
            }
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> moveToItems = new List<SelectListItem>();
            int currentGroupId = (id) ?? int.MinValue;
            GetGroupItems(items, dealerId, int.MinValue, "", currentGroupId);
            GetGroupItems(moveToItems, dealerId, int.MinValue, "", currentGroupId);
            moveToItems.RemoveAt(0);
            items.Insert(1, new SelectListItem { Value = "0", Text = ResourcesHelper.GetResourceString("AllProducts") });
            ViewData["groups"] = items;
            ViewData["moveToGroups"] = moveToItems;
            ViewData["groupId"] = currentGroupId;


            List<Manufacturer> manufacturers = context.Manufacturer.Select(m => m).ToList();
            List<SelectListItem> manufacturersList = new List<SelectListItem>();
            foreach (var item in manufacturers)
            {
                manufacturersList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewData["manufacturers"] = manufacturersList;

            List<Currencies> currencies = context.Currencies.Select(c => c).ToList();
            List<SelectListItem> currencysList = new List<SelectListItem>();
            currencysList.Add(new SelectListItem { Text = "---", Value = "0" });
            foreach (var item in currencies)
            {
                currencysList.Add(new SelectListItem { Text = item.Name + " [" + item.ShortName + "]", Value = item.Id.ToString() });
            }
            ViewData["currencies"] = currencysList;

            return View(products);
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult AddProduct(string partNumber, string name, decimal price, bool active, int groupId, string unit)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Product product = new Product();
                int dealerId = Security.GetCurentDealerId(User.Identity.Name);
                IEnumerable<KeyValuePair<string, object>> dealerKeyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Id", dealerId) };
                EntityKey dealer = new EntityKey("ZamovStorage.Dealers", dealerKeyValues);
                product.DealerReference.EntityKey = dealer;
                IEnumerable<KeyValuePair<string, object>> groupKeyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Id", groupId) };
                EntityKey group = new EntityKey("ZamovStorage.Groups", groupKeyValues);
                product.GroupReference.EntityKey = group;
                product.PartNumber = partNumber;
                product.Name = name;
                product.Price = price;
                product.Enabled = active;
                product.Unit = unit;
                context.AddToProducts(product);
                context.SaveChanges();
            }
            string url = Url.Action("Products", "DealerCabinet", new { id = groupId });
            return Redirect(url);
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult DeleteProduct(int productId, int groupId)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Product currentProduct = (from product in context.Products where product.Id == productId select product).First();
                currentProduct.Deleted = true;
                context.SaveChanges();
            }
            string url = Url.Action("Products", "DealerCabinet", new { id = groupId });
            return Redirect(url);
        }

        private void GetGroupItems(List<SelectListItem> items, int dealerId, int groupId, string prefix, int? currentGroupId)
        {

            if (groupId < 0)
                items.Add(new SelectListItem { Selected = currentGroupId == null, Text = ResourcesHelper.GetResourceString("SelectGroup"), Value = "" });
            using (ZamovStorage context = new ZamovStorage())
            {
                List<Group> groups = new List<Group>();
                string cacheKey = "dId_" + dealerId + "gId" + groupId;
                if (HttpContext.Cache[cacheKey] != null)
                    groups = (List<Group>)HttpContext.Cache[cacheKey];
                else
                {
                    if (groupId > 0)
                        groups = (from g in context.Groups where g.Dealer.Id == dealerId && g.Parent.Id == groupId && !g.Deleted select g).ToList();
                    else
                        groups = (from g in context.Groups where g.Dealer.Id == dealerId && g.Parent == null && !g.Deleted select g).ToList();
                    HttpContext.Cache[cacheKey] = groups;
                }
                foreach (var g in groups)
                {
                    SelectListItem listItem = new SelectListItem
                    {
                        Selected = (g.Id == currentGroupId),
                        Text = prefix + " " + g.GetName(SystemSettings.CurrentLanguage),
                        Value = g.Id.ToString()
                    };
                    items.Add(listItem);
                    if (!g.Groups.IsLoaded)
                        g.Groups.Load();
                    if (g.Groups != null && g.Groups.Count > 0)
                        GetGroupItems(items, dealerId, g.Id, prefix + "--", currentGroupId);
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UpdateProductDescription(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Product product = (from p in context.Products where p.Id == id select p).First();
                product.LoadDescriptions();
                ViewData["descroptionUkr"] = product.GetDescription("uk-UA");
                ViewData["descroptionRus"] = product.GetDescription("ru-RU");
                ViewData["productId"] = id;
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public void UpdateProductDescription(int productId, string descroptionUkr, string descroptionRus)
        {
            TranslationItem ukrItem = new TranslationItem
            {
                ItemId = productId,
                ItemType = ItemTypes.ProductDescription,
                Language = "uk-UA",
                Translation = Server.HtmlDecode(descroptionUkr)
            };

            TranslationItem rusItem = new TranslationItem
            {
                ItemId = productId,
                ItemType = ItemTypes.ProductDescription,
                Language = "ru-RU",
                Translation = Server.HtmlDecode(descroptionRus)
            };

            List<TranslationItem> items = new List<TranslationItem>();
            items.Add(ukrItem);
            items.Add(rusItem);
            using (ZamovStorage context = new ZamovStorage())
                context.UpdateTranslations(Utils.CreateTranslationXml(items));
            Response.Write("<script type=\"text/javascript\">top.closeDescriptionDialog();</script>");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UpdateProductImage(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                ProductImage image = context.ProductImages.Select(pi => pi).Where(pi => pi.Product.Id == id).SingleOrDefault();
                int imageId = (image != null) ? image.Id : int.MinValue;
                ViewData["imageId"] = imageId;
                ViewData["productId"] = id;
                return View();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public void UpdateProductImage(int id, int productId)
        {
            if (!string.IsNullOrEmpty(Request.Files["newImage"].FileName))
            {
                ProductImage image = null;
                image = new ProductImage();
                IEnumerable<KeyValuePair<string, object>> productKeyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Id", productId) };
                EntityKey product = new EntityKey("ZamovStorage.Products", productKeyValues);
                image.ProductReference.EntityKey = product;
                HttpPostedFileBase file = Request.Files["newImage"];
                image.ImageType = file.ContentType;
                Image uploadedImage = Image.FromStream(file.InputStream);

                const int maxDimension = 400;
                int width;
                int height;
                if (uploadedImage.Width > uploadedImage.Height)
                {
                    width = maxDimension;
                    height = (maxDimension * uploadedImage.Height) / uploadedImage.Width;

                }
                else if (uploadedImage.Height > uploadedImage.Width)
                {
                    height = maxDimension;
                    width = (maxDimension * uploadedImage.Width) / uploadedImage.Height;
                }
                else
                    width = height = maxDimension;
                Bitmap bitmap = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(uploadedImage, 0, 0, width, height);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(stream);
                image.Image = reader.ReadBytes((int)stream.Length);

                using (ZamovStorage context = new ZamovStorage())
                {
                    context.CleanupProductImages(productId);
                    context.AddToProductImages(image);
                    context.SaveChanges();
                }
            }
            Response.Write("<script type=\"text/javascript\">top.closeImageDialog();</script>");
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UpdateProducts(FormCollection form)
        {
            PostData postData = form.ProcessPostData("groupId", "groups", "manufacturers", "currencies");
            PostData updates = new PostData();
            foreach (var item in postData)
                updates.Add(item.Key, item.Value.Where(v => v.Key != "moveTo").ToDictionary(v => v.Key, v => v.Value));
            int groupId = int.Parse(form["groupId"]);

            foreach (var item in updates)
            {
                item.Value["price"] = item.Value["price"].Replace(",", ".");
            }

            int moveToGroup = 0;
            int manufacturerId = 0;
            int currencyId = 0;
            using (ZamovStorage context = new ZamovStorage())
            {
                context.UpdateProducts(updates.CreateUpdatesXml());

                if (int.TryParse(form["groups"], out moveToGroup))
                {
                    List<int> moverProducts = new List<int>();
                    foreach (var item in postData)
                    {
                        if (item.Value["moveTo"] == "true")
                            moverProducts.Add(int.Parse(item.Key));
                    }
                    if (moverProducts.Count > 0)
                    {
                        string[] movedProductsArray = moverProducts.Select(i => i.ToString()).ToArray();
                        string productIds = string.Join(",", movedProductsArray);

                        ObjectQuery<Product> productsQuery = new ObjectQuery<Product>(
                                    "SELECT VALUE P FROM Products AS P WHERE P.Id IN {" + productIds + "}",
                                    context);
                        Group group = context.Groups.Where(g => g.Id == moveToGroup).Select(g => g).First();

                        foreach (Product product in productsQuery)
                            product.Group = group;
                        context.SaveChanges();
                    }
                }



                if (int.TryParse(form["manufacturers"], out manufacturerId))
                {
                    List<int> manufacturerProducts = new List<int>();
                    foreach (var item in postData)
                    {
                        if (item.Value["setManufacturer"] == "true")
                            manufacturerProducts.Add(int.Parse(item.Key));
                    }
                    if (manufacturerProducts.Count > 0)
                    {
                        string[] manufacturerProductsArray = manufacturerProducts.Select(i => i.ToString()).ToArray();
                        string productIds = string.Join(",", manufacturerProductsArray);





                        ObjectQuery<Product> productsQuery = new ObjectQuery<Product>(
                                    "SELECT VALUE P FROM Products AS P WHERE P.Id IN {" + productIds + "}",
                                    context);



                        Manufacturer manufacturer = context.Manufacturer.Where(m => m.Id == manufacturerId).Select(m => m).First();

                        foreach (Product product in productsQuery)
                        {
                            context.CleanupProductManufacturer(product.Id);
                            product.Manufacturer.Add(manufacturer);
                        }
                        context.SaveChanges();

                    }
                }

                if (int.TryParse(form["currencies"], out currencyId))
                {
                    List<int> setCurrencyProducts = new List<int>();
                    foreach (var item in postData)
                    {
                        if (item.Value["setCurrency"] == "true")
                            setCurrencyProducts.Add(int.Parse(item.Key));
                    }
                    if (setCurrencyProducts.Count > 0)
                    {
                        string[] setCurrencyProductsArray = setCurrencyProducts.Select(i => i.ToString()).ToArray();
                        string productIds = string.Join(",", setCurrencyProductsArray);
                        ObjectQuery<Product> productsQuery = new ObjectQuery<Product>(
                                    "SELECT VALUE P FROM Products AS P WHERE P.Id IN {" + productIds + "}",
                                    context);


                        Currencies productCurrency = null;
                        if (currencyId != 0)
                            productCurrency = context.Currencies.Where(c => c.Id == currencyId).Select(c => c).First();



                        foreach (Product product in productsQuery)
                        {
                            product.Currencies = productCurrency;
                        }
                        context.SaveChanges();
                    }
                }
            }
            return Redirect("~/DealerCabinet/Products/" + groupId);
        }
        #endregion

        #region Products import

        [BreadCrumb(ResourceName = "ImportedProducts", Url = "/DealerCabinet/ImportedProducts")]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult ImportedProducts(int? groupId)
        {
            bool processData = false;

            Dictionary<string, Dictionary<string, string>> updatedItemsDictionary = null;
            Dictionary<string, Dictionary<string, string>> newItemsDictionary = null;

            if (Session["uploadedXls"] != null)
            {
                string fileName = (string)Session["uploadedXls"];
                List<Dictionary<string, string>> importedProductsSet = Utils.QureyUploadedXls(fileName, SystemSettings.CurrentDealer.Value, groupId);
                List<Dictionary<string, string>> updatedItems = (from item in importedProductsSet where item["productId"] != null select item).ToList();
                List<Dictionary<string, string>> newItems = (from item in importedProductsSet where item["productId"] == null select item).ToList();

                newItems.ForEach(i => i["productId"] = (-i.GetHashCode()).ToString());
                System.IO.File.Delete(fileName);
                Session["uploadedXls"] = null;
                updatedItemsDictionary = updatedItems.ToDictionary(el => (string)el["productId"], el => el);
                newItemsDictionary = newItems.ToDictionary(el => (string)el["productId"], el => el);
                Session["updatedItems"] = updatedItemsDictionary;
                Session["newItems"] = newItemsDictionary;

                processData = true;
            }
            else if (Session["updatedItems"] != null || Session["newItems"] != null)
            {
                updatedItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["updatedItems"];
                newItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["newItems"];
                processData = true;
            }
            if (processData)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                string cacheKey = "ImportedProduct_DealerId=" + SystemSettings.CurrentDealer.Value;
                GetGroupItems(items, SystemSettings.CurrentDealer.Value, int.MinValue, "", groupId);
                ViewData["groupItems"] = items;
                ViewData["updatedItems"] = updatedItemsDictionary;
                ViewData["newItems"] = newItemsDictionary;
            }
            return View();
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult ImportedProduct(Dictionary<string, string> product, bool isNew)
        {
            int productId = int.Parse(product["productId"]);
            string partNumber = (string)product["partNumber"];
            string name = (string)product["name"];
            string price = (string)product["price"].Replace(",", ".");
            string ukDescription = (string)product["ukDescription"];
            string ruDescription = (string)product["ruDescription"];
            string unit = (string)product["unit"];
            string imageUrl = null;
            if (product.ContainsKey("imageUrl"))
            {
                imageUrl = (string)product["imageUrl"];
            }
            ViewData["id"] = productId;
            ViewData["partNumber"] = partNumber;
            ViewData["name"] = name;
            decimal decimalPrice = 0;
            if (!decimal.TryParse(price, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out decimalPrice))
                decimalPrice = 0M;
            ViewData["price"] = decimalPrice.ToString(CultureInfo.GetCultureInfo("en-US"));
            ViewData["ukDescription"] = ukDescription;
            ViewData["ruDescription"] = ruDescription;
            ViewData["unit"] = unit;
            ViewData["isNew"] = isNew;
            ViewData["imageUrl"] = imageUrl;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UploadXls(int? groupId)
        {
            if (!string.IsNullOrEmpty(Request.Files["xls"].FileName))
            {
                string fileName = Request.Files["xls"].FileName;
                string extension = Path.GetExtension(fileName);
                if (extension != ".xls" && extension != ".xlsx")
                    return RedirectToAction("UploadXlsError");
                int hashcode = User.GetHashCode();
                Request.Files["xls"].SaveAs(Server.MapPath("~/UploadedFiles/" + hashcode + "_Imported" + extension));
                Session["uploadedXls"] = Server.MapPath("~/UploadedFiles/" + hashcode + "_Imported" + extension);
                return RedirectToAction("ImportedProducts", new { groupId = groupId });
            }
            else
                return RedirectToAction("UploadXlsError");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult MoveToNew(FormCollection form, int? groupId)
        {
            PostData postData = form.ProcessPostData("groupId");
            int[] items = (from item in postData where item.Value["check"] == "true" select int.Parse(item.Key)).ToArray();
            Dictionary<string, Dictionary<string, string>> updatedItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["updatedItems"];
            Dictionary<string, Dictionary<string, string>> newItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["newItems"];
            foreach (int id in items)
            {
                Dictionary<string, string> itemToMove = (from item in updatedItemsDictionary where item.Key == id.ToString() select item.Value).First();
                newItemsDictionary.Add((-itemToMove.GetHashCode()).ToString(), itemToMove);
                updatedItemsDictionary.Remove(id.ToString());
            }

            if (updatedItemsDictionary.Count == 0 && newItemsDictionary.Count == 0)
                return RedirectToAction("Products", new { groupId = groupId });

            return RedirectToAction("ImportedProducts", new { groupId = groupId });
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult SaveSelectedTo(FormCollection form, int groupItems)
        {
            PostData postData = form.ProcessPostData("groupItems");
            int[] items = (from item in postData where item.Value["check"] == "true" select int.Parse(item.Key)).ToArray();

            Dictionary<string, Dictionary<string, string>> newItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["newItems"];
            Dictionary<string, Dictionary<string, string>> updatedItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["updatedItems"];

            Dictionary<string, Dictionary<string, string>> itemsToMove =
                (from item in newItemsDictionary where items.Contains(int.Parse(item.Key)) select item.Value).ToDictionary(i => i["productId"], i => i);

            foreach (string key in itemsToMove.Keys)
            {
                itemsToMove[key]["groupId"] = groupItems.ToString();
                itemsToMove[key]["price"] = itemsToMove[key]["price"].Replace(",", ".");
                newItemsDictionary.Remove(key);
            }

            string newItemsXml = itemsToMove.CreateUpdatesXml();

            using (ZamovStorage context = new ZamovStorage())
                context.InsertImportedProducts(newItemsXml, SystemSettings.CurrentDealer.Value);

            if (updatedItemsDictionary.Count == 0 && newItemsDictionary.Count == 0)
                return RedirectToAction("Products", new { groupId = groupItems });

            return RedirectToAction("ImportedProducts", new { groupId = groupItems });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult SaveUpdated(FormCollection form)
        {
            Dictionary<string, Dictionary<string, string>> updatedItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["updatedItems"];
            Dictionary<string, Dictionary<string, string>> newItemsDictionary = (Dictionary<string, Dictionary<string, string>>)Session["newItems"];

            string updatesXml = updatedItemsDictionary.CreateUpdatesXml();
            using (ZamovStorage context = new ZamovStorage())
                context.UpdateImportedProducts(updatesXml);

            ((Dictionary<string, Dictionary<string, string>>)Session["updatedItems"]).Clear();

            if (updatedItemsDictionary.Count == 0 && newItemsDictionary.Count == 0)
                return RedirectToAction("Products", new { groupId = form["groupId"] });

            return RedirectToAction("ImportedProducts", new { groupId = form["groupId"] });
        }
        #endregion

        #region DealerMappings
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult DealerCityMappings()
        {
            int dealerId = SystemSettings.CurrentDealer.Value;
            List<City> cities = null;
            List<City> dealerCities = null;
            bool checkAllCity = false;
            using (ZamovStorage context = new ZamovStorage())
            {
                cities = (from city in context.Cities orderby city.IndexNumber select city).ToList();
                dealerCities = (from dealer in context.Dealers where dealer.Id == dealerId select dealer.Cities).First().ToList();
                checkAllCity = (from dealer in context.Dealers where dealer.Id == dealerId select dealer.LoadXml).FirstOrDefault();
            }
            List<SelectListItem> items = (from city in cities
                                          select new SelectListItem
                                          {
                                              Text = city.GetName(SystemSettings.CurrentLanguage),
                                              Value = city.Id.ToString(),
                                              Selected =
                                                (from dc in dealerCities where dc.Id == city.Id select dc).Count() > 0
                                          }).ToList();

            ViewData["checkAllCity"] = checkAllCity;
            ViewData["items"] = items;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public void DealerCityMappings(FormCollection form)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Dealer dealer = (from d in context.Dealers where d.Id == SystemSettings.CurrentDealer.Value select d).First();
                dealer.Cities.Load();
                dealer.Cities.Clear();
                foreach (string key in form.Keys)
                {
                    if (form[key].IndexOf("true") > -1)
                    {
                        int cityId = int.Parse(key);
                        City city = (from c in context.Cities where c.Id == cityId select c).First();
                        dealer.Cities.Add(city);
                    }
                }
                context.SaveChanges();
            }
            Response.Write(Helpers.Helpers.CloseParentScript("CityMappings"));
        }

        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult DealerCategoryMappings()
        {
            int dealerId = SystemSettings.CurrentDealer.Value;

            using (ZamovStorage context = new ZamovStorage())
            {
                List<CategoryPresentation> categories = (from category in context.Categories
                                                         join translation in context.Translations on category.Id equals translation.ItemId
                                                         where translation.TranslationItemTypeId == (int)ItemTypes.Category
                                                            && translation.Language == SystemSettings.CurrentLanguage
                                                            && category.Enabled
                                                         orderby category.IndexNumber
                                                         select new CategoryPresentation
                                                             {
                                                                 Id = category.Id,
                                                                 Name = translation.Text,
                                                                 Selected = context.Dealers.Include("Categories")
                                                                    .Where(d => d.Id == dealerId)
                                                                    .FirstOrDefault().Categories
                                                                    .Where(c => c.Id == category.Id).Count() > 0,
                                                                 ParentId = (category.Parent != null) ? (int?)category.Parent.Id : null
                                                             }
                                                         ).ToList();

                categories.ForEach(ac => ac.PickChildren(categories));
                List<CategoryPresentation> sortedCategories = categories.Select(c => c).Where(c => c.ParentId == null).ToList();
                return View(sortedCategories);

            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public void DealerCategoryMappings(FormCollection form)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Dealer dealer = (from d in context.Dealers where d.Id == SystemSettings.CurrentDealer.Value select d).First();
                dealer.Categories.Load();
                dealer.Categories.Clear();
                foreach (string key in form.Keys)
                {
                    if (form[key].IndexOf("true") > -1)
                    {
                        int categoryId = int.Parse(key);
                        Category category = (from c in context.Categories where c.Id == categoryId select c).First();
                        dealer.Categories.Add(category);
                    }
                }
                context.SaveChanges();
            }
            HttpContext.Cache.ClearCategoriesCache();
            Response.Write(Helpers.Helpers.CloseParentScript("CategoryMappings"));
        }

        #endregion

        #region Payment details
        public ActionResult PaymentDetails()
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Dealer dealer = (from d in context.Dealers where d.Id == SystemSettings.CurrentDealer select d).First();
                ViewData["cash"] = dealer.Cash;
                ViewData["noncash"] = dealer.Noncash;
                ViewData["card"] = dealer.Card;
                ViewData["hasDiscounts"] = dealer.HasDiscounts;
                return View();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void PaymentDetails(bool cash, bool noncash, bool card, bool hasDiscounts)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Dealer dealer = (from d in context.Dealers where d.Id == SystemSettings.CurrentDealer select d).First();
                dealer.Cash = cash;
                dealer.Card = card;
                dealer.Noncash = noncash;
                dealer.HasDiscounts = hasDiscounts;
                context.SaveChanges();
            }
            Response.Write(Helpers.Helpers.CloseParentScript("PaymentDetails"));
        }
        #endregion

        [Authorize(Roles = "Administrators")]
        public ActionResult SelectDealer(int currentDealerId, string redirectTo)
        {
            SystemSettings.CurrentDealer = currentDealerId;
            return Redirect(redirectTo);
        }

        #region Ordering
        [Authorize(Roles = "Administrators, Dealers")]
        [BreadCrumb(ResourceName = "Orders", Url = "/DealerCabinet/Orders")]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult Orders()
        {
            SystemSettings.LastTime = DateTime.Now;
            using (OrderStorage context = new OrderStorage())
            {
                List<Order> orders = (
                                         from order in
                                             context.Orders.Include("Dealer").Include("OrderItems")
                                         where order.Dealer.Id == SystemSettings.CurrentDealer
                                         orderby order.Date descending
                                         select order).ToList();

                return View(orders);
            }
        }

        [OutputCache(NoStore = true, VaryByParam = "*", Duration = 1)]
        public ActionResult ShowOrder(int id)
        {
            using (OrderStorage context = new OrderStorage())
            {
                Order order = (from o in context.Orders.Include("OrderItems") where o.Id == id select o).First();
                return View(order);

                /*List<Order> orders = (from order in context.Orders.Include("OrderItems") where order.Id == id select order).ToList();
                return View(orders);*/
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void AcceptOrder(int orderId)
        {
            using (OrderStorage context = new OrderStorage())
            {
                Order order = (from o in context.Orders where o.Id == orderId select o).First();
                order.Status = (int)Statuses.Accepted;
                context.SaveChanges();
                //return RedirectToAction("Orders");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CancelOrder(int orderId)
        {
            using (OrderStorage context = new OrderStorage())
            {
                Order order = (from o in context.Orders where o.Id == orderId select o).First();
                order.Status = (int)Statuses.Canceled;
                context.SaveChanges();
                return RedirectToAction("Orders");
            }
        }
        #endregion

        #region Excel(.xls, .xlsx) Upload for products additional

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UploadXlsForProductsAdditional(FormCollection form)
        {
            if (!string.IsNullOrEmpty(Request.Files["xls"].FileName))
            {
                string fileName = Request.Files["xls"].FileName;
                string extension = Path.GetExtension(fileName);
                if (extension != ".xls" && extension != ".xlsx")
                {
                    TempData["error"] = "Только .xls и .xlsx файлы";
                    return RedirectToAction("ProductsAdditional");
                }

                int hashcode = User.GetHashCode();
                string pathFile = Server.MapPath("~/UploadedFiles/" + hashcode + "_ImportedXls" + extension);
                Request.Files["xls"].SaveAs(pathFile);

                try
                {
                    FileStream stream = System.IO.File.Open(pathFile, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = null;
                    if (extension == ".xls")
                        excelReader = Factory.CreateReader(stream, ExcelFileType.Binary);
                    if (extension == ".xlsx")
                        excelReader = Factory.CreateReader(stream, ExcelFileType.OpenXml);
                    DataSet data = excelReader.AsDataSet();
                    ImportFromXlsToSQL(data);
                    
                    stream.Close();
                    excelReader.Close();
                    System.IO.File.Delete(pathFile);
                    TempData["success"] = "Информация из "+fileName+" файла успешно импортированна!";
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message + ";" + ex.Source + ";" + ex.StackTrace;
                }
                HttpContext.Cache.ClearProductAdditionalCache();
                return RedirectToAction("ProductsAdditional");
            }
            else
            {
                TempData["error"] = "Ошибка .xls(.xlsx) загрузки!";
                return RedirectToAction("ProductsAdditional");
            }
        }

        private void ImportFromXlsToSQL(DataSet data)
        {
            if (data == null)
                throw new Exception("Документ Excel не считан");

            if (data.Tables.Count != 3)
                throw new Exception("Документ Excel неправильно подготовлен, должно быть 3 страницы: Бренды, Группы, Товары");
            
            using (var context = new ZamovStorage())
            {
                int dealerId = Security.GetCurentDealerId(User.Identity.Name);
                DbCommand command = null;
                object[] parameters = new object[] {};

                // Проверка на корректность заполнения документа эксель, критерий количество колонок :)!
                DataTable dataBrends = null;
                DataTable dataGroups = null;
                DataTable dataProducts = null;
                for (int i = 0; i < data.Tables.Count; i++)
                {
                    if (data.Tables[i].Columns.Count == 2)
                    {
                        dataBrends = data.Tables[i];
                    }
                    if (data.Tables[i].Columns.Count == 3)
                    {
                        dataGroups = data.Tables[i];
                    }
                    if (data.Tables[i].Columns.Count == 11)
                    {
                        dataProducts = data.Tables[i];
                    }
                }

                StringBuilder error = new StringBuilder();
                if (dataBrends == null)
                    error.Append(" Страница Бренды должна иметь 2 колонки!");
                if (dataGroups == null)
                    error.Append(" Страница Группы должна иметь 3 колонки!");
                if (dataProducts == null)
                    error.Append(" Страница Товары должна иметь 11 колонок!");

                if (error.Length > 0)
                {
                    error.Insert(0, "Документ Excel неправильно подготовлен:");
                    throw new Exception(error.ToString());
                }

                //обработка брэндов
                #region обработка брэндов

                if (dataBrends.Rows.Count > 1)
                {
                    for (int i = 1; i < dataBrends.Rows.Count; i++)
                    {
                        object bID = dataBrends.Rows[i][0];
                        if (bID == DBNull.Value) break;
                        object bName = dataBrends.Rows[i][1];

                        parameters = new object[]
                                             {
                                                 new SqlParameter("db_brandID", Convert.ToInt32(bID)),
                                                 new SqlParameter("db_brandName", (string) bName),
                                                 new SqlParameter("db_dealerID", dealerId),
                                                 new SqlParameter("db_ourbID", DBNull.Value)
                                             };

                        command = context.CreateStoreCommand("sp_DealersBrands_Add", CommandType.StoredProcedure,
                                                             parameters);

                        using (context.Connection.CreateConnectionScope())
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    command = context.CreateStoreCommand("sp_BrandsDealersToOur", CommandType.StoredProcedure);
                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }

                    //var dealerBrands = new List<DealerBindBrandPresent>();
                    //command = context.CreateStoreCommand("sp_DealersBrandsNew_Select", CommandType.StoredProcedure);

                    //using (context.Connection.CreateConnectionScope())
                    //{
                    //    using (var reader = command.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            var d = new DealerBindBrandPresent
                    //            {
                    //                IdDealerBrand = Convert.ToInt32(reader["dbID"]),
                    //                NameBrand = Convert.ToString(reader["brandName"]),
                    //                IdBrandMain = reader["brandIdMain"] != DBNull.Value ? Convert.ToInt32(reader["brandIdMain"]) : -1
                    //            };
                    //            dealerBrands.Add(d);
                    //        }
                    //    }
                    //}

                    //foreach (var dp in dealerBrands)
                    //{
                    //    parameters = new object[]
                    //                {
                    //                    new SqlParameter("@bID", dp.IdDealerBrand),
                    //                    new SqlParameter("@bNameOur", dp.NameBrand),
                    //                };

                    //    command = context.CreateStoreCommand(dp.IdBrandMain > 0 ? "sp_DealersBrandsNew_Bind"
                    //        : "sp_DealersBrandsNew_Add", CommandType.StoredProcedure, parameters);

                    //    using (context.Connection.CreateConnectionScope())
                    //    {
                    //        command.ExecuteNonQuery();
                    //    }
                    //}
                }
                #endregion обработка брэндов

                //обработка групп
                #region обработка групп

                if (dataGroups.Rows.Count > 1)
                {
                    for (int i = 1; i < dataGroups.Rows.Count; i++)
                    {
                        object gID = dataGroups.Rows[i][0];
                        if (gID == DBNull.Value) break;
                        object gParID = dataGroups.Rows[i][1];
                        object gName = dataGroups.Rows[i][2];

                        parameters = new object[]
                        {   
                            new SqlParameter("dg_dealerID",dealerId),
                            new SqlParameter("dg_dgrID",Convert.ToInt32(gID)),
                            new SqlParameter("dg_parGrID",gParID == DBNull.Value ? 0 : Convert.ToInt32(gParID)),
                            new SqlParameter("dg_dgrName",(string)gName),
                            new SqlParameter("dg_ourgrID",DBNull.Value)
                        };

                        command = context.CreateStoreCommand("sp_DealersGroup_Add", CommandType.StoredProcedure, parameters);

                        using (context.Connection.CreateConnectionScope())
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    command = context.CreateStoreCommand("sp_GroupsDealersToOur", CommandType.StoredProcedure);
                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }
                #endregion обработка групп

                //обработка продуктов
                #region обработка продуктов

                if (dataProducts.Rows.Count > 1)
                {
                    for (int i = 1; i < dataProducts.Rows.Count; i++)
                    {
                        object pID = dataProducts.Rows[i][0];
                        if (pID == DBNull.Value) break;
                        object pName = dataProducts.Rows[i][1];
                        object bID = dataProducts.Rows[i][2];
                        object gID = dataProducts.Rows[i][3];
                        object unit = dataProducts.Rows[i][4];
                        object photoUrl = dataProducts.Rows[i][5];
                        object price = dataProducts.Rows[i][6];
                        object currency = dataProducts.Rows[i][7];
                        object state = dataProducts.Rows[i][8];
                        object guarantee = dataProducts.Rows[i][9];
                        object description = dataProducts.Rows[i][10];
                        
                        parameters = new object[]
                        {   
                            new SqlParameter("dp_dealerID",dealerId),
                            new SqlParameter("dp_prodID",Convert.ToInt32(pID)),
                            new SqlParameter("dp_prodName",(string)pName),
                            new SqlParameter("dp_grID",Convert.ToInt32(gID)),
                            new SqlParameter("dp_brandID",Convert.ToInt32(bID)),
                            new SqlParameter("dp_unit", unit==DBNull.Value ? "шт." : (string)unit),
                            new SqlParameter("dp_guarantee",guarantee==DBNull.Value ? 0 : Convert.ToInt32(guarantee)),
                            new SqlParameter("dp_photoUrl",photoUrl==DBNull.Value ? String.Empty : (string)photoUrl),
                            new SqlParameter("dp_ourpID",DBNull.Value),
                            new SqlParameter("dp_descr",description==DBNull.Value ? String.Empty : (string)description)
                        };

                        command = context.CreateStoreCommand("sp_DealersProducts_Add", CommandType.StoredProcedure, parameters);

                        using (context.Connection.CreateConnectionScope())
                        {
                            command.ExecuteNonQuery();
                        }

                        parameters = new object[]
                        {   
                            new SqlParameter("pd_dealerID",dealerId),
                            new SqlParameter("pd_prodID",Convert.ToInt32(pID)),
                            new SqlParameter("pd_price",price==null ? 0 : Convert.ToDouble(price)),
                            new SqlParameter("currency",currency==null ? "UAH" : (string)currency),
                            new SqlParameter("pd_state",state==null ? String.Empty : (string)state),
                        };

                        command = context.CreateStoreCommand("sp_ProductsByDealers_Add", CommandType.StoredProcedure, parameters);

                        using (context.Connection.CreateConnectionScope())
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    command = context.CreateStoreCommand("sp_ProductsDealersToOur", CommandType.StoredProcedure);
                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }
                #endregion  обработка продуктов
            }
        }

        #endregion

        #region XML Upload for products additional

        [BreadCrumb(ResourceName = "UploadXml", Url = "/DealerCabinet/UploadXml")]
        public ActionResult UploadXml()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrators, Dealers")]
        public ActionResult UploadXml(FormCollection form)
        {
            if (!string.IsNullOrEmpty(Request.Files["xml"].FileName))
            {
                string fileName = Request.Files["xml"].FileName;
                string extension = Path.GetExtension(fileName);
                if (extension != ".xml")
                {
                    TempData["error"] = "Только .xml файлы";
                    return RedirectToAction("ProductsAdditional");
                }
                int hashcode = User.GetHashCode();
                string pathFile = Server.MapPath("~/UploadedFiles/" + hashcode + "_ImportedXml" + extension);
                Request.Files["xml"].SaveAs(pathFile);
                try
                {
                    XmlTextReader xmlTextReader = new XmlTextReader(pathFile);
                    var doc = XDocument.Load(xmlTextReader);
                    XMLtoSQL(doc);
                    xmlTextReader.Close();
                    TempData["success"] = "Информация из файла " + fileName + " успешно импортированна!";
                    System.IO.File.Delete(pathFile);
                    HttpContext.Cache.ClearProductAdditionalCache();
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message + ";" + ex.Source + ";" + ex.StackTrace;
                }

                return RedirectToAction("ProductsAdditional");
            }
            else
            {
                TempData["error"] = "Ошибка .xml загрузки!";
                return RedirectToAction("ProductsAdditional");
            }
        }

        private void XMLtoSQL(System.Xml.Linq.XDocument doc)
        {
            if (doc == null)
                throw new Exception("Документ XML не считан");

            using (var context = new ZamovStorage())
            {
                //считать ID дилера
                int dealerId = Security.GetCurentDealerId(User.Identity.Name);
                //SystemSettings.CurrentDealer.Value;
                DbCommand command = null;
                object[] parameters = new object[] { };

                try
                {
                    var xmlCurrency = from currency in doc.Elements("dealer").Elements("currency")
                                      select new
                                                 {
                                                     CurrencyCode = currency.Attribute("code") !=null ? currency.Attribute("code").Value : "",
                                                     CurrencyRate = currency.Attribute("rate") != null ? currency.Attribute("rate").Value : "" ,
                                                     CurrencyDate = currency.Attribute("date") != null ? currency.Attribute("date").Value : ""
                                                 };

                    foreach (var xml in xmlCurrency)
                    {
                        decimal curRate;
                        decimal.TryParse(xml.CurrencyRate.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out curRate);

                        if (curRate > 0)
                        {
                            DateTime dateBegin;
                            DateTime.TryParse(xml.CurrencyDate, out dateBegin);
                            if (dateBegin == DateTime.MinValue)
                            {
                                dateBegin = DateTime.Now;
                            }

                            Dealer currentDealer = context.Dealers.Select(d => d).FirstOrDefault(d => d.Id == dealerId);
                            Currencies currentCurrency =
                                context.Currencies.Select(c => c).FirstOrDefault(
                                    c => c.ShortName == xml.CurrencyCode.ToUpper());
                            ExchangeRate dealerExchangeRate = new ExchangeRate();
                            dealerExchangeRate.Dealers = currentDealer;
                            dealerExchangeRate.Currencies = currentCurrency;
                            dealerExchangeRate.xr_value = curRate;
                            dealerExchangeRate.xr_dateBeg = dateBegin;
                            context.AddToExchangeRate(dealerExchangeRate);
                            context.SaveChanges();
                        }
                    }
                }
                catch(Exception)
                {
                    
                }

                var xmlBrands = from Brand in doc.Elements("dealer").Elements("brands").Elements("brand")
                                select new
                                {
                                    BrandID = (Int32)Brand.Element("ID"),
                                    BrandName = (string)Brand.Element("Name"),
                                };
                foreach (var xml in xmlBrands)
                {
                    parameters = new object[]
                    {   
                        new SqlParameter("db_brandID",xml.BrandID),
                        new SqlParameter("db_brandName",xml.BrandName),
                        new SqlParameter("db_dealerID",dealerId),
                        new SqlParameter("db_ourbID",DBNull.Value)
                    };

                    command = context.CreateStoreCommand("sp_DealersBrands_Add", CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }

                command = context.CreateStoreCommand("sp_BrandsDealersToOur", CommandType.StoredProcedure);
                using (context.Connection.CreateConnectionScope())
                {
                    command.ExecuteNonQuery();
                }

                var xmlGroups = from Group in doc.Elements("dealer").Elements("groups").Elements("group")
                                select new
                                {
                                    GroupID = (Int32)Group.Element("ID"),
                                    GroupName = (string)Group.Element("Name"),
                                    ParentID = (Int32)Group.Element("ParID")
                                };

                foreach (var xml in xmlGroups)
                {
                    parameters = new object[]
                    {   
                        new SqlParameter("dg_dealerID",dealerId),
                        new SqlParameter("dg_dgrID",xml.GroupID),
                        new SqlParameter("dg_parGrID",xml.ParentID),
                        new SqlParameter("dg_dgrName",xml.GroupName),
                        new SqlParameter("dg_ourgrID",DBNull.Value)
                    };

                    command = context.CreateStoreCommand("sp_DealersGroup_Add", CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }

                command = context.CreateStoreCommand("sp_GroupsDealersToOur", CommandType.StoredProcedure);
                using (context.Connection.CreateConnectionScope())
                {
                    command.ExecuteNonQuery();
                }

                var xmlProducts = from Product in doc.Elements("dealer").Elements("products").Elements("product")
                                  from Price in Product.Elements("price")
                                  select new
                                  {
                                      ProdID = (Int32)Product.Element("ID"),
                                      ProdName = (string)Product.Element("Name"),
                                      BrandID = (Int32)Product.Element("BrandID"),
                                      GroupID = (Int32)Product.Element("GroupID"),
                                      Unit = (Product.Elements("Unit").Any() ? (string)Product.Element("Unit") : "шт."),
                                      PhotoUrl = (string)Product.Element("PhotoUrl"),
                                      Price = (double)Price.Element("Value"),
                                      Currency = (string)Price.Element("Currency"),
                                      State = (string)Price.Element("State"),
                                      Guarantee = (Product.Elements("Guarantee").Any() ? (Int32)Product.Element("Guarantee") : 0),
                                      Description = (string)Product.Element("Description")
                                  };

                foreach (var xml in xmlProducts)
                {
                    parameters = new object[]
                    {   
                        new SqlParameter("dp_dealerID",dealerId),
                        new SqlParameter("dp_prodID",xml.ProdID),
                        new SqlParameter("dp_prodName",xml.ProdName),
                        new SqlParameter("dp_grID",xml.GroupID),
                        new SqlParameter("dp_brandID",xml.BrandID),
                        new SqlParameter("dp_unit",xml.Unit),
                        new SqlParameter("dp_guarantee",xml.Guarantee),
                        new SqlParameter("dp_photoUrl", xml.PhotoUrl),
                        new SqlParameter("dp_ourpID",DBNull.Value),
                        new SqlParameter("dp_descr",xml.Description),
                    };

                    command = context.CreateStoreCommand("sp_DealersProducts_Add", CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }

                    parameters = new object[]
                    {   
                        new SqlParameter("pd_dealerID",dealerId),
                        new SqlParameter("pd_prodID",xml.ProdID),
                        new SqlParameter("pd_price",xml.Price),
                        new SqlParameter("currency",xml.Currency),
                        new SqlParameter("pd_state",xml.State),
                    };

                    command = context.CreateStoreCommand("sp_ProductsByDealers_Add", CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }

                command = context.CreateStoreCommand("sp_ProductsDealersToOur", CommandType.StoredProcedure);
                using (context.Connection.CreateConnectionScope())
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}