using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class DealerBindBrandPresent
    {
        public int IdDealer { get; set; }
        public string NameDealer { get; set; }
        public int IdDealerBrand { get; set; }
        public string NameBrand { get; set; }
        public string NameBrandMain { get; set; }
        public int IdBrandMain { get; set; }
    }
}
