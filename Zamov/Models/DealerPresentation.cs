using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class DealerPresentation
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool OnLine { get; set; }
        public bool Card { get; set; }
        public bool Cash { get; set; }
        public bool Noncash { get; set; }
        public bool HasDiscounts { get; set; }
        public bool TopDealer { get; set; }
        public bool Enabled { get; set; }
        public int IndexNumber { get; set; }
    }
}
