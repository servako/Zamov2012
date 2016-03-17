using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Web.Security;
using System.Data.Objects.DataClasses;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Zamov.Helpers;
using Zamov.srSms;

namespace Zamov.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/

        public ActionResult Index(string redirectUrl)
        {
            ViewData["redirectUrl"] = redirectUrl;
            if (SystemSettings.Cart.Orders.Count == 0)
                return RedirectToAction("Index", "Home");
            using (ZamovStorage context = new ZamovStorage())
            {
                List<CategoryPresentation> categories = context.GetCachedCategories(SystemSettings.CityId, SystemSettings.CurrentLanguage);
                ViewData["categories"] = categories;
            }
            return View(SystemSettings.Cart.Orders);
        }

        public ActionResult EmptyCart(string redirectUrl)
        {
            SystemSettings.EmptyCart();
            if (string.IsNullOrEmpty(redirectUrl))
                return RedirectToAction("Index", "Home");
            string decodedUrl = HttpUtility.UrlDecode(redirectUrl);
            return Redirect(decodedUrl);
        }

        public ActionResult RemoveOrderItem(int id, string redirectUrl)
        {
            Cart cart = SystemSettings.Cart;
            foreach (var order in cart.Orders)
            {
                if (order.OrderItems != null && order.OrderItems.Count > 0)
                {
                    for (int i = order.OrderItems.Count - 1; i >= 0; i--)
                    {
                        OrderItem orderItem = order.OrderItems.ElementAt(i);
                        if (orderItem.HashCode == id)
                            order.OrderItems.Remove(orderItem);
                    }
                }
            }

            if (cart.Orders != null)
                for (int i = cart.Orders.Count - 1; i >= 0; i--)
                {
                    Order order = cart.Orders.ElementAt(i);
                    if (order.OrderItems.Count == 0)
                        cart.Orders.Remove(order);
                }

            int orderItemsCount = cart.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity));
            if (orderItemsCount == 0)
            {
                SystemSettings.EmptyCart();
                if (string.IsNullOrEmpty(redirectUrl))
                    return RedirectToAction("Index", "Home");
                return Redirect(redirectUrl);
            }
            return RedirectToAction("Index", new { redirectUrl = redirectUrl });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Recalculate(string updates, string redirectUrl)
        {
            if (!string.IsNullOrEmpty(updates))
            {
                Cart cart = SystemSettings.Cart;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, Dictionary<string, string>> orderItems =
                    serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(updates);
                var orderItemList =
                    (from oi in orderItems
                     select new { Id = int.Parse(oi.Key), Quantity = int.Parse(oi.Value["quantity"]) })
                     .ToList();
                foreach (var order in cart.Orders)
                {
                    if (order.OrderItems != null && order.OrderItems.Count > 0)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            foreach (var item in orderItemList)
                            {
                                if (orderItem.HashCode == item.Id)
                                    orderItem.Quantity = item.Quantity;
                            }
                        }

                    }
                }
            }
            return RedirectToAction("Index", new { redirectUrl = redirectUrl });
        }


        public ActionResult MakeOrder()
        {
            ViewData["deliveryDate"] = DateTime.Now.ToString("dd.MM.yyyy");
            if (SystemSettings.Cart.Orders.Count == 0)
                return RedirectToAction("Expired");


            HttpCookie cookie = new HttpCookie("makeOrder", "true");
            cookie.Path = "/";
            Response.AppendCookie(cookie);

            using (ZamovStorage context = new ZamovStorage())
            {
                string city = (from c in context.Cities
                               join tran in context.Translations on c.Id equals tran.ItemId
                               where tran.TranslationItemTypeId == (int)ItemTypes.City
                                   && tran.Language == SystemSettings.CurrentLanguage
                                   && c.Id == SystemSettings.CityId
                               select tran.Text).First();
                ViewData["city"] = city;
            }
            if (Request.IsAuthenticated)
            {
                MembershipUser user = Membership.GetUser();
                ProfileCommon profile = ProfileCommon.Create(user.UserName) as ProfileCommon;
                ViewData["firstName"] = profile.FirstName;
                ViewData["lastName"] = profile.LastName;
                ViewData["deliveryAddress"] = profile.DeliveryAddress;
                ViewData["contactPhone"] = profile.MobilePhone;
                ViewData["phone"] = profile.Phone;
                ViewData["email"] = user.UserName;
            }

            return View(SystemSettings.Cart.Orders);
        }

        [BreadCrumb(ResourceName = "PageExpired", Url = "")]
        public ActionResult Expired()
        {
            SystemSettings.EmptyCart();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CaptchaValidation("captcha")]
        public ActionResult MakeOrder(
            string firstName,
            string lastName,
            string city,
            string deliveryAddress,
            string contactPhone,
            string email,
            string comments,
            string deliveryDate,
            string deliveryTime,
            string orderSettings,
            bool agreed,
            bool captchaValid,
            FormCollection form
            )
        {
            foreach (string key in form)
            {
                if (key.StartsWith("paymentType") || key.StartsWith("voucherCode"))
                    ViewData[key] = form[key];
            }
            if (!ValidateMakeOrder(firstName, city, deliveryAddress, contactPhone, deliveryDate, deliveryTime, email, agreed, captchaValid))
                return View(SystemSettings.Cart.Orders);
            Cart cart = SystemSettings.Cart;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, Dictionary<string, string>> orderSettingsDictionary = form.ProcessPostData("firstName", "x", "y", "email", "comments", "lastName", "city", "deliveryAddress", "contactPhone", "deliveryDate", "deliveryTime", "agreedPresentation", "deliveryTimeClientState", "deliveryDateTime", "agreed", "captcha", "captchaValid", "captcha-guid");

            var systemSettingsList = (from os in orderSettingsDictionary
                                      select new
                                      {
                                          Id = int.Parse(os.Key),
                                          VoucherCode = (os.Value.ContainsKey("voucherCode")) ? os.Value["voucherCode"] : null,
                                          PaymentType = GetPaymentType(os.Value)
                                      }).ToList();
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo("ru-RU");
            DateTime date = DateTime.Parse(deliveryDate + " " + deliveryTime, cultureInfo);
            MembershipUser user = Membership.GetUser(true);
            Guid? userId = null;
            SystemSettings.MemberProperties.DeliveryAddress = deliveryAddress;
            SystemSettings.MemberProperties.Email = email;
            SystemSettings.MemberProperties.FirstName = firstName;
            SystemSettings.MemberProperties.LastName = lastName;
            SystemSettings.MemberProperties.MobilePhone = contactPhone;
            SystemSettings.MemberProperties.Phone = contactPhone;
            SystemSettings.MemberProperties.City = city;
            if (user != null)
                userId = (Guid)user.ProviderUserKey;
            using (OrderStorage context = new OrderStorage())
            {
                foreach (var order in cart.Orders)
                {
                    order.Address = deliveryAddress;
                    order.ClientName = firstName + " " + lastName;
                    order.Date = DateTime.Now;
                    order.DeliveryDate = date;
                    order.Phone = contactPhone;
                    order.Email = email;
                    order.UserId = userId;
                    order.PaymentType = (int)PaymentTypes.Encash;
                    order.City = city;
                    order.Comments = comments;
                    foreach (var option in systemSettingsList)
                    {
                        if (option.Id == order.GetHashCode())
                        {
                            if (option.PaymentType != null)
                                order.PaymentType = option.PaymentType.Value; ;
                            order.DiscountCardNumber = option.VoucherCode;
                        }
                    }
                    order.DiscountCardNumber = form["voucherCode_" + order.HashCode];
                    order.PaymentType = int.Parse(form["paymentType_" + order.HashCode]);
                }
                cart.Date = DateTime.Now;
                context.AddToCarts(cart);
                context.SaveChanges();
                context.Detach(cart);
                MailOrders(cart);
            }
            return RedirectToAction("ThankYou");
        }

        private void MailOrders(Cart cart)
        {
            using (OrderStorage context = new OrderStorage())
            {
                context.Attach(cart);
                if (!cart.Orders.IsLoaded)
                    cart.Orders.Load();
                List<MailAddress> addresses = new List<MailAddress>();
                var dicSendSms = new Dictionary<string, List<string>>();

                foreach (Order order in cart.Orders)
                {
                    int dealerId = (int)order.DealerReference.EntityKey.EntityKeyValues[0].Value;
                    DealerPresentation dealer = Dealer.GetPresentation(dealerId);

                    using (var zamovStorage = new ZamovStorage())
                    {
                        List<DealerContacts> dealerContacts =
                            (from dc in zamovStorage.DealerContacts.Include("Dealers")
                             where dc.Dealers.Id == dealerId && dc.ContactType == (int)ContactType.PhoneMobile 
                             && dc.Enabled && dc.Value.Length > 0
                             select dc).ToList();
                        if (dealerContacts.Count > 0)
                        {
                            foreach (DealerContacts contact in dealerContacts)
                            {
                                if (!dicSendSms.ContainsKey(contact.Value))
                                {
                                    dicSendSms.Add(contact.Value, new List<string> { dealer.Name });
                                }
                                else if (!dicSendSms[contact.Value].Contains(dealer.Name))
                                {
                                    dicSendSms[contact.Value].Add(dealer.Name);
                                }
                            }
                        }
                    }
                                        
                    if (NeedsToBeMailed(order))
                    {
                        string mailAddress = MembershipExtensions.GetDealerEmail((int)order.DealerReference.EntityKey.EntityKeyValues[0].Value);
                        if (!string.IsNullOrEmpty(mailAddress))
                            addresses.Add(new MailAddress(mailAddress));
                    }
                }
                if (addresses.Count > 0)
                    MailHelper.SendTemplate(ApplicationData.FeedbackEmail, addresses, "newOrder", false);

                if (dicSendSms.Count > 0)
                {
                    foreach (var pair in dicSendSms)
                    {
                        var contacts = new srSms.contact[1];
                        contacts[0] = new contact {phone = pair.Key};
                        string dealersName = string.Join(",", pair.Value.ToArray());
                        SmsHelper.SendTemplate("Zamov", contacts, "newOrder", String.Empty, TransliteHelper.TransliterateString(dealersName));
                    }
                }
            }
        }

        private bool NeedsToBeMailed(Order order)
        {
            return true;
            /*
            if (HttpContext.Items["onlineDealers"] == null)
                HttpContext.Items["onlineDealers"] = MembershipExtensions.GetOnlineDealers();
            int[] onlineDealers = (int[])HttpContext.Items["onlineDealers"];
            if (!onlineDealers.Contains((int)order.DealerReference.EntityKey.EntityKeyValues[0].Value))
                return true;
            return false;
            */
        }

        private bool ValidateMakeOrder(
            string firstName,
            string city,
            string deliveryAddress,
            string contactPhone,
            string deliveryDate,
            string deliveryTime,
            string email,
            bool agreed,
            bool captchaValid
            )
        {
            if (string.IsNullOrEmpty(firstName.Replace(" ", "")))
                ModelState.AddModelError("firstName", ResourcesHelper.GetResourceString("FirstNameRequired"));
            if (string.IsNullOrEmpty(city.Replace(" ", "")))
                ModelState.AddModelError("city", ResourcesHelper.GetResourceString("CityRequired"));
            if (string.IsNullOrEmpty(deliveryAddress.Replace(" ", "")))
                ModelState.AddModelError("deliveryAddress", ResourcesHelper.GetResourceString("DeliveryAddressRequired"));
            if (string.IsNullOrEmpty(contactPhone))
                ModelState.AddModelError("contactPhone", ResourcesHelper.GetResourceString("PhoneRequired"));
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo("ru-RU");
            DateTime date;
            if (!DateTime.TryParse(deliveryDate, cultureInfo, DateTimeStyles.None, out date))
                ModelState.AddModelError("deliveryDate", ResourcesHelper.GetResourceString("DateTimeInvalidOrEmpty"));
            Regex rtime = new Regex(@"[0-2][0-9]\:[0-6][0-9]");
            if (!rtime.IsMatch(deliveryTime))
                ModelState.AddModelError("deliveryTime", ResourcesHelper.GetResourceString("TimeInvalidOrEmpty"));
            if (!agreed)
                ModelState.AddModelError("agreed", ResourcesHelper.GetResourceString("YouMustAgree"));
            if (!captchaValid)
                ModelState.AddModelError("captchaInvalid", ResourcesHelper.GetResourceString("IncorrectCaptcha"));
            Regex emailCheck = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!string.IsNullOrEmpty(email) && !emailCheck.IsMatch(email))
                ModelState.AddModelError("email", ResourcesHelper.GetResourceString("EmailIncorrect"));
            Regex phoneCheck = new Regex(@"^(\+?\d+(-|\s))?(\(?\d+\)?(-|\s)?)?\d+(-|\s)?\d+(-|\s)?\d+$");
            if (!string.IsNullOrEmpty(contactPhone) && !phoneCheck.IsMatch(contactPhone))
                ModelState.AddModelError("contactPhone", ResourcesHelper.GetResourceString("PhoneIncorrect"));
            return ModelState.IsValid;
        }

        public ActionResult ThankYou()
        {
            if (Request.IsAuthenticated)
                SystemSettings.EmptyCart();
            return View();
        }

        private int? GetPaymentType(Dictionary<string, string> item)
        {
            int? result = null;
            if (item.ContainsKey("paymentType") && item["paymentType"] != null)
                result = int.Parse(item["paymentType"]);
            return result;
        }
    }
}