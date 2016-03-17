using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class CategoryPresentation
    {
        private List<CategoryPresentation> children = new List<CategoryPresentation>();

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected;
        public int? ParentId { get; set; }
        public bool Additional { get; set; }
        public int IndexNumber { get; set; }
        public List<CategoryPresentation> Children { get { return children; } }

        public void PickChildren(List<CategoryPresentation> source)
        {
            children = (from item in source where item.ParentId == Id select item).ToList();
        }

        public void SetChildrenAdditional(List<CategoryPresentation> source)
        {
            children.AddRange(source);
        }
    }
}
