using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zamov.Controllers;

namespace Zamov.Helpers
{
    public static class HierarchicalDropDownHelper
    {
        public static string HierarchicalDropDown<T>(this HtmlHelper html, string name, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemText, Func<T, string> itemValue, Func<T, bool> selectedCheck, object htmlAttributes)
        {
            return HierarchicalDropDown(html, name, rootItems, childrenProperty, itemText, itemValue, selectedCheck, htmlAttributes, false);
        }

		public static string HierarchicalDropDown<T>(this HtmlHelper html, string name, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemText, Func<T, string> itemValue, Func<T, bool> selectedCheck, object htmlAttributes, bool insertItemFirtTextChoice)
		{
			List<SelectListItem> items = new List<SelectListItem>();
			if (insertItemFirtTextChoice)
			{
				items.Add(new SelectListItem {Text = ResourcesHelper.GetResourceString("SelectGroup"), Value = "-1"});
			}
			foreach (var item in rootItems)
			{
				bool selected = false;
				if (selectedCheck != null)
					selected = selectedCheck(item);
				items.Add(new SelectListItem { Text = itemText(item), Value = itemValue(item), Selected = selected });
				AppendChildren(items, item, childrenProperty, itemText, itemValue, selectedCheck, 1);
			}
			return html.DropDownList(name, items, htmlAttributes).ToString();
		}

        public static string HierarchicalDropDown<T>(this HtmlHelper html, string name, IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemText, Func<T, string> itemValue, Func<T, bool> selectedCheck, object htmlAttributes, List<SelectListItem> items)
        {
            foreach (var item in rootItems)
            {
                bool selected = false;
                if (selectedCheck != null)
                    selected = selectedCheck(item);
                items.Add(new SelectListItem { Text = itemText(item), Value = itemValue(item), Selected = selected });
                AppendChildren(items, item, childrenProperty, itemText, itemValue, selectedCheck, 1);
            }
            return html.DropDownList(name, items, htmlAttributes).ToString();
        }

        private static void AppendChildren<T>(List<SelectListItem> items, T root, Func<T, IEnumerable<T>> childrenProperty, Func<T, string> itemText, Func<T, string> itemValue, Func<T, bool> selectedCheck, int level)
        {
            var children = childrenProperty(root);
            foreach (T item in children)
            {
                bool selected = false;
                if (selectedCheck != null)
                    selected = selectedCheck(item);
                items.Add(new SelectListItem { Text = GetPrefix(level) + itemText(item), Value = itemValue(item), Selected = selected });
                AppendChildren(items, item, childrenProperty, itemText, itemValue, selectedCheck, level + 1);
            }
        }

        static string GetPrefix(int level)
        {
            string result = "";
            for (int i = 0; i < level; i++)
            {
                result += "--";
            }
            return result;
        }
    }


}
