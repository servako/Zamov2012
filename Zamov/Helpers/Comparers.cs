using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zamov.Models;

namespace Zamov.Helpers
{

    //TODO: http://social.msdn.microsoft.com/Forums/en-US/adodotnetentityframework/thread/22011e56-1269-44ec-947c-6b6fe2ad42cb


    public class PSortByProductNameAsc : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Name.CompareTo(y.Name);
        }
    }

    public class PSortByProductNameDesc : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Name.CompareTo(x.Name);
        }
    }

    public class PSortByPriceAsc : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Price.CompareTo(y.Price);
        }
    }
    public class PSortByPriceDesc : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Price.CompareTo(x.Price);
        }
    }

////////////////////////////
    
    public class SortByProductNameAsc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Name.CompareTo(y.Name);
        }
    }

    public class SortByProductNameDesc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Name.CompareTo(x.Name);
        }
    }

    public class SortByDealerNameAsc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.DealerName.CompareTo(y.DealerName);
        }
    }
    public class SortByDealerNameDesc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.DealerName.CompareTo(x.DealerName);
        }
    }

    public class SortByPriceAsc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Price.CompareTo(y.Price);
        }
    }
    public class SortByPriceDesc : IComparer<ProductSearchPresentation>
    {
        public int Compare(ProductSearchPresentation x, ProductSearchPresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Price.CompareTo(x.Price);
        }
    }

    #region comparers for ProductByGroupPresent
    public class SortProductByGroupPresentByNameAsc : IComparer<ProductByGroupPresent>
            {
        public int Compare(ProductByGroupPresent x, ProductByGroupPresent y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Name.CompareTo(y.Name);
        }
    }

    public class SortProductByGroupPresentByNameDesc : IComparer<ProductByGroupPresent>
    {
        public int Compare(ProductByGroupPresent x, ProductByGroupPresent y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Name.CompareTo(x.Name);
        }
    }

    public class SortProductByGroupPresentByPriceAsc : IComparer<ProductByGroupPresent>
    {
        public int Compare(ProductByGroupPresent x, ProductByGroupPresent y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.PriceMin.CompareTo(y.PriceMin);
        }
    }
    public class SortProductByGroupPresentByPriceDesc : IComparer<ProductByGroupPresent>
    {
        public int Compare(ProductByGroupPresent x, ProductByGroupPresent y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.PriceMin.CompareTo(x.PriceMin);
        }
    }
#endregion

#region comparers for ProductPricePresentation
    public class SortProductPricePresentationByNameAsc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.Name.CompareTo(y.Name);
        }
    }

    public class SortProductPricePresentationByNameDesc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.Name.CompareTo(x.Name);
        }
    }

    public class SortProductPricePresentationByDealerNameAsc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.DealerName.CompareTo(y.DealerName);
        }
    }
    public class SortProductPricePresentationByDealerNameDesc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.DealerName.CompareTo(x.DealerName);
        }
    }

    public class SortProductPricePresentationByPriceAsc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return x.PriceHrn.CompareTo(y.PriceHrn);
        }
    }
    public class SortProductPricePresentationByPriceDesc : IComparer<ProductPricePresentation>
    {
        public int Compare(ProductPricePresentation x, ProductPricePresentation y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else if (y == null)
                return 1;
            else
                return y.PriceHrn.CompareTo(x.PriceHrn);
        }
    }
#endregion
}
