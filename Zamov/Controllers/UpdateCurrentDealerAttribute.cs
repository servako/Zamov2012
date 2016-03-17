using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zamov.Models;

namespace Zamov.Controllers
{
    public class UpdateCurrentDealerAttribute : FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string userName = filterContext.HttpContext.User.Identity.Name;
                if (Roles.IsUserInRole(userName, "Dealers"))
                {
                    ProfileCommon profile = ProfileCommon.Create(userName);
                    SystemSettings.CurrentDealer = profile.DealerId;
                }
            }
        }
    }
}
