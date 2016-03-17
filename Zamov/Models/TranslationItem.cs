using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public struct TranslationItem
    {
        public int ItemId { get; set; }
        public ItemTypes ItemType { get; set; }
        public string Language { get; set; }
        public string Translation { get; set; }
    }
}
