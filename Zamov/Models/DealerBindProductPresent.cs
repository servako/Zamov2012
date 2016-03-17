using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class DealerBindProductPresent
    {
        public int IdDealer { get; set; }
        public string NameDealer { get; set; }
        public int IdDealerProduct { get; set; }
        public string NameProduct { get; set; }
        public string NameProductMain { get; set; }
        public int IdProductMain { get; set; }
    }
}
