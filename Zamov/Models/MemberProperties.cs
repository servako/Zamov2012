using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    [Serializable]
    public class MemberProperties
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string Phone { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
    }
}
