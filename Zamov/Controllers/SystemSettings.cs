using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Web.Security;
using Zamov.Models;

namespace Zamov.Controllers
{
    public static class SystemSettings
    {
        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public static string CurrentDomain
        {
            get 
            { 
                string result = "Com";
                string host = HttpContext.Current.Request.Url.Host;
                if(host.EndsWith(".net"))
                    result = "Net";
                return result;
            }
        }

        public static int UsersPageSize
        {
            get
            {
                int result = 0;
                string pageSizeString = WebConfigurationManager.AppSettings["UsersPageSize"];
                if (!string.IsNullOrEmpty(pageSizeString))
                {
                    result = int.Parse(pageSizeString);
                }
                return result;
            }
        }

        public static string CurrentLanguage
        {
            get
            {
                if (Session["lang"] == null)
                    Session["lang"] = "ru-RU";
                return (string)Session["lang"];
            }
            set { System.Web.HttpContext.Current.Session["lang"] = value; }
        }

        public static DateTime LastTime
        {
            get
            {
                if (Session["lasttime"] == null)
                    Session["lasttime"] = DateTime.Now;
                return (DateTime)Session["lasttime"];
            }
            set { Session["lasttime"] = value; }
        }

        public static string SearchContext
        {
            get
            {
                return (string)Session["searchContext"];
            }
            set
            {
                Session["searchContext"] = value;
            }
        }

        public static int? CurrentDealer
        {
            get
            {
                int? result = null;
                if (Session["CurrentDealer"] != null)
                    result = Convert.ToInt32(Session["CurrentDealer"]);
                return result;
            }
            set { Session["CurrentDealer"] = value; }
        }

        public static int? SelectedDealer
        {
            get
            {
                int? result = null;
                if (Session["SelectedDealer"] != null)
                    result = Convert.ToInt32(Session["SelectedDealer"]);
                return result;
            }
            set { Session["SelectedDealer"] = value; }
        }
        
        public static Guid? CurrentUserId
        {
            get
            {
                MembershipUser user = Membership.GetUser();
                if (user != null)
                    return (Guid) user.ProviderUserKey;
                return null;
            }
        }
        
        public static int CityId
        {
            get 
            {
                int result = int.MinValue;
                if (Session["CityId"] != null)
                    result = Convert.ToInt32(Session["CityId"]);
                return result;
            }
            set { Session["CityId"] = value; }
        }

        public static Cart Cart
        {
            get 
            {
                if (Session["Cart"] == null)
                    Session["Cart"] = new Cart();
                return (Cart)Session["Cart"];
            }
        }

        public static MemberProperties MemberProperties
        {
            get 
            {
                if (Session["MemberProperties"] == null)
                    Session["MemberProperties"] = new MemberProperties();
                return (MemberProperties)Session["MemberProperties"];
            }
            
        }

        public static void EmptyCart()
        {
            Session["Cart"] = null;
        }


        public static void InitializeCity(int defaultValue)
        {
            if (SystemSettings.CityId < 1)
            {
                int? cityId = null;
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    string cityName = ProfileCommon.Create(HttpContext.Current.User.Identity.Name).City;
                    if (!string.IsNullOrEmpty(cityName))
                    {
                        using (ZamovStorage context = new ZamovStorage())
                        {
                            cityId = (from city in context.Cities
                                      join ruName in context.Translations on city.Id equals ruName.ItemId
                                      join uaName in context.Translations on city.Id equals uaName.ItemId
                                      where ruName.Language == "ru-RU" && uaName.Language == "uk-UA"
                                      && ruName.TranslationItemTypeId == (int)ItemTypes.City && uaName.TranslationItemTypeId == (int)ItemTypes.City
                                      && (ruName.Text == cityName || uaName.Text == cityName)
                                      select city.Id).SingleOrDefault();
                        }
                    }
                }
                if (cityId == null || cityId == 0)
                    cityId = defaultValue;

                SystemSettings.CityId = cityId.Value;
            }
        }
    }
}
