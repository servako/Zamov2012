using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Zamov.Controllers
{
    public static class ResourcesHelper
    {
        public static CultureInfo GetSelectedCulture()
        {
            CultureInfo info = null;
            if (HttpContext.Current.Session["lang"] != null)
            {
                string lang = HttpContext.Current.Session["lang"].ToString();
                info = CultureInfo.GetCultureInfo(lang);
            }
            else
                info = CultureInfo.GetCultureInfo("ru-RU");
            return info;
        }

        public static string CurrentCulture(this System.Web.Mvc.HtmlHelper helper)
        {
            return GetSelectedCulture().Name;
        }

        public static string GetResourceString(string resourceName)
        {
            return HttpContext.GetGlobalResourceObject("Resources", resourceName, GetSelectedCulture()).ToString();
        }
    }
}
