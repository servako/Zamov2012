using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Zamov.Models;

namespace Zamov.Controllers
{
    public static class Security
    {
        public static bool AllowDealerInteractions(string userName, int dealerId)
        {
            bool result = false;
            if (Roles.IsUserInRole(userName, "Administrators"))
                result = true;
            if (Roles.IsUserInRole(userName, "Dealers"))
            {
                ProfileCommon profile = ProfileCommon.Create(userName);
                if (profile.DealerEmployee && profile.DealerId == dealerId)
                    result = true;
            }
            return result;
        }

        public static int GetCurentDealerId(string userName)
        {
            int result = int.MinValue;
            if (Roles.IsUserInRole(userName, "Administrators") && SystemSettings.CurrentDealer != null)
                result = SystemSettings.CurrentDealer.Value;
            if (Roles.IsUserInRole(userName, "Dealers"))
            {
                if (SystemSettings.CurrentDealer != null)
                    result = SystemSettings.CurrentDealer.Value;
                else
                {
                    ProfileCommon profile = ProfileCommon.Create(userName);
                    result = profile.DealerId;
                    SystemSettings.CurrentDealer = result;
                }
            }
            return result;
        }
    }
}