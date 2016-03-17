using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class ProductByGroupPresent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public List<decimal> Prices { get; set; }
        public bool Action { get; set; }
        public bool New { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceAvg { get; set; }
        public decimal PriceMax { get; set; }
        public int Proposals { get; set; }
        public int? IdBrand { get; set; }
        
        public ProductByGroupPresent()
        {
            Prices = new List<decimal>();
        }
    }
}
