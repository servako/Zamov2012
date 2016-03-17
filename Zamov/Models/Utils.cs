using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using Zamov.Models;
using System.IO;
using System.Text;
using System.Collections;
using Excel;

namespace Zamov.Models
{
    public static class Utils
    {
        public static string CreateTranslationXml(int itemId, ItemTypes itemType, Dictionary<string, string> translations)
        {
            XmlDocument document = new XmlDocument();
            XmlElement node = document.CreateElement("root");
            document.AppendChild(node);
            foreach (string key in translations.Keys)
            {
                XmlElement item = document.CreateElement("translation");
                XmlAttribute language = document.CreateAttribute("Language");
                language.Value = key;
                XmlAttribute itemIdAttribute = document.CreateAttribute("ItemId");
                itemIdAttribute.Value = itemId.ToString();
                XmlAttribute translationItemTypeId = document.CreateAttribute("TranslationItemTypeId");
                translationItemTypeId.Value = ((int)itemType).ToString();
                XmlAttribute translation = document.CreateAttribute("Translation");
                translation.Value = translations[key];
                item.Attributes.Append(language);
                item.Attributes.Append(itemIdAttribute);
                item.Attributes.Append(translationItemTypeId);
                item.Attributes.Append(translation);
                node.AppendChild(item);
            }
            return document.OuterXml;
        }

        public static string CreateTranslationXml(List<TranslationItem> translations)
        {
            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("root");
            document.AppendChild(root);
            foreach (var translationItem in translations)
            {
                XmlElement item = document.CreateElement("translation");
                XmlAttribute language = document.CreateAttribute("Language");
                language.Value = translationItem.Language;
                XmlAttribute itemIdAttribute = document.CreateAttribute("ItemId");
                itemIdAttribute.Value = translationItem.ItemId.ToString();
                XmlAttribute translationItemTypeId = document.CreateAttribute("TranslationItemTypeId");
                translationItemTypeId.Value = ((int)translationItem.ItemType).ToString();
                XmlAttribute translation = document.CreateAttribute("Translation");
                translation.Value = translationItem.Translation;
                item.Attributes.Append(language);
                item.Attributes.Append(itemIdAttribute);
                item.Attributes.Append(translationItemTypeId);
                item.Attributes.Append(translation);
                root.AppendChild(item);
            }
            return document.OuterXml;
        }

        public static string CreateUpdatesXml(this Dictionary<string, Dictionary<string, string>> updates)
        {
            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("root");
            document.AppendChild(root);
            foreach (string key in updates.Keys)
            {
                Dictionary<string, string> itemUpdates = updates[key];
                XmlElement item = document.CreateElement("item");
                XmlAttribute itemIdAttribute = document.CreateAttribute("Id");
                itemIdAttribute.Value = key;
                item.Attributes.Append(itemIdAttribute);
                foreach (string itemKey in itemUpdates.Keys)
                {
                    XmlAttribute itemAttribute = document.CreateAttribute(itemKey);
                    itemAttribute.Value = itemUpdates[itemKey];
                    item.Attributes.Append(itemAttribute);
                }
                root.AppendChild(item);
            }
            return document.OuterXml;
        }

        public static List<Dictionary<string, string>> QureyUploadedXls(string fileName, int dealerId, int? groupId)
        {
            DataSet result = GetExcelDataSet(fileName);

            List<Dictionary<string, string>> importedItems = result.Tables[0].ToDictionaryList();

            MarkImportedCorrespondences(importedItems, dealerId, groupId);
            return importedItems;
        }

        private static DataSet GetExcelDataSet(string fileName)
        {
            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            string extension = Path.GetExtension(fileName);
            IExcelDataReader excelReader = null;
            if (extension == ".xls")
                excelReader = Factory.CreateReader(stream, ExcelFileType.Binary);
            if (extension == ".xlsx")
                excelReader = Factory.CreateReader(stream, ExcelFileType.OpenXml);
            DataSet result = new DataSet();
            DataTable table = new DataTable("dtExcel");
            bool columnDefinitions = true;
            List<string> columns = new List<string>();
            columns.Add("partNumber");
            columns.Add("name");
            columns.Add("price");
            columns.Add("unit");
            columns.Add("ukDescription");
            columns.Add("ruDescription");
            columns.Add("imageUrl");
            while (excelReader.Read())
            {
                if (columnDefinitions)
                {
                    for (int i = 0; i < excelReader.FieldCount; i++)
                        table.Columns.Add(columns[i], typeof(string));
                    columnDefinitions = false;
                }
                else
                {
                    DataRow row = table.NewRow();
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (table.Columns[i].ColumnName == "price")
                        {
                            string rowValue = excelReader.GetString(i);
                            if(!string.IsNullOrEmpty(rowValue))
                                rowValue = rowValue.Replace(",", ".");
                            row[table.Columns[i]] = rowValue;
                        }
                        else
                        {
                            row[table.Columns[i]] = excelReader.GetString(i);
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            result.Tables.Add(table);

            excelReader.Close();
            stream.Close();
            return result;
        }

        private static DataSet GetExcelDataSetOleDb(string fileName)
        {
            DataSet result = new DataSet();
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=Excel 8.0;";

            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();

            OleDbCommand command = new OleDbCommand("Select * from [Sheet1$]", connection);
            OleDbDataAdapter adapter = new OleDbDataAdapter(command);
            adapter.Fill(result);
            connection.Close();
            return result;
        }

        public static List<Dictionary<string, string>> ToDictionaryList(this System.Data.DataTable table)
        {
            List<Dictionary<string, string>> importedItems = new List<Dictionary<string, string>>();
            foreach (System.Data.DataRow row in table.Rows)
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                foreach (DataColumn column in table.Columns)
                    item.Add(column.ColumnName, row[column].ToString());
                importedItems.Add(item);
            }
            return importedItems;
        }

        private static void MarkImportedCorrespondences(List<Dictionary<string, string>> importedItems, int dealerId, int? groupId)
        {
            Dictionary<string, Product> products = new Dictionary<string, Product>();
            using (ZamovStorage context = new ZamovStorage())
            {
                products = (from product in context.Products
                            where product.Deleted == false && product.Dealer.Id == dealerId
                            && !product.Deleted && product.Group.Name != "TRASH"
                            //&& (groupId == null || product.Group.Id == groupId.Value)
                            select product).ToDictionary(p => p.PartNumber, p => p);
            }

            foreach (var item in importedItems)
            {
                string partNumber = item["partNumber"];

                Product product = null;
                if (products.Keys.Contains(partNumber))
                    product = products[partNumber];
                item["productId"] = (product != null) ? product.Id.ToString() : null;
            }
        }

        public static int CompareObjectFields<T>(T a, T b, string fieldName, int direction)
        {
            IComparable valueA = (IComparable)a.GetType().GetProperty(fieldName).GetValue(a, null);
            IComparable valueB = (IComparable)a.GetType().GetProperty(fieldName).GetValue(b, null);

            int result = 0;
            if (valueA == null && valueB == null)
                result = 0;
            else if (valueA == null)
                result = -1;
            else if (valueB == null)
                result = 1;
            else
                result = valueA.CompareTo(valueB);

            return result * direction;

        }
    }
}