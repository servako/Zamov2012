using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AjaxControlToolkitMvc;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;
using Zamov.Controllers;

namespace Zamov.Helpers
{
    public class BreadCrumbsTracker : BaseTracker<KeyValuePair<string, string>>
    {
        public BreadCrumbsTracker(HttpContextBase context) : base(context) { }

        protected override void Initialize(HttpContextBase context)
        {
            resourceKey = "__breadCrumbs";
            base.Initialize(context);
            if (Items.Count == 0)
                Items.Add(new KeyValuePair<string, string>(ResourcesHelper.GetResourceString("MainPage"), "/"));
        }

        public List<KeyValuePair<string, string>> Items
        {
            get { return _resources; }
        }
    }

    public static class BreadCrumbsExtensions
    {
        public static string BreadCrumbs(this HtmlHelper helper)
        {
            BreadCrumbsTracker tracker = new BreadCrumbsTracker(helper.ViewContext.HttpContext);
            StringBuilder layout = new StringBuilder();
            int itemsCount = tracker.Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                if (i < itemsCount - 1)
                {
                    layout.Append(BreadCrumbsExtensions.RenderItem(tracker.Items[i]));
                    layout.Append(BreadCrumbsExtensions.RenderSeparator());
                }
                else
                    layout.Append(BreadCrumbsExtensions.RenderLastItem(tracker.Items[i].Key));
            }
            return layout.ToString();
        }

        public static void AddBreadCrumb(HttpContextBase context, string text, string url)
        {
            BreadCrumbsTracker tracker = new BreadCrumbsTracker(context);
            tracker.Add(new KeyValuePair<string, string>(text, url));
        }

        private static string RenderItem(KeyValuePair<string, string> item)
        {
            string template = "<span class=\"breadCrumb\"><a href=\"{0}\">{1}</a></span>";
            return String.Format(template, item.Value, item.Key);
        }

        private static string RenderLastItem(string item)
        {
            string template = "<span class=\"currentBreadCrumb\">{0}</span>";
            return String.Format(template, item);
        }

        private static string RenderSeparator()
        {
            return "<span class=\"breadCrumbSeparator\">/</span>";
        }
    }
}
