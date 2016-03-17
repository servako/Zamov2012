using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration;
using System.Globalization;
using System.Data.Objects;

namespace Zamov.Models
{
    public static class MembershipExtensions
    {
        public static int[] GetOnlineDealers()
        {
            int[] result = null;
            using (MembershipStorage context = new MembershipStorage())
            {
                DateTime verificationTime = DateTime.Now.ToUniversalTime().AddMinutes(-3);
                var profiles = (from
                                    user in context.aspnet_Users.Include("aspnet_Roles").Include("aspnet_Profile")
                                where
                                    user.LastActivityDate > verificationTime
                                    && user.aspnet_Roles.Where(r => r.RoleName == "Dealers").Count() > 0
                                select
                                    new
                                    {
                                        PropertyNames = user.aspnet_Profile.PropertyNames,
                                        PropertyValues = user.aspnet_Profile.PropertyValuesString
                                    });

                result = new int[profiles.Count()];
                int i = 0;
                foreach (var item in profiles)
                {
                    result[i] = ExtractDealerId(item.PropertyNames, item.PropertyValues);
                    i++;
                }
            }
            return result;
        }

        private static int ExtractDealerId(string propertyNames, string propertyValues)
        {
            string[] properties = propertyNames.Split(':');
            int endPosition = int.Parse(properties[3]);
            return int.Parse(propertyValues.Substring(0, endPosition));
        }


        private static Dictionary<string, string> GetProfileProperties(string[] names, string values)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (names != null && values != null)
            {
                try
                {
                    for (int i = 0; i < (names.Length / 4); i++)
                    {
                        string str = names[i * 4];
                        int startIndex = int.Parse(names[(i * 4) + 2], CultureInfo.InvariantCulture);
                        int length = int.Parse(names[(i * 4) + 3], CultureInfo.InvariantCulture);
                        if (((names[(i * 4) + 1] == "S") && (startIndex >= 0)) && ((length > 0) && (values.Length >= (startIndex + length))))
                        {
                            result[str] = values.Substring(startIndex, length);
                        }
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        public static UserPresentation GetUserPresentation(string userId)
        {
            UserPresentation result = new UserPresentation();
            using (MembershipStorage context = new MembershipStorage())
            {

                var userRecord = (from user in context.aspnet_Users.Include("aspnet_Profile")
                                  where user.UserName == userId
                                  select new
                                  {
                                      email = user.UserName,
                                      profileProperties = user.aspnet_Profile.PropertyNames,
                                      profileValues = user.aspnet_Profile.PropertyValuesString
                                  }).FirstOrDefault();

                Dictionary<string, string> profileProperties = GetProfileProperties(userRecord.profileProperties.Split(':'), userRecord.profileValues);
                result = GetUserPresentation(profileProperties);
                if (result != null)
                    result.Email = userId;
            }
            return result;
        }

        public static List<UserPresentation> GetUserPresentations(string[] userId)
        {
            List<UserPresentation> result = new List<UserPresentation>();
            using (MembershipStorage context = new MembershipStorage())
            {
                string aspnet_Users = string.Join(",", userId);
                ObjectQuery<aspnet_Users> query = new ObjectQuery<aspnet_Users>(
                    "SELECT VALUE user from aspnet_Users as user WHERE user.UserName IN {" + aspnet_Users + "}", 
                    context);

                var userRecords = (from user in query.Include("aspnet_Profile")
                                  select new
                                  {
                                      email = user.UserName,
                                      profileProperties = user.aspnet_Profile.PropertyNames,
                                      profileValues = user.aspnet_Profile.PropertyValuesString
                                  });

                foreach (var userRecord in userRecords)
                {
                    Dictionary<string, string> profileProperties = GetProfileProperties(userRecord.profileProperties.Split(':'), userRecord.profileValues);
                    UserPresentation item = GetUserPresentation(profileProperties);
                    if (item != null)
                    {
                        item.Email = userRecord.email;
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private static UserPresentation GetUserPresentation(Dictionary<string, string> profileProperties)
        {
            UserPresentation result = null;
            if (profileProperties != null)
            {
                result = new UserPresentation();
                if (profileProperties.ContainsKey("FirstName"))
                    result.FirstName = profileProperties["FirstName"];
                if (profileProperties.ContainsKey("LastName"))
                    result.LastName = profileProperties["LastName"];
                if (profileProperties.ContainsKey("MobilePhone"))
                    result.MobilePhone = profileProperties["MobilePhone"];
                if (profileProperties.ContainsKey("Phone"))
                    result.Phone = profileProperties["Phone"];
                if (profileProperties.ContainsKey("City"))
                    result.City = profileProperties["City"];
                if (profileProperties.ContainsKey("DeliveryAddress"))
                    result.DeliveryAddress = profileProperties["DeliveryAddress"];
                if (profileProperties.ContainsKey("DealerEmployee"))
                {
                    result.DealerEmployee = bool.Parse(profileProperties["DealerEmployee"]);
                    if (result.DealerEmployee)
                    {
                        result.DealerId = int.Parse(profileProperties["DealerId"]);
                    }
                }
            }
            return result;
        }

        public static List<UserPresentation> GetAllUsers(this MembershipStorage context)
        {
            List<UserPresentation> result = new List<UserPresentation>();

            var users = (from user in context.aspnet_Users.Include("aspnet_Profile")
                         where user.UserName != "root" && user.UserName != "root2" && user.UserName != "root0"
                         select new
                             {
                                 email = user.UserName,
                                 profileProperties = user.aspnet_Profile.PropertyNames,
                                 profileValues = user.aspnet_Profile.PropertyValuesString
                             }).ToList();

            foreach (var item in users)
            {
                if (item.profileProperties == null || item.profileValues == null) continue;
                Dictionary<string, string> profileProperties = GetProfileProperties(item.profileProperties.Split(':'), item.profileValues);
                UserPresentation user = GetUserPresentation(profileProperties);
                if (user != null)
                {
                    user.Email = item.email;
                    result.Add(user);
                }
            }
            return result;
        }

        private class Profile
        {
            public string PropertyNames;
            public string PropertyValues;
            public Guid? UserId;
        }

        public static string GetDealerEmail(int dealerId)
        {
            string dealerEmail = "";
            if (HttpContext.Current.Cache["dealerEmail" + dealerId] == null)
            {
                using (MembershipStorage context = new MembershipStorage())
                {
                    var profiles = (from
                                        user in context.aspnet_Users.Include("aspnet_Roles").Include("aspnet_Profile")
                                    where
                                        user.aspnet_Roles.Where(r => r.RoleName == "Dealers").Count() > 0
                                    select
                                        new Profile
                                        {
                                            PropertyNames = user.aspnet_Profile.PropertyNames,
                                            PropertyValues = user.aspnet_Profile.PropertyValuesString,
                                            UserId = user.aspnet_Profile.UserId
                                        }).ToList();

                    profiles = profiles.Where(p => p.UserId != null).Select(p => p).ToList();
                    foreach (var item in profiles)
                    {
                        int id = ExtractDealerId(item.PropertyNames, item.PropertyValues);
                        if (dealerId == id)
                            dealerEmail = Membership.GetUser(item.UserId).Email;
                    }
                }
                HttpContext.Current.Cache["dealerEmail" + dealerId] = dealerEmail;
            }
            dealerEmail = HttpContext.Current.Cache["dealerEmail" + dealerId].ToString();
            return dealerEmail;
        }
    }
}