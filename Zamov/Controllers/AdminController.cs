//using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Microsoft.Data.Extensions;
using Zamov.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.Security;
using System.Data;
using Zamov.Helpers;
using System;
using System.Globalization;
using System.Web.Profile;
using System.Configuration;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Data.Objects.DataClasses;

namespace Zamov.Controllers
{
	[Authorize(Roles = "Administrators")]
	[BreadCrumb(ResourceName = "Administration", Url = "/Admin")]
	public class AdminController : Controller
	{
	    private readonly int _adminPageSize = 50;

		public ActionResult Index()
		{
			return View();
		}

		#region Currencies

		[BreadCrumb(ResourceName = "Currencies", Url = "/Admin/Currencies")]
		public ActionResult Currencies()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<Currencies> currencies = context.Currencies.Select(c => c).ToList();
				return View(currencies);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InsertCurrency(FormCollection form)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				Currencies currency = new Currencies();
				currency.Name = form["currencyName"];
				currency.ShortName = form["currencyShortName"];
				currency.Sign = form["currencySign"];
				context.AddToCurrencies(currency);
				context.SaveChanges();
			}
			return RedirectToAction("Currencies");
		}

		public ActionResult DeleteCurrency(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				Currencies currency = (from c in context.Currencies where c.Id == id select c).First();
				context.DeleteObject(currency);
				context.SaveChanges();
			}
			return RedirectToAction("Currencies");
		}

		#endregion

        #region Brands

        [BreadCrumb(ResourceName = "Brands", Url = "/Admin/Brands")]
        public ActionResult Brands()
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                List<Brands> brands = context.Brands.Select(c => c).ToList();
                return View(brands);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Brands(List<Brands> brands)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                List<Brands> brandsContext = context.Brands.Select(c => c).ToList();
                brandsContext.ForEach(bc =>
                {
                    bc.brandName = (from b in brands where bc.brandID == b.brandID select b.brandName).First();
                });
                context.SaveChanges();
                return View(brands);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertBrand(FormCollection form)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                Brands brands = new Brands();
                brands.brandName= form["brandName"];
                context.AddToBrands(brands);
                context.SaveChanges();
            }
            return RedirectToAction("Brands");
        }

        #endregion

        #region Manufacturers

        //[BreadCrumb(ResourceName = "Manufacturers", Url = "/Admin/Manufacturers")]
        //public ActionResult Manufacturers()
        //{
        //    using (ZamovStorage context = new ZamovStorage())
        //    {
        //        List<Manufacturer> manufacturers = context.Manufacturer.Select(c => c).ToList();
        //        return View(manufacturers);
        //    }
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult InsertManufacturer(FormCollection form)
        //{
        //    using (ZamovStorage context = new ZamovStorage())
        //    {
        //        Manufacturer manufacturer = new Manufacturer();
        //        manufacturer.Name = form["manufacturerName"];
        //        context.AddToManufacturer(manufacturer);
        //        context.SaveChanges();
        //    }
        //    return RedirectToAction("Manufacturers");
        //}

        //public ActionResult DeleteManufacturer(int id)
        //{
        //    using (ZamovStorage context = new ZamovStorage())
        //    {
        //        Manufacturer manufacturer = (from c in context.Manufacturer where c.Id == id select c).First();
        //        context.DeleteObject(manufacturer);
        //        context.SaveChanges();
        //    }
        //    return RedirectToAction("Manufacturers");
        //}

        #endregion

		#region Cities
		[BreadCrumb(ResourceName = "Cities", Url = "/Admin/Cities")]
		public ActionResult Cities()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<City> cities = context.Cities.OrderBy(c => c.IndexNumber).Select(c => c).ToList();
				return View(cities);
			}
		}

		public ActionResult DeleteCity(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				City city = (from c in context.Cities where c.Id == id select c).First();
				context.DeleteObject(city);
				context.SaveChanges();
				context.DeleteTranslations(id, (int)ItemTypes.City);
			}
			return RedirectToAction("Cities");
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InsertCity(FormCollection form)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				City city = new City();
				city.Name = form["cityName"];
				city.Names.Clear();
				city.Names["ru-RU"] = form["cityRusName"];
				city.Names["uk-UA"] = form["cityUkrName"];
				city.Enabled = form["cityEnabled"].Contains("true");
			    city.IndexNumber = int.Parse(form["cityIndex"]);
				context.AddToCities(city);
				context.SaveChanges();
				context.UpdateTranslations(city.NamesXml);
			}
			return RedirectToAction("Cities");
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateCities(FormCollection form)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			if (!string.IsNullOrEmpty(form["updates"]))
			{
				Dictionary<string, Dictionary<string, string>> updates = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
					form["updates"]
					);
				foreach (string key in updates.Keys)
				{
					int itemId = int.Parse(key);
					Dictionary<string, string> translations = updates[key];
					List<TranslationItem> translationItems = new List<TranslationItem>();
					translationItems = (from tr in translations select new TranslationItem { ItemId = itemId, ItemType = ItemTypes.City, Language = tr.Key, Translation = tr.Value }).ToList();
					string translationXml = Utils.CreateTranslationXml(translationItems);
					using (ZamovStorage context = new ZamovStorage())
					{
						context.UpdateTranslations(translationXml);
					}
				}
			}
            if (!string.IsNullOrEmpty(form["updateCity"]))
			{
                Dictionary<string, Dictionary<string, string>> updateCity = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(form["updateCity"]);
                using (ZamovStorage context = new ZamovStorage())
                {
                    foreach (string key in updateCity.Keys)
                    {
                        int id = int.Parse(key);
                        City city = context.Cities.Select(c => c).Where(c => c.Id == id).First();
                        Dictionary<string, string> cityFields = updateCity[key];
                        foreach (var field in cityFields.Keys)
                        {
                            switch (field)
                            {
                                case "enabled":
                                    city.Enabled = bool.Parse(cityFields[field]);
                                    break;
                                case "index":
                                    city.IndexNumber = int.Parse(cityFields[field]);
                                    break;
                            }
                        }
                    }
                    context.SaveChanges(true);
                }
			}
			return RedirectToAction("Cities");
		}
		#endregion

		#region Dealers
		[BreadCrumb(ResourceName = "Dealers", Url = "/Admin/Dealers")]
		public ActionResult Dealers()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<Dealer> dealers = context.Dealers.OrderBy(d => d.IndexNumber).Select(d => d).ToList();
				return View(dealers);
			}
		}

        class DealersUpdate
        {
            internal int Id;
            internal bool Top;
            internal bool Enabled;
            internal int? OrderLifeTime;
            internal bool LoadXml;
            internal int IndexNumber;
        }

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Dealers(FormCollection form)
		{
			PostData postData = form.ProcessPostData();

			Func<string, int?> getNullableInt = (str =>
			{
				if (string.IsNullOrEmpty(str))
					return null;
				return int.Parse(str);
			});

            var dealerUpdates = (from pd in postData
								 select new DealersUpdate
								 {
									 Id = int.Parse(pd.Key),
									 Top = pd.Value["top"].Contains("true"),
									 Enabled = pd.Value["enabled"].Contains("true"),
									 OrderLifeTime = getNullableInt(pd.Value["orderLifeTime"]),
                                     LoadXml = pd.Value["loadXml"].Contains("true"),
								 }).ToArray();

            for (int i = 0; i < dealerUpdates.Count(); i++)
            {
                dealerUpdates[i].IndexNumber = i + 1;
            }

			using (ZamovStorage context = new ZamovStorage())
			{
				List<Dealer> dealers = (from dealer in context.Dealers select dealer).ToList();
				dealers.ForEach(d =>
				{
					d.TopDealer = (from du in dealerUpdates where du.Id == d.Id select du.Top).First();
					d.Enabled = (from du in dealerUpdates where du.Id == d.Id select du.Enabled).First();
					d.OrderLifeTime = (from du in dealerUpdates where du.Id == d.Id select du.OrderLifeTime).First();
                    d.LoadXml = (from du in dealerUpdates where du.Id == d.Id select du.LoadXml).First();
                    d.IndexNumber = (from du in dealerUpdates where du.Id == d.Id select du.IndexNumber).First();
				});
				context.SaveChanges();
			}
            HttpContext.Cache.ClearCache(k => true);
			return RedirectToAction("Dealers");
		}

		public ActionResult DeleteDealer(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				Dealer dealer = (from d in context.Dealers where d.Id == id select d).First();
				context.DeleteObject(dealer);
				context.SaveChanges();
			}
			return RedirectToAction("Dealers");
		}

		public ActionResult AddUpdateDealer(int id)
		{
			string bcText = ResourcesHelper.GetResourceString("CreateDealer");
			if (id > 0)
			{
				bcText = ResourcesHelper.GetResourceString("EditDealer");
				using (ZamovStorage context = new ZamovStorage())
				{
					Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
					ViewData["dealer"] = dealer;
				}
			}
			BreadCrumbsExtensions.AddBreadCrumb(HttpContext, bcText, "/Admin/AddUpdateDealer/" + id);
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AddUpdateDealer(FormCollection form)
		{
			int dealerId = int.Parse(form["dealerId"]);
			Dealer dealer = null;
			using (ZamovStorage context = new ZamovStorage())
			{
				if (dealerId >= 0)
					dealer = context.Dealers.Select(d => d).Where(d => d.Id == dealerId).First();
				else
					dealer = new Dealer();
				dealer.Name = form["name"];
				dealer.Names["ru-RU"] = form["rName"];
				dealer.Names["uk-UA"] = form["uName"];
				dealer.Descriptions["ru-RU"] = Server.HtmlDecode(form["rDescription"]);
				dealer.Descriptions["uk-UA"] = Server.HtmlDecode(form["uDescription"]);
				dealer.Enabled = form["enabled"].Contains("true");
				Group deleted = new Group();
				deleted.Enabled = false;
				dealer.Groups.Add(deleted);
				deleted.Name = "TRASH";
				deleted.Names["uk-UA"] = HttpContext.GetGlobalResourceObject("Resources", "Deleted", CultureInfo.GetCultureInfo("uk-UA")).ToString();
				deleted.Names["ru-RU"] = HttpContext.GetGlobalResourceObject("Resources", "Deleted", CultureInfo.GetCultureInfo("ru-RU")).ToString();
				if (!string.IsNullOrEmpty(Request.Files["logoImage"].FileName))
				{
					HttpPostedFileBase file = Request.Files["logoImage"];
					dealer.LogoType = file.ContentType;
					BinaryReader reader = new BinaryReader(file.InputStream);
					dealer.LogoImage = reader.ReadBytes((int)file.InputStream.Length);
				}
				if (dealerId < 0)
					context.AddToDealers(dealer);
				context.SaveChanges();
				context.UpdateTranslations(dealer.NamesXml);
				context.UpdateTranslations(dealer.DescriptionsXml);
				context.UpdateTranslations(deleted.NamesXml);
			}
			return RedirectToAction("Dealers");
		}

		public ActionResult DealerContacts(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				//ViewData["dealerId"] = id;
				Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
				ViewData["dealer"] = dealer;
				List<DealerContacts> dealerContacts = (from dc in context.DealerContacts.Include("Dealers") where dc.Dealers.Id == id select dc).ToList();
				return View(dealerContacts);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DealerContacts(FormCollection form)
		{
			int dealerId = int.Parse(form["dealerId"]);
			PostData postData = form.ProcessPostData("dealer", "dealerId");
			//PostData updates = new PostData();
			//foreach (var item in postData)
			//{
			//    updates.Add(item.Key, item.Value.ToDictionary(v => v.Key, v => v.Value));
			//}
			//foreach (var item in updates)
			//{
			//    item.Value["rate"] = item.Value["rate"].Replace(".", ",");
			//}

			using (ZamovStorage context = new ZamovStorage())
			{
				List<DealerContacts> dealerContacts = (from dc in context.DealerContacts.Include("Dealers") where dc.Dealers.Id == dealerId select dc).ToList();
				foreach (var item in dealerContacts)
				{
					item.ContactType = int.Parse(postData[item.Id.ToString()]["type"]);
					item.Enabled = bool.Parse(postData[item.Id.ToString()]["enabled"]);
					item.Value = postData[item.Id.ToString()]["contact"];
				}
				context.SaveChanges();
			}
			return RedirectToAction("DealerContacts", new { id = dealerId });
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InsertDealerContact(FormCollection form)
		{
			int dealerId;
			using (ZamovStorage context = new ZamovStorage())
			{
				dealerId = int.Parse(form["dealerId"]);
				Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == dealerId).FirstOrDefault();
				var dealerContacts = new DealerContacts();
				dealerContacts.Dealers = dealer;
				dealerContacts.Value = form["contact"];
				dealerContacts.ContactType = int.Parse(form["type"]);
				dealerContacts.Enabled = form["enabled"].Contains("true");
				context.AddToDealerContacts(dealerContacts);
				context.SaveChanges();
			}
			return RedirectToAction("DealerContacts", new { id = dealerId });
		}

		public ActionResult DeleteDealerContact(int id)
		{
			int dealerId;
			using (var context = new ZamovStorage())
			{
				DealerContacts dealerContact = (from d in context.DealerContacts where d.Id == id select d).First();
				dealerId = (int)dealerContact.DealersReference.EntityKey.EntityKeyValues[0].Value;
				context.DeleteObject(dealerContact);
				context.SaveChanges();
			}
			return RedirectToAction("DealerContacts", new { id = dealerId });
		}

		public ActionResult DealersBrands()
		{
			var dealerBrand = new List<DealerBindBrandPresent>();
			using (var context = new ZamovStorage())
			{
				DbCommand command = context.CreateStoreCommand("sp_DealersBrandsNew_Select", CommandType.StoredProcedure);

				using (context.Connection.CreateConnectionScope())
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var d = new DealerBindBrandPresent
							{
                                IdDealer = Convert.ToInt32(reader["db_dealerID"]),
                                NameDealer = Convert.ToString(reader["dealerName"]),
								IdDealerBrand = Convert.ToInt32(reader["dbID"]),
								NameBrand = Convert.ToString(reader["brandName"]),
								NameBrandMain = Convert.ToString(reader["brandNameMain"])
							};
							dealerBrand.Add(d);
						}
					}
				}
			}
			return View(dealerBrand);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DealersBrands(FormCollection form)
		{
            PostData postData = form.ProcessPostData("bind", "create", "save");

            using (var context = new ZamovStorage())
            {
                foreach (var brand in postData)
                {
                    bool bind;
                    Boolean.TryParse(brand.Value["bind"], out bind);
                    bool create;
                    Boolean.TryParse(brand.Value["create"], out create);

                    if (!bind && !create) continue;
                    object[] parameters =
                        {
                            new SqlParameter("bNameOur", brand.Value["name"]),
                            new SqlParameter("bID", brand.Key),
                        };

                    DbCommand command =
                        context.CreateStoreCommand(
                            bind ? "sp_DealersBrandsNew_Bind" : "sp_DealersBrandsNew_Add",
                            CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
			return RedirectToAction("DealersBrands");
		}

        public ActionResult DealersGroups(int page)
		{
			var dealerGrops = new List<DealerBindGroupPresent>();
			using (var context = new ZamovStorage())
			{
				DbCommand command = context.CreateStoreCommand("sp_DealersGroupsNew_Select", CommandType.StoredProcedure);

				using (context.Connection.CreateConnectionScope())
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var d = new DealerBindGroupPresent
							{
                                IdDealer = Convert.ToInt32(reader["dg_dealerID"]),
                                NameDealer = Convert.ToString(reader["dealerName"]),
								IdDealerGroup = Convert.ToInt32(reader["dgID"]),
								NameGroup = Convert.ToString(reader["groupName"]),
								NameGroupMain = Convert.ToString(reader["groupNameMain"])
							};
							dealerGrops.Add(d);
						}
					}
				}

				var categories = context.Categories.Where(ca => ca.Enabled && ca.Parent == null).OrderBy(ca=>ca.IndexNumber).Select(ca => ca).ToList();

                List<SelectListItem> categ = categories.Select(ca => new SelectListItem { Text = ca.Name, Value = ca.Id.ToString() }).ToList();

				ViewData["Categories"] = new SelectList(categ, "Value", "Text");
			}
            int pageSize = _adminPageSize;
            int countDealerGrops = dealerGrops.Count;
            int totalPages = (int)Math.Ceiling((double)countDealerGrops / pageSize);
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            return View(dealerGrops.Skip((page - 1) * pageSize).Take(pageSize).ToList());
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DealersGroups(FormCollection form)
		{
			PostData postData = form.ProcessPostData("bind", "create", "save");

            using (var context = new ZamovStorage())
            {
                foreach (var brand in postData)
                {
                    bool bind;
                    Boolean.TryParse(brand.Value["bind"], out bind);
                    bool create;
                    Boolean.TryParse(brand.Value["create"], out create);

                    if (!bind && !create) continue;

                    object[] parameters = null;
                    string nameProcedure = null;

                    if (bind)
                    {
                        nameProcedure = "sp_DealersGroupsNew_Bind";
                        parameters = new object[]
                                         {
                                             new SqlParameter("grNameOur", brand.Value["name"]),
                                             new SqlParameter("grID", brand.Key),
                                         };
                    }
                    else
                    {
                        nameProcedure = "sp_DealersGroupsNew_Add";
                        parameters = new object[]
                                         {
                                             new SqlParameter("grNameOur", brand.Value["name"]),
                                             new SqlParameter("catID", string.IsNullOrEmpty(brand.Value["category"]) ? "-1" : brand.Value["category"]),
                                             new SqlParameter("grID", brand.Key),
                                         };
                    }

                    DbCommand command = context.CreateStoreCommand(nameProcedure, CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
			return RedirectToAction("DealersGroups");
		}

		public ActionResult DealersProducts(int page)
		{
			var dealerProduct = new List<DealerBindProductPresent>();
			using (var context = new ZamovStorage())
			{
				DbCommand command = context.CreateStoreCommand("sp_DealersProductsNew_Select", CommandType.StoredProcedure);

				using (context.Connection.CreateConnectionScope())
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var d = new DealerBindProductPresent
							{
								IdDealer = Convert.ToInt32(reader["dp_dealerID"]),
                                NameDealer = Convert.ToString(reader["dealerName"]),
								IdDealerProduct = Convert.ToInt32(reader["dpID"]),
								NameProduct = Convert.ToString(reader["ProductName"]),
								NameProductMain = Convert.ToString(reader["ProductNameMain"])
							};
							dealerProduct.Add(d);
						}
					}
				}
			}
            int pageSize = _adminPageSize;
            int countdealerProduct = dealerProduct.Count;
            int totalPages = (int)Math.Ceiling((double)countdealerProduct / pageSize);
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            return View(dealerProduct.Skip((page - 1) * pageSize).Take(pageSize).ToList());
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DealersProducts(FormCollection form)
		{
            PostData postData = form.ProcessPostData("bind", "create", "save");

            using (var context = new ZamovStorage())
            {
                foreach (var prod in postData)
                {
                    bool bind;
                    Boolean.TryParse(prod.Value["bind"], out bind);
                    bool create;
                    Boolean.TryParse(prod.Value["create"], out create);

                    if (!bind && !create) continue;
                    object[] parameters =
                        {
                            new SqlParameter("pNameOur", prod.Value["name"]),
                            new SqlParameter("pID", prod.Key),
                        };

                    if (create)
                    {
                        int prodDealerId = Convert.ToInt32(prod.Key);
                        DealersProducts product = context.DealersProducts.FirstOrDefault(p => p.dpID == prodDealerId);
                        string photoUrl = ImageHelper.LoadImageFromURL(product.dp_photoUrl, string.Format("d{0}pa{1}", product.dp_dealerID, product.dpID));
                        product.dp_photoUrl = photoUrl;
                        context.SaveChanges();
                    }

                    DbCommand command =
                        context.CreateStoreCommand(
                            bind ? "sp_DealersProductsNew_Bind" : "sp_DealersProductsNew_Add",
                            CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
			return RedirectToAction("DealersProducts");
		}

		#endregion

		#region Categories
		[BreadCrumb(ResourceName = "Categories", Url = "/Admin/Categories")]
		public ActionResult Categories()
		{
			return View();
		}

		public ActionResult CategoriesList(int? id, int level)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<Category> categories = context.Categories.Select(c => c).ToList();
				if (id == null)
					categories = categories.Select(c => c).Where(c => c.Parent == null).OrderBy(ca=> ca.IndexNumber).ToList();
				else
					categories = categories.Select(c => c).Where(c => c.Parent != null && c.Parent.Id == id.Value).OrderBy(ca=> ca.IndexNumber).ToList();

				ViewData["level"] = level;
				if (id != null)
					ViewData["id"] = id.Value;
				return View(categories);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateCategories(FormCollection form)
		{
            Dictionary<string, Dictionary<string, string>> updates = form.ProcessPostData("enablities", "updateCategory");// serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(

			foreach (string key in updates.Keys)
			{
				int itemId = int.Parse(key);
				Dictionary<string, string> translations = updates[key];
				List<TranslationItem> translationItems = new List<TranslationItem>();
				translationItems = (from tr in translations where !tr.Key.StartsWith("sub") && tr.Key != "Enabled" select new TranslationItem { ItemId = itemId, ItemType = ItemTypes.Category, Language = tr.Key, Translation = tr.Value }).ToList();
				List<TranslationItem> subcategoryNames = (from tr in translations where tr.Key.StartsWith("sub") && tr.Key != "Enabled" select new TranslationItem { ItemId = itemId, ItemType = ItemTypes.SubCategoriesName, Language = tr.Key.Replace("sub", ""), Translation = tr.Value }).ToList();
				translationItems.AddRange(subcategoryNames);
				string translationXml = Utils.CreateTranslationXml(translationItems);
				using (ZamovStorage context = new ZamovStorage())
				{
					context.UpdateTranslations(translationXml);
				}
			}

			if (!string.IsNullOrEmpty(form["enablities"]))
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				Dictionary<string, string> enables = serializer.Deserialize<Dictionary<string, string>>(form["enablities"]);
				using (ZamovStorage context = new ZamovStorage())
				{
					foreach (string key in enables.Keys)
					{
						int id = int.Parse(key);
						Category category = context.Categories.Select(c => c).Where(c => c.Id == id).First();
						category.Enabled = bool.Parse(enables[key]);
					}
					context.SaveChanges(true);
				}
			}
            if (!string.IsNullOrEmpty(form["updateCategory"]))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, Dictionary<string, string>> updateCategory = 
                    serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(form["updateCategory"]);
                using (ZamovStorage context = new ZamovStorage())
                {
                    foreach (string key in updateCategory.Keys)
                    {
                        int id = int.Parse(key);
                        Category category = context.Categories.Select(c => c).Where(c => c.Id == id).First();
                        Dictionary<string, string> cityFields = updateCategory[key];
                        foreach (var field in cityFields.Keys)
                        {
                            switch (field)
                            {
                                case "index":
                                    category.IndexNumber = int.Parse(cityFields[field]);
                                    break;
                            }
                        }
                    }
                    context.SaveChanges(true);
                }
            }
		    HttpContext.Cache.ClearCategoriesCache();
			return RedirectToAction("Categories");
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InsertCategory(FormCollection form)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				int parentId = int.Parse(form["parentId"]);
				Category parent = null;
				if (parentId >= 0)
					parent = context.Categories.Select(c => c).Where(c => c.Id == parentId).First();
				Category category = new Category();
				category.Parent = parent;
				category.Name = form["categoryUkrName"];
				category.Names.Clear();
				category.Names["ru-RU"] = form["categoryRusName"];
				category.Names["uk-UA"] = form["categoryUkrName"];
				category.Enabled = form["categoryEnabled"].ToLowerInvariant().IndexOf("true") > -1;
                category.IndexNumber = int.Parse(form["categoryIndex"]);
				context.AddToCategories(category);
				context.SaveChanges();
				context.UpdateTranslations(category.NamesXml);
			}
			HttpContext.Cache.ClearCategoriesCache();
			return RedirectToAction("Categories");
		}

		public ActionResult DeleteCategory(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				Category category = (from c in context.Categories.Include("Groups") where c.Id == id select c).First();
				EntityCollection<Group> groups = category.Groups;
				foreach (var item in groups)
				{
					item.CategoryReference = null;
				}

				context.DeleteObject(category);

				context.SaveChanges();
				context.DeleteTranslations(id, (int)ItemTypes.Category);
			}
			return RedirectToAction("Categories");
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult UpdateCategoryImage(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				CategoryImage image = context.CategoryImages.Select(pi => pi).Where(pi => pi.Category.Id == id).SingleOrDefault();
				int imageId = (image != null) ? image.Id : int.MinValue;
				ViewData["imageId"] = imageId;
				ViewData["categoryId"] = id;
				return View();
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateCategoryImage(int id, int categoryId)
		{
			if (!string.IsNullOrEmpty(Request.Files["newImage"].FileName))
			{
				CategoryImage image = null;
				image = new CategoryImage();
				IEnumerable<KeyValuePair<string, object>> productKeyValues = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Id", categoryId) };
				EntityKey product = new EntityKey("ZamovStorage.Categories", productKeyValues);
				image.CategoryReference.EntityKey = product;
				HttpPostedFileBase file = Request.Files["newImage"];
				image.ImageType = file.ContentType;
				BinaryReader reader = new BinaryReader(file.InputStream);
				image.Image = reader.ReadBytes((int)file.InputStream.Length);
				using (ZamovStorage context = new ZamovStorage())
				{
					context.CleanupCategoryImages(categoryId);
					context.AddToCategoryImages(image);
					context.SaveChanges();
				}
			}
			Response.Write("<script type=\"text/javascript\">top.closeImageDialog();</script>");
		}

		public ActionResult CategoryDescription(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				int typeId = (int)ItemTypes.CategoryDescription;
				int titleTypeId = (int)ItemTypes.CategoryDescriptionTitle;

				var items = context.Translations.Where(tr => tr.ItemId == id).Where(tr => tr.TranslationItemTypeId == typeId)
					.ToDictionary(ks => ks.Language, es => es.Text);

				var titles = context.Translations.Where(tr => tr.ItemId == id).Where(tr => tr.TranslationItemTypeId == titleTypeId)
					.ToDictionary(ks => ks.Language, es => es.Text);

				if (items.ContainsKey("ru-RU"))
					ViewData["ruDescription"] = items["ru-RU"];
				if (items.ContainsKey("uk-UA"))
					ViewData["uaDescription"] = items["uk-UA"];
				if (items.ContainsKey("ru-RU"))
					ViewData["ruTitle"] = titles["ru-RU"];
				if (items.ContainsKey("uk-UA"))
					ViewData["uaTitle"] = titles["uk-UA"];
			}
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CategoryDescription(int id, string ruTitle, string uaTitle, string ruDescription, string uaDescription)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				int typeId = (int)ItemTypes.CategoryDescription;
				int titleTypeId = (int)ItemTypes.CategoryDescriptionTitle;

				var ruTranslation = context.Translations.Where(tr => tr.ItemId == id)
					.Where(tr => tr.TranslationItemTypeId == typeId)
					.Where(tr => tr.Language == "ru-RU").Select(tr => tr).FirstOrDefault();

				if (ruTranslation == null)
				{
					ruTranslation = new Translation { ItemId = id, Language = "ru-RU", TranslationItemTypeId = typeId };
					context.AddToTranslations(ruTranslation);
				}

				var uaTranslation = context.Translations.Where(tr => tr.ItemId == id)
					.Where(tr => tr.TranslationItemTypeId == typeId)
					.Where(tr => tr.Language == "uk-UA").Select(tr => tr).FirstOrDefault();

				if (uaTranslation == null)
				{
					uaTranslation = new Translation { ItemId = id, Language = "uk-UA", TranslationItemTypeId = typeId };
					context.AddToTranslations(uaTranslation);
				}

				var ruTitleTranslation = context.Translations.Where(tr => tr.ItemId == id)
					.Where(tr => tr.TranslationItemTypeId == titleTypeId)
					.Where(tr => tr.Language == "ru-RU").Select(tr => tr).FirstOrDefault();

				if (ruTitleTranslation == null)
				{
					ruTitleTranslation = new Translation { ItemId = id, Language = "ru-RU", TranslationItemTypeId = titleTypeId };
					context.AddToTranslations(ruTitleTranslation);
				}

				var uaTitleTranslation = context.Translations.Where(tr => tr.ItemId == id)
					.Where(tr => tr.TranslationItemTypeId == titleTypeId)
					.Where(tr => tr.Language == "uk-UA").Select(tr => tr).FirstOrDefault();

				if (uaTitleTranslation == null)
				{
					uaTitleTranslation = new Translation { ItemId = id, Language = "uk-UA", TranslationItemTypeId = titleTypeId };
					context.AddToTranslations(uaTitleTranslation);
				}

				ruTranslation.Text = HttpUtility.HtmlDecode(ruDescription);
				uaTranslation.Text = HttpUtility.HtmlDecode(uaDescription);
				ruTitleTranslation.Text = ruTitle;
				uaTitleTranslation.Text = uaTitle;

				context.SaveChanges();
			}
			return RedirectToAction("Categories");
		}

		#endregion

		#region Groups
		[BreadCrumb(ResourceName = "Groups", Url = "/Admin/Groups")]
		[Authorize(Roles = "Administrators")]
		public ActionResult GroupsAddit()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<CategoryPresentation> categories = context.GetTranslatedCategories(SystemSettings.CurrentLanguage, true, null, false)
					.Select(c => new CategoryPresentation
					{
						Id = c.Entity.Id,
						Name = c.Translation.Text,
						ParentId = c.Entity.Parent.Id,
                        IndexNumber = c.Entity.IndexNumber
					})
                    .OrderBy(cp => cp.IndexNumber)
                    .ToList();
				categories.ForEach(c => c.PickChildren(categories));
				categories = categories.Where(c => c.ParentId == null).ToList();
				ViewData["categories"] = categories;
				return View();
			}
		}

		[Authorize(Roles = "Administrators")]
		public ActionResult GroupsAdditList(int? id, int level, List<CategoryPresentation> categories)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<GroupsAdditional> groups = context.GroupsAdditional.
                    Select(g => g).Where(g => !g.gr_deleted && g.gr_Name != "TRASH").ToList();
				if (id == null)
                    groups = groups.Select(g => g).Where(g => g.Parent == null).OrderBy(ga => ga.gr_IndexNumber).ToList();
				else
                    groups = groups.Select(g => g).Where(g => g.Parent != null && g.Parent.grID == id.Value).OrderBy(ga => ga.gr_IndexNumber).ToList();
				ViewData["level"] = level;
				ViewData["categories"] = categories;
				return View(groups);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Administrators")]
		public ActionResult InsertGroup(string groupName, string groupUkrName, string groupRusName, int groupIndex, bool displayImages, bool enabled, int parentId, int categoryId)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				GroupsAdditional parent = null;
				if (parentId >= 0)
					parent = context.GroupsAdditional.Select(c => c).Where(c => c.grID == parentId).First();
				GroupsAdditional groupAddit = new GroupsAdditional();
				groupAddit.Parent = parent;
				groupAddit.gr_Name = groupUkrName;
				groupAddit.Names.Clear();
				groupAddit.Names["ru-RU"] = groupRusName;
				groupAddit.Names["uk-UA"] = groupUkrName;
			    groupAddit.gr_IndexNumber = groupIndex;
				groupAddit.gr_enabled = enabled;
				groupAddit.gr_displayProductImages = displayImages;
				if (parentId < 0)
					groupAddit.CategoriesReference.EntityKey = new EntityKey("ZamovStorage.Categories", "Id", categoryId);
				context.AddToGroupsAdditional(groupAddit);
				context.SaveChanges();
				context.UpdateTranslations(groupAddit.NamesXml);
			}
			HttpContext.Cache.ClearCategoriesCache();
			return RedirectToAction("GroupsAddit");
		}

		[HttpPost]
		[Authorize(Roles = "Administrators")]
		public ActionResult UpdateGroups(FormCollection form)
		{
			Dictionary<string, Dictionary<string, string>> updates = form.ProcessPostData();
			List<TranslationItem> translationItems = new List<TranslationItem>();
			using (ZamovStorage context = new ZamovStorage())
			{
				foreach (string key in updates.Keys)
				{
					int itemId = int.Parse(key);
					Dictionary<string, string> translations =
						(from item in updates[key] where item.Key != "itemId" && item.Key != "enabled" && item.Key != "categoryId" select item)
						.ToDictionary(i => i.Key, i => i.Value);
					translationItems.AddRange((from tr in translations
											   select
												   new TranslationItem { ItemId = itemId, ItemType = ItemTypes.GroupAdditional, Language = tr.Key, Translation = tr.Value }).ToList());

				    int indexNumber = int.Parse(updates[key]["index"]);
                    bool enabled = (updates[key].ContainsKey("enabled") && (updates[key]["enabled"].Contains("true") || updates[key]["enabled"] == "on"));
					bool displayImages = (updates[key].ContainsKey("displayImages")
						&& (updates[key]["displayImages"].Contains("true") || updates[key]["displayImages"] == "on"));
					int? categoryId = null;
					if (updates[key].ContainsKey("categoryId"))
						categoryId = Convert.ToInt32(updates[key]["categoryId"]);

					GroupsAdditional groupAddit = context.GroupsAdditional.Select(g => g).Where(g => g.grID == itemId).First();
				    groupAddit.gr_IndexNumber = indexNumber;
                    groupAddit.gr_enabled = enabled;
					groupAddit.gr_displayProductImages = displayImages;
					if (categoryId != null)
					{
						groupAddit.CategoriesReference.EntityKey = new EntityKey("ZamovStorage.Categories", "Id", categoryId);
					}
				}
				string translationXml = Utils.CreateTranslationXml(translationItems);
				context.SaveChanges();
				context.UpdateTranslations(translationXml);
			}

			using (ZamovStorage context = new ZamovStorage())
			{
				List<GroupsAdditional> groups = context.GroupsAdditional.Include("Categories").Select(gr => gr).ToList();
				foreach (var gr in groups)
				{
					if (gr.Parent != null)
					{
						int catId = groups.Where(g => g.grID == gr.Parent.grID).Select(g => g.Categories.Id).FirstOrDefault();
						if (gr.Categories.Id != catId)
						{
							gr.Parent = null;
						}
					}
				}
				context.SaveChanges();
			}
			HttpContext.Cache.ClearCategoriesCache();
			return RedirectToAction("GroupsAddit");
		}

		[Authorize(Roles = "Administrators, Dealers")]
		public ActionResult DeleteGroup(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				GroupsAdditional groupAddit = context.GroupsAdditional.Select(g => g).Where(g => g.grID == id).First();
				groupAddit.gr_deleted = true;
				context.SaveChanges();
			}
			HttpContext.Cache.ClearCategoriesCache();
			return RedirectToAction("GroupsAddit");
		}
		#endregion

		#region Products Additional
		[Authorize(Roles = "Administrators")]
		[BreadCrumb(ResourceName = "Products", Url = "/Admin/Products")]
		public ActionResult ProductsAddit(int? idGroup)
		{
			ViewData["idGroup"] = idGroup;
			var productsAddit = new List<ProductsAdditional>();
			var groupAdditionalAll = new List<GroupResentation>();
			//var sortedGroups = new List<GroupResentation>();
			using (ZamovStorage context = new ZamovStorage())
			{
				groupAdditionalAll = (from gr in context.GroupsAdditional
										join translation in context.Translations on gr.grID equals translation.ItemId
										where translation.TranslationItemTypeId == (int)ItemTypes.GroupAdditional
											&& translation.Language == SystemSettings.CurrentLanguage
											&& gr.gr_enabled
											&& !gr.gr_deleted
                                        orderby gr.gr_IndexNumber
										select new GroupResentation
										{
											Id = gr.grID,
											Name = translation.Text,
											ParentId = (gr.Parent != null) ? (int?)gr.Parent.grID : null,
											CategoryId = gr.Categories.Id
										}
									).ToList();
				groupAdditionalAll.ForEach(ac => ac.PickChildren(groupAdditionalAll));

				if (idGroup.HasValue && idGroup.Value != -1)
				{
					GroupResentation currentGroup = groupAdditionalAll.Where(g => g.Id == idGroup.Value).SingleOrDefault();

					var listGroupsId = new List<int>();
					listGroupsId.AddRange(GetListGroupsId(currentGroup));

                    foreach (var gId in listGroupsId)
					{
						int id = gId;
						productsAddit.AddRange(context.ProductsAdditional.Include("GroupsAdditional")
						                       	.Where(pa => pa.GroupsAdditional.grID == id)
						                       	.Select(pa => pa).ToList());
					}
				}
				groupAdditionalAll = groupAdditionalAll.Select(c => c).Where(c => c.ParentId == null).ToList();
			}
			ViewData["groups"] = groupAdditionalAll;
			return View(productsAddit);
		}

		private IEnumerable<int> GetListGroupsId(GroupResentation currentGroup)
		{
			var gr = new List<int>();
			if (currentGroup.Children.Count > 0)
			{
				currentGroup.Children.ForEach(ac => gr.AddRange(GetListGroupsId(ac)));
			}
			gr.Add(currentGroup.Id);
			return gr;
		}

		[HttpPost]
		[Authorize(Roles = "Administrators")]
		public ActionResult ProductsAddit(int? idGroup, List<ProductsAdditional> prodAddit)
		{
			ViewData["idGroup"] = idGroup;
            var sortedGroups = new List<GroupResentation>();
			var productsAddit = new List<ProductsAdditional>();
			using (ZamovStorage context = new ZamovStorage())
			{
                List<GroupResentation> groupAdditionalAll = 
                                    (from gr in context.GroupsAdditional
                                      join translation in context.Translations on gr.grID equals translation.ItemId
                                      where translation.TranslationItemTypeId == (int)ItemTypes.GroupAdditional
                                          && translation.Language == SystemSettings.CurrentLanguage
                                          && gr.gr_enabled
                                          && !gr.gr_deleted
                                      orderby gr.gr_IndexNumber
                                      select new GroupResentation
                                      {
                                          Id = gr.grID,
                                          Name = translation.Text,
                                          ParentId = (gr.Parent != null) ? (int?)gr.Parent.grID : null,
                                          CategoryId = gr.Categories.Id
                                      }
                                    ).ToList();

				groupAdditionalAll.ForEach(ac => ac.PickChildren(groupAdditionalAll));

				GroupResentation currentGroup = groupAdditionalAll.Where(g => g.Id == idGroup.Value).SingleOrDefault();

				var listGroupsId = new List<int>();
				listGroupsId.AddRange(GetListGroupsId(currentGroup));

                foreach (var gId in listGroupsId)
				{
					int id = gId;
					productsAddit.AddRange(context.ProductsAdditional.Include("GroupsAdditional")
						                    .Where(pa => pa.GroupsAdditional.grID == id)
						                    .Select(pa => pa).ToList());
				}

				foreach (ProductsAdditional paUpdate in prodAddit)
				{
					ProductsAdditional p = productsAddit.Where(pa => pa.p_prodID == paUpdate.p_prodID).Select(pa => pa).FirstOrDefault();
					p.p_prodName = paUpdate.p_prodName;
					p.p_top = paUpdate.p_top;
					p.p_unit = paUpdate.p_unit;
				    p.p_photo_url = paUpdate.p_photo_url;
				    p.p_descr = paUpdate.p_descr;
					p.p_action = paUpdate.p_action;
					p.p_deleted = paUpdate.p_deleted;
					p.p_enabled = paUpdate.p_enabled;
					p.p_new = paUpdate.p_new;
				}
				context.SaveChanges();

                sortedGroups = groupAdditionalAll.Select(c => c).Where(c => c.ParentId == null).ToList();
			}
            ViewData["groups"] = sortedGroups;
			return View("ProductsAddit", productsAddit);
		}

		#endregion

		#region Mappings
		public ActionResult DealerMappings(int id, ItemTypes itemType)
		{
			List<Dealer> dealers = new List<Dealer>();
			using (ZamovStorage context = new ZamovStorage())
			{
				dealers = context.Dealers.Select(d => d).OrderBy(d=>d.IndexNumber).ToList();
			}
			ViewData["id"] = id;
			ViewData["itemType"] = itemType;
			return View(dealers);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateDealerMappingsMappings()
		{

		}
		#endregion

		#region Users
		[BreadCrumb(ResourceName = "Users", Url = "/Admin/Users")]
		public ActionResult Users(int? pageIndex, string userType, string sortField, SortDirection? sortOrder)
		{

			HttpContext.Items["sortField"] = ViewData["sortField"] = sortField;
			SortDirection sortDirection = (sortOrder == SortDirection.Ascending || sortOrder == null) ? SortDirection.Ascending : SortDirection.Descending;
			HttpContext.Items["sortDirection"] = ViewData["sortDirection"] = sortDirection;

			HttpContext.Items["userType"] = ViewData["userType"] = userType;
			List<UserPresentation> users = null;
			using (MembershipStorage context = new MembershipStorage())
			{
				users = context.GetAllUsers();
			}

			switch (userType)
			{
				case "dealers":
					users = users.Where(u => u.DealerEmployee).Select(u => u).ToList();
					users.ForEach(u => { u.DealerName = BreadCrumbAttribute.DealerName(u.DealerId); });
					break;
				case "custmers":
					users = users.Where(u => !u.DealerEmployee).Select(u => u).ToList();
					break;
				default:
					users = users.Where(u => !u.DealerEmployee).Select(u => u).ToList();
					break;
			}


			if (!string.IsNullOrEmpty(sortField))
			{
				int direction = (sortOrder == SortDirection.Ascending || sortOrder == null) ? direction = 1 : direction = -1;

				users.Sort(delegate(UserPresentation a, UserPresentation b)
						{
							return Utils.CompareObjectFields(a, b, sortField, direction);
						}
					);
			}

			return View(users);
		}

		public ActionResult UserDetails(UserPresentation user)
		{
			return View(user);
		}

		[OutputCache(Duration = 1, VaryByParam = "*", NoStore = true)]
		public ActionResult UpdateUser(string id, string userType, string sortField, SortDirection? sortOrder)
		{
			ViewData["userType"] = userType;
			ViewData["sortField"] = sortField;
			ViewData["sortOrder"] = sortOrder;

			UserPresentation user = MembershipExtensions.GetUserPresentation(id);

			ViewData["email"] = user.Email;
			ViewData["dealerEmployee"] = user.DealerEmployee;
			ViewData["city"] = user.City;
			ViewData["deliveryAddress"] = user.DeliveryAddress;
			ViewData["firstName"] = user.FirstName;
			ViewData["lastName"] = user.LastName;
			ViewData["mobilePhone"] = user.MobilePhone;
			ViewData["phone"] = user.Phone;

			List<DealerPresentation> dealers = Dealer.GetDealerPresentations(SystemSettings.CurrentLanguage);
			List<SelectListItem> dealerItems = (from dealer in dealers
												select new SelectListItem
												{
													Text = dealer.Name,
													Value = dealer.Id.ToString(),
													Selected = dealer.Id == user.DealerId
												}).ToList();
			ViewData["dealerId"] = dealerItems;

			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateUser(
			string userType,
			string sortField,
			SortDirection? sortOrder,
			string email,
			string firstName,
			string lastName,
			string city,
			string deliveryAddress,
			string phone,
			string mobilePhone,
			bool dealerEmployee,
			int dealerId
		)
		{
			ProfileCommon profile = ProfileCommon.Create(email) as ProfileCommon;
			profile.FirstName = firstName;
			profile.LastName = lastName;
			profile.DeliveryAddress = deliveryAddress;
			profile.Phone = phone;
			profile.MobilePhone = mobilePhone;
			profile.City = city;

			if (dealerEmployee)
			{
				profile.DealerEmployee = true;
				profile.DealerId = dealerId;
				if (!Roles.IsUserInRole(email, "Dealers"))
					Roles.AddUserToRole(email, "Dealers");
			}
			else
			{
				if (Roles.IsUserInRole(email, "Dealers"))
					Roles.RemoveUserFromRole(email, "Dealers");
				profile.DealerId = int.MinValue;
				profile.DealerEmployee = false;
			}

			profile.Save();

			return RedirectToAction("Users", new { userType = userType, sortField = sortField, sortOrtder = sortOrder });
		}

		public ActionResult DeleteUser(string id, string userType, string sortField, SortDirection? sortOrder)
		{
			Membership.DeleteUser(id, true);
			return RedirectToAction("Users", new { userType = userType, sortField = sortField, sortOrtder = sortOrder });
		}
		#endregion

		#region ApplicationSettings
		public ActionResult StartText()
		{
			ViewData["ruText"] = ApplicationData.GetStartText("ru-RU");
			ViewData["uaText"] = ApplicationData.GetStartText("uk-UA");
			return View();
		}

		public void UpdateStartText(string ruText, string uaText)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			values["ru-RU"] = HttpUtility.HtmlDecode(ruText);
			values["uk-UA"] = HttpUtility.HtmlDecode(uaText);
			ApplicationData.UpdateStartText(values);
			Response.Write(Helpers.Helpers.CloseParentScript("StartText"));
		}

		public ActionResult AboutUsALittle()
		{
			ViewData["ruText"] = ApplicationData.GetAboutUsALittle("ru-RU");
			ViewData["uaText"] = ApplicationData.GetAboutUsALittle("uk-UA");
			return View();
		}

		public void UpdateAboutUsALittle(string ruText, string uaText)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			values["ru-RU"] = HttpUtility.HtmlDecode(ruText);
			values["uk-UA"] = HttpUtility.HtmlDecode(uaText);
			ApplicationData.UpdateAboutUsALittle(values);
			Response.Write(Helpers.Helpers.CloseParentScript("AboutUsALittle"));
		}

		public ActionResult Agreement()
		{
			ViewData["ruText"] = ApplicationData.GetAgreement("ru-RU");
			ViewData["uaText"] = ApplicationData.GetAgreement("uk-UA");
			return View();
		}

		public void UpdateAgreement(string ruText, string uaText)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			values["ru-RU"] = HttpUtility.HtmlDecode(ruText);
			values["uk-UA"] = HttpUtility.HtmlDecode(uaText);
			ApplicationData.UpdateAgreement(values);
			Response.Write(Helpers.Helpers.CloseParentScript("Agreement"));
		}

		public ActionResult SubCategoryText()
		{
			ViewData["ruText"] = ApplicationData.GetSubCategoryText("ru-RU");
			ViewData["uaText"] = ApplicationData.GetSubCategoryText("uk-UA");
			return View();
		}

		public void UpdateSubCategoryText(string ruText, string uaText)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			values["ru-RU"] = HttpUtility.HtmlDecode(ruText);
			values["uk-UA"] = HttpUtility.HtmlDecode(uaText);
			ApplicationData.UpdateSubCategoryText(values);
			Response.Write(Helpers.Helpers.CloseParentScript("SubCategoryText"));
		}

		public ActionResult ContactsHeader()
		{
			ViewData["ruText"] = ApplicationData.GetContactsHeader("ru-RU");
			ViewData["uaText"] = ApplicationData.GetContactsHeader("uk-UA");
			return View();
		}

		public void UpdateContactsHeader(string ruText, string uaText)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			values["ru-RU"] = HttpUtility.HtmlDecode(ruText);
			values["uk-UA"] = HttpUtility.HtmlDecode(uaText);
			ApplicationData.UpdateContactsHeader(values);
			Response.Write(Helpers.Helpers.CloseParentScript("Contacts"));
		}
		#endregion

		#region News
		public ActionResult News()
		{
			using (NewsStorage context = new NewsStorage())
			{
				List<NewsPresentation> news = (from newsItem in context.News
											   join title in context.Translations on newsItem.Id equals title.ItemId
											   where title.TranslationItemTypeId == (int)ItemTypes.NewsTitle
											   && title.Language == SystemSettings.CurrentLanguage
											   orderby newsItem.Date descending
											   select new NewsPresentation
											   {
												   Id = newsItem.Id,
												   Enabled = newsItem.Enabled,
												   Date = newsItem.Date,
												   Title = title.Translation
											   }).ToList();
				return View(news);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult News(FormCollection form)
		{
			PostData updates = form.ProcessPostData();
			using (NewsStorage context = new NewsStorage())
			{
				var news = (from item in context.News select item).ToList();
				foreach (string key in updates.Keys)
				{
					int itemId = int.Parse(key);
					bool enabled = bool.Parse(updates[key]["enabled"]);
					News item = (from n in news where n.Id == itemId select n).First();
					item.Enabled = enabled;
				}
				context.SaveChanges();
			}
			return RedirectToAction("News");
		}

		public ActionResult DeleteNews(int id)
		{
			using (NewsStorage context = new NewsStorage())
			{
				News item = (from n in context.News where n.Id == id select n).First();
				context.DeleteObject(item);
				context.SaveChanges();
			}
			return RedirectToAction("News");
		}

		public ActionResult AddEditNews(int? id)
		{
			if (id != null)
			{
				using (NewsStorage context = new NewsStorage())
				{
					var item = (from news in context.News
								join ruTitle in context.Translations on news.Id equals ruTitle.ItemId
								join uaTitle in context.Translations on news.Id equals uaTitle.ItemId
								join ruShortText in context.Translations on news.Id equals ruShortText.ItemId
								join uaShortText in context.Translations on news.Id equals uaShortText.ItemId
								join ruLongText in context.Translations on news.Id equals ruLongText.ItemId
								join uaLongText in context.Translations on news.Id equals uaLongText.ItemId
								where ruTitle.TranslationItemTypeId == (int)ItemTypes.NewsTitle && ruTitle.Language == "ru-RU"
								   && uaTitle.TranslationItemTypeId == (int)ItemTypes.NewsTitle && uaTitle.Language == "uk-UA"
								   && ruShortText.TranslationItemTypeId == (int)ItemTypes.NewsDescription && ruShortText.Language == "ru-RU"
								   && uaShortText.TranslationItemTypeId == (int)ItemTypes.NewsDescription && uaShortText.Language == "uk-UA"
								   && ruLongText.TranslationItemTypeId == (int)ItemTypes.NewsText && ruLongText.Language == "ru-RU"
								   && uaLongText.TranslationItemTypeId == (int)ItemTypes.NewsText && uaLongText.Language == "uk-UA"
								   && news.Id == id.Value
								select new
								{
									ruTitle = ruTitle.Translation,
									uaTitle = uaTitle.Translation,
									ruShortText = ruShortText.Translation,
									uaShortText = uaShortText.Translation,
									ruLongText = ruLongText.Translation,
									uaLongText = uaLongText.Translation,
									date = news.Date,
									enabled = news.Enabled
								}
									).First();
					ViewData["ruTitle"] = item.ruTitle;
					ViewData["uaTitle"] = item.uaTitle;
					ViewData["ruShortText"] = item.ruShortText;
					ViewData["uaShortText"] = item.uaShortText;
					ViewData["ruLongText"] = item.ruLongText;
					ViewData["uaLongText"] = item.uaLongText;
					ViewData["date"] = item.date.ToString("dd.MM.yyyy");
					ViewData["enabled"] = item.enabled;
				}
			}
			else
				ViewData["date"] = DateTime.Now.ToString("dd.MM.yyyy");
			ViewData["id"] = id;
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AddEditNews(int? id, string uaTitle, string ruTitle, string date, string uaShortText, string ruShortText, string uaLongText, string ruLongText, bool enabled)
		{
			if (ValidateNewsAdd(uaTitle, ruTitle, uaShortText, ruShortText, uaLongText, ruLongText))
			{
				News news = null;
				using (NewsStorage context = new NewsStorage())
				{
					if (id != null)
					{
						news = context.News.Where(n => n.Id == id.Value).Select(n => n).First();
					}
					else
						news = new News();
					news.Enabled = enabled;
					CultureInfo cultureInfo = CultureInfo.GetCultureInfo("ru-RU");
					news.Date = DateTime.Parse(date, cultureInfo);
					if (id == null)
						context.AddToNews(news);
					context.SaveChanges();
				}

				List<TranslationItem> translations = new List<TranslationItem>{
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsTitle, Language="ru-RU", Translation = ruTitle},
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsTitle, Language="uk-UA", Translation = uaTitle},
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsDescription, Language="ru-RU", Translation = Server.HtmlDecode(ruShortText)},
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsDescription, Language="uk-UA", Translation = Server.HtmlDecode(uaShortText)},
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsText, Language="ru-RU", Translation = Server.HtmlDecode(ruLongText)},
                    new TranslationItem{ ItemId = news.Id, ItemType = ItemTypes.NewsText, Language="uk-UA", Translation = Server.HtmlDecode(uaLongText)}
                };
				using (ZamovStorage context = new ZamovStorage())
					context.UpdateTranslations(Utils.CreateTranslationXml(translations));
				return RedirectToAction("News");
			}
			else
				return View();
		}

		private bool ValidateNewsAdd(string uaTitle, string ruTitle, string uaShortText, string ruShortText, string uaLongText, string ruLongText)
		{
			if (string.IsNullOrEmpty(uaTitle) || string.IsNullOrEmpty(ruTitle))
				ModelState.AddModelError("title", "");
			if (string.IsNullOrEmpty(uaShortText) || string.IsNullOrEmpty(ruShortText))
				ModelState.AddModelError("shortText", "");
			if (string.IsNullOrEmpty(ruLongText) || string.IsNullOrEmpty(uaLongText))
				ModelState.AddModelError("longText", "");
			return ModelState.IsValid;
		}
		#endregion

		#region StartupTools
		public ActionResult RemoveDealers()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				List<DealerPresentation> dealers = (from dealer in context.Dealers
													join name in context.Translations on dealer.Id equals name.ItemId
													where name.TranslationItemTypeId == (int)ItemTypes.DealerName
													&& name.Language == SystemSettings.CurrentLanguage
                                                    orderby dealer.IndexNumber
													select new DealerPresentation
													{
														Name = name.Text,
														Id = dealer.Id,
														Enabled = dealer.Enabled
													}
													).ToList();
				return View(dealers);
			}
		}

		public ActionResult RemoveDealer(int id)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				context.RemoveDealer(id);
			}
			return RedirectToAction("RemoveDealers");
		}

		public ActionResult ClearCache()
		{
			HttpContext.Cache.ClearCache(k => true);
			return View();
		}

		public ActionResult CopyDealer()
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				var dealers = context.Dealers.Include("Cities")
					.Join(context.Translations.Where(tr => tr.Language == "uk-UA").Where(tr => tr.TranslationItemTypeId == 5),
					d => d.Id, tr => tr.ItemId, (d, tr) => new { id = d.Id, name = tr.Text }).ToList();

				ViewData["dealerId"] = dealers.Select(d => new SelectListItem { Text = d.name, Value = d.id.ToString() }).ToList();

				var cities = context.Cities
					.Join(context.Translations.Where(tr => tr.Language == "uk-UA").Where(tr => tr.TranslationItemTypeId == 1),
					c => c.Id, tr => tr.ItemId, (c, tr) => new { name = tr.Text, id = c.Id }).ToList();

				ViewData["cityId"] = cities.Select(c => new SelectListItem { Text = c.name, Value = c.id.ToString() });
			}
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CopyDealer(int dealerId, int cityId)
		{
			using (ZamovStorage context = new ZamovStorage())
			{
				context.CopyDealer(dealerId, cityId);
			}
			return RedirectToAction("CopyDealer");
		}
		#endregion

		#region Orders
		public ActionResult ExpireOrders()
		{
			using (ZamovStorage context = new ZamovStorage())
				context.ExpireOrders();
			return RedirectToAction("Index");
		}
		#endregion

		#region Advert

		public ActionResult Advert()
		{
			using (var context = new ZamovStorage())
			{
				var adverts = context.Advert.Include("Category").Select(a => a).Where(a => a.Category == null).ToList();
				var categories = context.Categories.Include("Advert").Select(c => c).OrderBy(c => c.IndexNumber).ToList();
				ViewData["categories"] = categories;
				return View(adverts);
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InsertAdvert(FormCollection form)
		{

			return RedirectToAction("Advert");
		}

		public ActionResult EditAdvert(int? id, int? categoryId)
		{
			using (var context = new ZamovStorage())
			{
				if (id.HasValue)
				{
					var advert = context.Advert.Select(a => a).Where(a => a.Id == id).First();
					return View(advert);
				}
				else
				{
					var newAdvert = new Advert();
					Category category = context.Categories.Select(c => c).Where(c => c.Id == categoryId).First();
					newAdvert.Category = category;
					newAdvert.IsActive = true;
					newAdvert.Position = (int)BannerPosition.Top;
					context.AddToAdvert(newAdvert);
					context.SaveChanges();

					// TODO: Как получить Id только что сохранённого newAdvert ?
					var advert = context.Advert.Include("Category").Select(a => a).Where(a => a.Category.Id == categoryId).First();
					/////////////////////////////////////////
					return View(advert);
				}
			}
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult EditAdvert(int id, bool isActive, string imageSource)
		{
			using (var context = new ZamovStorage())
			{

				var advert = context.Advert.Select(a => a).Where(a => a.Id == id).First();
				advert.IsActive = isActive;

				string file = Request.Files["banner"].FileName;
				if (!string.IsNullOrEmpty(file))
				{
					string newFileName = IOHelper.GetUniqueFileName("~/Content/Banners", file);
					string filePath = Path.Combine(Server.MapPath("~/Content/Banners"), newFileName);
					Request.Files["banner"].SaveAs(filePath);
					advert.ImageSource = newFileName;
				}
				context.SaveChanges();
			}
			return RedirectToAction("Advert");
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult UpdateAdvertList(FormCollection form)
		{
			if (!string.IsNullOrEmpty(form["enablities"]))
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				Dictionary<string, string> enables = serializer.Deserialize<Dictionary<string, string>>(form["enablities"]);
				using (ZamovStorage context = new ZamovStorage())
				{
					foreach (string key in enables.Keys)
					{
						int id = int.Parse(key);
						var advert = context.Advert.Select(a => a).Where(a => a.Id == id).First();
						advert.IsActive = bool.Parse(enables[key]);
					}
					context.SaveChanges();
				}
			}

			return RedirectToAction("Advert");
		}

		#endregion

		#region Autocomplete

		public ActionResult FindBrands(string q)
		{
			return Content(string.Join("\n", Find(q, "sp_BrandsSearchByNamePart")));
		}

		private static string[] Find(string q, string nameProcedure)
		{
			using (var context = new ZamovStorage())
			{
				var brands = new List<string>();
				object param = new SqlParameter("prefix", q);
				DbCommand command = context.CreateStoreCommand(nameProcedure, CommandType.StoredProcedure, param);

				using (context.Connection.CreateConnectionScope())
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							brands.Add(reader.GetString(0));
						}
					}
				}
				return brands.ToArray();
			}
		}

		public ActionResult FindGroups(string q)
		{
			return Content(string.Join("\n", Find(q, "sp_GroupsSearchByNamePart")));
		}

		public ActionResult FindProducts(string q)
		{
			return Content(string.Join("\n", Find(q, "sp_ProductsSearchByNamePart")));
		}

		#endregion

        #region SEO

        public ActionResult SeoSettings(int id, int seoEntityType)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                if (seoEntityType == (int)SeoEntityType.Dealers)
                {
                    Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
                    ViewData["dealer"] = dealer;
                }
                if (seoEntityType == (int)SeoEntityType.Categories)
                {
				    Category category = context.Categories.Include("Images").Where(c => c.Id == id).FirstOrDefault();
                    ViewData["category"] = category;
                }
                if (seoEntityType == (int)SeoEntityType.Groups)
                {
                    GroupsAdditional group = context.GroupsAdditional.Where(g => g.grID == id).FirstOrDefault();
                    ViewData["group"] = group;
                }
            }
            ViewData["seoEntityType"] = seoEntityType;
            using (SeoStorage seoContext = new SeoStorage())
            {
                var seoDictMetaTags = new Dictionary<string, SeoMetaTags>();

                List<SeoAdditional> listSeoAdditional = seoContext.SeoAdditional.
                    Where(sa => sa.TypeEntity == seoEntityType && sa.IdEntity == id).ToList();

                foreach (SeoAdditional seo in listSeoAdditional)
                {
                    SeoMetaTags seoMetaTags = new SeoMetaTags
                                                  {
                                                      Description = seo.Description,
                                                      Keywords = seo.Keywords,
                                                      MetaTags = seo.MetaTags,
                                                      Title = seo.Title
                                                  };
                    seoDictMetaTags.Add(seo.Language, seoMetaTags);
                }
                return View(seoDictMetaTags);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeoSettings(FormCollection form)
        {
            int id = int.Parse(form["id"]);
            int seoEntityType = int.Parse(form["seoEntityType"]);
            PostData postData = form.ProcessPostData("id", "seoEntityType");

            using (SeoStorage context = new SeoStorage())
            {
                foreach (var update in postData)
                {
                    string lang = update.Key;
                    SeoAdditional seoAdditional = context.SeoAdditional.
                        Where(sa => sa.IdEntity == id && sa.TypeEntity == seoEntityType && sa.Language == lang).FirstOrDefault();
                    if (seoAdditional == null)
                    {
                        seoAdditional = new SeoAdditional
                                            {
                                                Language = lang,
                                                IdEntity = id,
                                                TypeEntity = seoEntityType,
                                                Title = update.Value["Title"],
                                                Keywords = update.Value["Keywords"],
                                                Description = update.Value["Description"],
                                                MetaTags = update.Value["MetaTags"]
                                            };
                        context.AddToSeoAdditional(seoAdditional);
                    }
                    else
                    {
                        seoAdditional.Title = update.Value["Title"];
                        seoAdditional.Keywords = update.Value["Keywords"];
                        seoAdditional.Description = update.Value["Description"];
                        seoAdditional.MetaTags = update.Value["MetaTags"];
                    }
                    context.SaveChanges();
                }
            }
            return RedirectToAction("SeoSettings", new { id = id, seoEntityType = seoEntityType });
        }

        #endregion

	}
}