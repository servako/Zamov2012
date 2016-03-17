using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Controllers
{
    public static class Extensions
    {
        public static TElement SingleOrDefault<TElement>(this IQueryable<TElement> query)
        {
            if (query.Count() == 1)
                return query.First();
            else if (query.Count() == 0)
                return default(TElement);
            else
                throw new InvalidOperationException();
        }

    }
}
