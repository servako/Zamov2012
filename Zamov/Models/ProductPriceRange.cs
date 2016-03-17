using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class ProductPriceRange
    {
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public int Count { get; set; }
        public bool IsCheck { get; set; }

        public void CheckBetween(List<ProductByGroupPresent> productsByGroupPresent)
        {
            Count = 0;
            foreach (var product in productsByGroupPresent)
            {
                foreach (var price in product.Prices)
                {
                    if (PriceFrom <= price && PriceTo >= price)
                    {
                        Count += 1;
                    }
                }
            }
        }

    }
}