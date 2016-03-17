using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.Data.Extensions;
using Zamov.Controllers;
using Zamov.Models;

namespace Zamov.Helpers
{
    public static class OrderHelper
    {
        public static void AddToCart(List<OrderItemCart> orderItemCart, ProductVersion productVersion)
        {
            Cart cart = SystemSettings.Cart;

            if (orderItemCart.Count > 0)
            {
                if (productVersion == ProductVersion.First)
                {
                    Dictionary<int, Product> products = null;
                    using (ZamovStorage context = new ZamovStorage())
                    {
                        string productIds = string.Join(",", orderItemCart.Select(oil => oil.ProductId.ToString()).ToArray());
                        ObjectQuery<Product> productsQuery = new ObjectQuery<Product>(
                            "SELECT VALUE P FROM Products AS P WHERE P.Id IN {" + productIds + "}",
                            context);
                        products = productsQuery.ToDictionary(pr => pr.Id);
                    }
                    if (products != null && products.Count > 0)
                    {
                        foreach (var orderItem in orderItemCart)
                        {
                            Order order = (from o in cart.Orders
                                           where
                                               o.DealerReference.EntityKey != null &&
                                               (int) o.DealerReference.EntityKey.EntityKeyValues[0].Value ==
                                               orderItem.DealerId
                                           select o).SingleOrDefault();
                            if (order == null)
                            {
                                order = new Order();
                                order.HashCode = order.GetHashCode();
                                IEnumerable<KeyValuePair<string, object>> dealerKeyValues =
                                    new KeyValuePair<string, object>[]
                                        {new KeyValuePair<string, object>("Id", orderItem.DealerId)};
                                EntityKey dealer = new EntityKey("OrderStorage.OrderDealers", dealerKeyValues);
                                order.DealerReference.EntityKey = dealer;
                                cart.Orders.Add(order);
                            }
                            Product product = products[orderItem.ProductId];
                            OrderItem item = null;
                            if (order.OrderItems != null && order.OrderItems.Count > 0)
                                item =
                                    (from i in order.OrderItems where i.PartNumber == product.PartNumber select i).
                                        SingleOrDefault();
                            if (item == null)
                            {
                                item = new OrderItem();
                                item.HashCode = item.GetHashCode();
                            }
                            item.PartNumber = product.PartNumber;
                            item.Name = product.Name;
                            item.Price = product.Price;
                            item.ProductId = product.Id;
                            item.Quantity = orderItem.Quantity;
                            item.Unit = product.Unit;
                            order.OrderItems.Add(item);
                        }
                    }
                }
                else if (productVersion == ProductVersion.Additional)
                {
                    var orderItemGroup = orderItemCart.GroupBy(o => o.DealerId, o => o);

                    foreach (var orderGroup in orderItemGroup)
                    {
                        Order order = (from o in cart.Orders
                                       where o.DealerReference.EntityKey != null
                                             &&
                                             (int)o.DealerReference.EntityKey.EntityKeyValues[0].Value ==
                                             orderGroup.Key
                                       select o).SingleOrDefault();

                        if (order == null)
                        {
                            order = new Order();
                            order.HashCode = order.GetHashCode();
                            IEnumerable<KeyValuePair<string, object>> dealerKeyValues =
                                new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Id", orderGroup.Key) };
                            EntityKey dealer = new EntityKey("OrderStorage.OrderDealers", dealerKeyValues);
                            order.DealerReference.EntityKey = dealer;
                        }

                        List<OrderItemCart> orderItemListLocals = orderGroup.ToList();

                        var products = new Dictionary<int, ProductPricePresentation>();
                        string productIds = string.Join(",", orderItemListLocals.Select(oil => oil.ProductId.ToString()).ToArray());
                        if (!string.IsNullOrEmpty(productIds))
                        {
                            using (ZamovStorage context = new ZamovStorage())
                            {
                                var prod = new List<ProductPricePresentation>();
                                var parameters = new object[]
                                                     {
                                                         new SqlParameter("@dealerProdId", productIds),
                                                         new SqlParameter("@cityId", SystemSettings.CityId)
                                                     };

                                DbCommand command = context.CreateStoreCommand("sp_ProductPrices_Select", CommandType.StoredProcedure, parameters);

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
                                                PartNum = Convert.ToString(reader["p_partNum"]),
                                                Name = Convert.ToString(reader["dp_prodName"]),
                                                PriceHrn = Convert.ToDecimal(reader["pd_price_hrn"]),
                                                Unit = Convert.ToString(reader["p_unit"]),
                                                Price = Convert.ToDecimal(reader["pd_price"])
                                            };
                                            prod.Add(d);
                                        }
                                    }
                                }

                                foreach (var p in prod)
                                {
                                    products.Add(p.IdDealerProd, p);
                                }

                            }
                        }
                        if (products != null && products.Count > 0)
                        {
                            foreach (var orderItem in orderItemListLocals)
                            {
                                bool hasItem = false;
                                ProductPricePresentation product = products[orderItem.ProductId];
                                OrderItem item = null;
                                if (order.OrderItems != null && order.OrderItems.Count > 0)
                                    item =
                                        (from i in order.OrderItems
                                         where i.Additional && i.ProductId == product.IdDealerProd
                                         select i).SingleOrDefault();
                                if (item == null)
                                {
                                    item = new OrderItem();
                                    item.HashCode = item.GetHashCode();
                                    item.Additional = true;
                                }
                                else
                                    hasItem = true;
                                if (!hasItem)
                                {
                                    item.PartNumber = product.PartNum;
                                    item.Name = product.Name;
                                    item.Price = product.PriceHrn;
                                    item.ProductId = product.IdDealerProd;
                                    item.Unit = product.Unit;
                                    item.Quantity = orderItem.Quantity;
                                    order.OrderItems.Add(item);
                                }
                                else
                                    item.Quantity += orderItem.Quantity;
                            }
                            cart.Orders.Add(order);
                        }
                    }
                }
            }
        }
    }
}