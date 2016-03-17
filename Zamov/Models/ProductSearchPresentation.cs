using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class ProductSearchPresentation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DealerName { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public int DealerId { get; set; }
        public string Description { get; set; }
        public string DealerNameId { get; set; }
        public int Version { get; set; }
        public string Url { get; set; }
    }
}
