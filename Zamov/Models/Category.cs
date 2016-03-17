using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Data.Common;
using Zamov.Controllers;

namespace Zamov.Models
{
    public partial class Category
    {
        private Dictionary<string, string> names = new Dictionary<string, string>();
        private Dictionary<string, string> subCategoryNames = new Dictionary<string, string>();
        
        public Dictionary<string, string> SubCategoryNames
        {
            get { return subCategoryNames; }
            set { subCategoryNames = value; }
        }

        public Dictionary<string, string> Names
        {
            get
            {
                return names;
            }
        }

        public void LoadNames()
        {
            using (ZamovStorage context = new ZamovStorage())
                names = (from translation in context.Translations
                         where (translation.ItemId == this.Id && translation.TranslationItemTypeId == (int)ItemTypes.Category)
                         select new { lang = translation.Language, val = translation.Text })
                    .ToDictionary(k => k.lang, v => v.val);
        }

        public void LoadSubCategoryNames()
        {
            using (ZamovStorage context = new ZamovStorage())
                subCategoryNames = (from translation in context.Translations
                         where (translation.ItemId == this.Id && translation.TranslationItemTypeId == (int)ItemTypes.SubCategoriesName)
                         select new { lang = translation.Language, val = translation.Text })
                    .ToDictionary(k => k.lang, v => v.val);
        }

        public string NamesXml
        {
            get
            {
                return Utils.CreateTranslationXml(this.Id, ItemTypes.Category, Names);
            }
        }

        public string SubCategoryNamesXml
        {
            get
            {
                return Utils.CreateTranslationXml(this.Id, ItemTypes.SubCategoriesName, SubCategoryNames);
            }
        }

        public string GetSubCategoryName(string language)
        {
            if (SubCategoryNames.Count == 0)
                LoadSubCategoryNames();
            string result = "";
            if (SubCategoryNames.Keys.Contains(language))
                result = SubCategoryNames[language];
            return result;
        }

        public string GetName(string language)
        {
            if (Names.Count == 0)
                LoadNames();
            return GetName(language, true);
        }

        public string GetName(string language, bool replaceWithDefault)
        {
            string result = (replaceWithDefault) ? Name : "";
            if (Names.Keys.Contains(language))
                result = Names[language];
            return result;
        }
    }
}
