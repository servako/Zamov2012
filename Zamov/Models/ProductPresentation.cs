using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class ProductPresentation
    {
        public int Id { get; set; }
        public bool HasImage { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; }
        public bool TopProduct { get; set; }
        public int GroupId { get; set; }
    }
}
