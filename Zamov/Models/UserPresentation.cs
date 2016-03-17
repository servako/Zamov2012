using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class UserPresentation
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string Phone { get; set; }
        public bool DealerEmployee { get; set; }
        public int DealerId { get; set; }
        public string DealerName { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }

        public UserPresentation()
        {
            DealerEmployee = false;
            DealerId = int.MinValue;
        }
    }
}
