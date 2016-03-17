using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zamov.Models
{
    public class GroupResentation
    {
        private List<GroupResentation> children = new List<GroupResentation>();
        

        public string Name { get; set; }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public List<GroupResentation> Children { get { return children; } }
        public GroupResentation Parent{get; private set;}
        public string DealerName { get; set; }
        public int CategoryId { get; set; }

        public void PickChildren(List<GroupResentation> source)
        {
            children = (from item in source where item.ParentId == Id select item).ToList();
        }

        public void PickParent(List<GroupResentation> source)
        {
            if (ParentId != null)
                Parent = (from item in source where item.Id == ParentId.Value select item).FirstOrDefault();
        }
    }
}
