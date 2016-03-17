using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AjaxControlToolkitMvc
{
    public class BaseTracker<T>
    {
        protected string resourceKey = "__resources";
        protected List<T> _resources;


        public BaseTracker(HttpContextBase context)
        {
            Initialize(context);
        }

        protected virtual void Initialize(HttpContextBase context)
        {
            _resources = (List<T>)context.Items[resourceKey];
            if (_resources == null)
            {
                _resources = new List<T>();
                context.Items[resourceKey] = _resources;
            }
        }

        public virtual void Add(T item)
        {
            _resources.Add(item);
        }

        public virtual bool Contains(T item)
        {
            return _resources.Contains(item);
        }

        public virtual bool Contains(T item, IEqualityComparer<T> comparer)
        {
            return _resources.Contains(item, comparer);
        }
    }

    public class ResourceTracker : BaseTracker<string>
    {
        public ResourceTracker(HttpContextBase context) : base(context) { }

        public override void Add(string url)
        {
            url = url.ToLower();
            _resources.Add(url);
        }

        public override bool Contains(string url)
        {
            url = url.ToLower();
            return _resources.Contains(url);
        }

    }
}