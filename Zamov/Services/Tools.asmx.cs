using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Zamov.Models;
using Zamov.Controllers;
using Zamov.Helpers;
using System.Web.Security;

namespace Zamov.Services
{
    /// <summary>
    /// Summary description for Tools
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Tools : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public object GetCityCategories(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                SystemSettings.CityId = id;
                List<CategoryPresentation> categories = context.GetCachedCategories(id, SystemSettings.CurrentLanguage);

				List<SelectListItem> result = categories
					.Select(c => new SelectListItem
					{
						Text = c.Name,
						Value = c.Additional ? string.Format("/Products/ShowCategory/{0}/{1}", id, c.Id) : "/Dealers/" + c.Id.ToString(),
						Selected = c.Id == id || c.Children.Where(ch => ch.Id == id).Count() > 0
					})
					.ToList();
				//result.Insert(0, new SelectListItem { Selected = true, Text = "--" + ResourcesHelper.GetResourceString("SelectCategory") + "--", Value = "" });
				//var result = (from category in categories select new { Text = category.Name, Value = category.Id });
                return result;
            }
        }
        
        
        [WebMethod(EnableSession = true)]
        public object GetNewOrders()
        {
            using (OrderStorage context = new OrderStorage())
            {
                DateTime lasttime = SystemSettings.LastTime;
                MembershipUser user = Membership.GetUser(true);

                ProfileCommon profile = ProfileCommon.Create(user.UserName);
                int dealerId = profile.DealerId;
                List<Order> orders = new List<Order>();
                SystemSettings.LastTime = DateTime.Now;
                int ordersCount = context.Orders.Where(o => o.Dealer.Id == dealerId && o.Status == (int)Statuses.New).Count();
                if (!Roles.IsUserInRole(user.UserName, "Managers"))
                {
                    orders = (
                                             from order in
                                                 context.Orders.Include("Dealer").Include("OrderItems")
                                             where order.Dealer.Id == dealerId && order.Date > lasttime
                                             select order).ToList();
                }
                else
                {
                    orders = (
                                             from order in
                                                 context.Orders.Include("Dealer").Include("OrderItems")
                                             where order.Date > lasttime
                                             select order).ToList();                
                }
                var newOrders = (from order in orders
                              select
                                  new
                                      {
                                          Id = order.Id,
                                          DeliveryDate = String.Format("{0:g}", order.DeliveryDate),
                                          OrderDate = String.Format("{0:g}", order.Date),
                                          ClientName = order.ClientName,
                                          Address = order.Address,
                                          Comments = order.Comments,
                                          DiscountCardNumber = order.DiscountCardNumber,
                                          TotalPrice = ((decimal)order.OrderItems.Sum(oi => oi.Quantity * oi.Price)).ToString("N"),
                                          Status = Controllers.ResourcesHelper.GetResourceString("Status" + (Statuses)order.Status),
                                          StatusName = (Statuses)order.Status
                                      });
                var result = new
                {
                    NewOrdersCount = ordersCount,
                    NewOrders = newOrders
                };
                //needed to update LastActivityTime of user
                return result;
            }
        }
        


       


    }
}
