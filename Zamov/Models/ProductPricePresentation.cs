using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Zamov.Models
{
    public class ProductPricePresentation
    {
        public int Id { get; set; }
        public int IdDealerProd { get; set; }
        public string PartNum { get; set; }
        public string Name { get; set; }
        public string dpName { get; set; }
        public string DealerName { get; set; }
        public int DealerId { get; set; }
        public decimal PriceHrn { get; set; }
        public decimal Price { get; set; }
        public int CurId { get; set; }
        public string CurSign { get; set; }
        public string Unit { get; set; }
        public string dpUnit { get; set; }
        public string Description { get; set; }
        public string dpDescription { get; set; }
        public string Url { get; set; }
        public string dpUrl { get; set; }
        public DateTime DateBegin { get; set; }
        public string State { get; set; }
        public int Quarantee { get; set; }
        public bool Action { get; set; }
        public bool New { get; set; }
        public int IdGroup { get; set; }
        public int? IdBrand { get; set; }
        public bool dpEnable { get; set; }
		public int IdProductByDealer { get; set; }

        public bool CheckBetween(List<ProductPriceRange> range)
        {
            bool res = false;
            foreach (var productPriceRange in range)
            {
                if (productPriceRange.PriceFrom <= PriceHrn && productPriceRange.PriceTo >= PriceHrn)
                {
                    res = true;
                }
            }
            return res;
        }
    }
}
