using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class ProductPriceRangePresent
    {
        public ProductPriceRangePresent()
        {
            ListProductPriceRange = new List<ProductPriceRange>();
            CustomProductPriceRange = new ProductPriceRange();
        }

        public List<ProductPriceRange> ListProductPriceRange { get; set; }
        public ProductPriceRange CustomProductPriceRange { get; set; }
    }
}