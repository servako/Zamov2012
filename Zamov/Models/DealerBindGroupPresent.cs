using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class DealerBindGroupPresent
    {
        public int IdDealer { get; set; }
        public string NameDealer { get; set; }
        public int IdDealerGroup { get; set; }
        public string NameGroup { get; set; }
        public string NameGroupMain { get; set; }
        public int IdGroupMain { get; set; }
    }
}
