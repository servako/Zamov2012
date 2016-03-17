using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Microsoft.Data.Extensions;
using Zamov.Models;
using System.Data;
using Zamov.Helpers;
using System.Collections;
using System.Web.UI.WebControls;

namespace Zamov.Controllers
{
    public enum SortFieldNames
    {
        Name,
        Price,
        Dealer
    }
    /*
    public enum SortDirection
    {
        Ascending,
        Descending
    }
    */
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(SystemSettings.SearchContext))
                SearchProduct(SystemSettings.SearchContext, null, null);
            return View();
        }

        [BreadCrumb(ResourceName = "Search", Url = "/Search")]
        public ActionResult SearchProduct(string searchContext, SortFieldNames? sortField, SortDirection? sortOrder)
        {

            ViewData["sortDirection"] = sortOrder;
            ViewData["sortField"] = (sortField != null) ? sortField.ToString() : null;
            ViewData["searchContext"] = HttpUtility.UrlPathEncode(searchContext);
            if (string.IsNullOrEmpty(searchContext))
                searchContext = SystemSettings.SearchContext;
            if (!string.IsNullOrEmpty(searchContext))
            {
                SystemSettings.SearchContext = searchContext;
                using (ZamovStorage context = new ZamovStorage())
                {
                    ObjectQuery<Product> productsQuery = new ObjectQuery<Product>(
                        "SELECT VALUE P FROM Products AS P WHERE P.Name LIKE '%" + searchContext + "%'", context);
                    List<ProductSearchPresentation> products = (from product in productsQuery
                                                                join dealerName in context.Translations on product.Dealer.Id equals dealerName.ItemId
                                                                where dealerName.TranslationItemTypeId == (int)ItemTypes.DealerName
                                                                && dealerName.Language == SystemSettings.CurrentLanguage
                                                                && product.Group.Enabled && !product.Group.Deleted
                                                                && product.Dealer.Enabled
                                                                let description =
                                                                    (from d in context.Translations
                                                                     where d.Language == SystemSettings.CurrentLanguage
                                                                         && d.TranslationItemTypeId == (int)ItemTypes.ProductDescription
                                                                         && d.ItemId == product.Id
                                                                     select d.Text).FirstOrDefault()
                                                                select new ProductSearchPresentation
                                                                {
                                                                    DealerNameId = product.Dealer.Name,
                                                                    DealerId = product.Dealer.Id,
                                                                    DealerName = dealerName.Text,
                                                                    Name = product.Name,
                                                                    Price = product.Price,
                                                                    Id = product.Id,
                                                                    Unit = product.Unit,
                                                                    Description = description,
                                                                    Version = (int)ProductVersion.First
                                                                }
                                                                    ).ToList();

                    var productsAddit = new List<ProductSearchPresentation>();
                    var parameters = new object[]
                                         {
                                             new SqlParameter("@prodNameLike", searchContext),
                                             new SqlParameter("@cityId", SystemSettings.CityId)
                                         };

                    DbCommand command = context.CreateStoreCommand("sp_ProductPrices_Select", CommandType.StoredProcedure, parameters);

                    using (context.Connection.CreateConnectionScope())
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var d = new ProductSearchPresentation
                                            {
                                                DealerNameId = Convert.ToString(reader["dealerName"]),
                                                DealerId = Convert.ToInt32(reader["dp_dealerID"]),
                                                DealerName = Convert.ToString(reader["dealerName"]),
                                                Name = Convert.ToString(reader["dp_prodName"]),
                                                Price = Convert.ToDecimal(reader["pd_price_hrn"]),
                                                Id = Convert.ToInt32(reader["dpID"]),
                                                Unit = Convert.ToString(reader["dp_unit"]),
                                                Description = Convert.ToString(reader["dp_descr"]),
                                                Version = (int)ProductVersion.Additional,
                                                Url = Convert.ToString(reader["dp_photoUrl"])
                                            };
                                productsAddit.Add(d);
                            }
                        }
                    }

                    productsAddit = (from product in productsAddit
                                     join dealerName in context.Translations on product.DealerId equals dealerName.ItemId
                                     where dealerName.TranslationItemTypeId == (int)ItemTypes.DealerName
                                     && dealerName.Language == SystemSettings.CurrentLanguage
                                     select new ProductSearchPresentation
                                     {
                                         DealerNameId = product.DealerNameId,
                                         DealerId = product.DealerId,
                                         DealerName = dealerName.Text,
                                         Name = product.Name,
                                         Price = product.Price,
                                         Id = product.Id,
                                         Unit = product.Unit,
                                         Description = product.Description,
                                         Version = product.Version,
                                         Url = product.Url
                                     }
                            ).ToList();

                    products.AddRange(productsAddit);

                    if (sortField != null && sortOrder != null)
                        switch (sortField)
                        {
                            case SortFieldNames.Name:
                                if (sortOrder == SortDirection.Ascending)
                                    products.Sort(new SortByProductNameAsc());
                                else
                                    products.Sort(new SortByProductNameDesc());
                                break;
                            case SortFieldNames.Price:
                                if (sortOrder == SortDirection.Ascending)
                                    products.Sort(new SortByPriceAsc());
                                else
                                    products.Sort(new SortByPriceDesc());
                                break;
                            case SortFieldNames.Dealer:
                                if (sortOrder == SortDirection.Ascending)
                                    products.Sort(new SortByDealerNameAsc());
                                else
                                    products.Sort(new SortByDealerNameDesc());
                                break;
                        }

                    return View(products);
                }
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddToCart(FormCollection items)
        {
            Cart cart = SystemSettings.Cart;
            PostData orderItems = items.ProcessPostData("X-Requested-With");
            if (orderItems.Count > 0)
            {
                // для ProductVersion.First
                var orderItemListPrVerFirst =
                   (from oi in orderItems
                    where oi.Value["order"].ToLowerInvariant().Contains("true") && int.Parse(oi.Value["verProduct"]) == (int)ProductVersion.First
                    select new OrderItemCart
                    {
                        ProductId = int.Parse(oi.Key),
                        DealerId = int.Parse(oi.Value["dealer"]),
                        Quantity = int.Parse(oi.Value["quantity"])
                    })
                    .ToList();

                OrderHelper.AddToCart(orderItemListPrVerFirst, ProductVersion.First);

                // для ProductVersion.Additional
                var orderItemListPrVerAdditional = 
                    (from oi in orderItems
                     where oi.Value["order"].ToLowerInvariant().Contains("true") && int.Parse(oi.Value["verProduct"]) == (int)ProductVersion.Additional
                    select new OrderItemCart
                    {
                        ProductId = int.Parse(oi.Key),
                        Quantity = int.Parse(oi.Value["quantity"].Replace("_", string.Empty)),
                        DealerId = int.Parse(oi.Value["dealer"].Replace("_", string.Empty))
                    })
                    .ToList();

                OrderHelper.AddToCart(orderItemListPrVerAdditional, ProductVersion.Additional);
            }
            int totalCartItems = cart.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity));
            decimal totalCartPrice = cart.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Price));
            return Json(new { TotalCartPrice = totalCartPrice, TotalCartItems = totalCartItems });
        }


    }
}
