using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class NewsPresentation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public DateTime Date { get; set; }
        public bool Enabled { get; set; }
    }
}
