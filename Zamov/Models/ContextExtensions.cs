using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data;
using System.Data.Objects;

namespace Zamov.Models
{
    public static class ContextExtensions
    {
        private static void ExecuteNonQuery(ZamovStorage context, string storedProcedureName, params EntityParameter[] parameters)
        {
            bool closeConnection = false;
            DbCommand command = context.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            command.Parameters.AddRange(parameters);
            if (context.Connection.State != System.Data.ConnectionState.Open)
            {
                context.Connection.Open();
                closeConnection = true;
            }
            command.ExecuteNonQuery();
            if (closeConnection)
                context.Connection.Close();
        }

        public static IQueryable<EntityTranslationPair<Category>> GetTranslatedCategories(
            this ZamovStorage context,
            string language,
            bool enabledOnly,
            int? cityId,
            bool roots
        )
        {
            IQueryable<EntityTranslationPair<Category>> result = context
                .Categories.Include("Parent")
                .Include("Groups")
                .Include("GroupsAdditional")
                .Include("Groups.Dealer")
                .Include("Groups.Dealer.Cities")
                .Join
                (
                    context.Translations.Where(t => t.Language == language && t.TranslationItemTypeId == (int)ItemTypes.Category),
                    c => c.Id,
                    t => t.ItemId, (c, i) => new EntityTranslationPair<Category>
                    {
                        Entity = c,
                        Language = language,
                        Translation = i
                    }
            );

            if (enabledOnly)
            {
                result = result.Where(c => c.Entity.Enabled);
            }
            if (roots)
            {
                result = result.Where(c => c.Entity.Parent == null);
            }
                    
            if (cityId != null)
            {
                result = result.Where(c =>
                    (c.Entity.Groups
                        .Where(g => !g.Deleted && g.Enabled).Any(g => g.Dealer.Cities.Any(city => city.Id == cityId)))
                    ||
                    (c.Entity.Categories.SelectMany(cat => cat.Groups)
                        .Where(g => !g.Deleted && g.Enabled).Any(g => g.Dealer.Cities.Any(city => city.Id == cityId)))
                    ||
                    (from e in c.Entity.GroupsAdditional
                     join gd in context.DealersGroups on e.grID equals gd.GroupsAdditional.grID
                     join dl in context.Dealers on gd.dg_dealerID equals dl.Id
                     where !e.gr_deleted && e.gr_enabled
                           && dl.Cities.Any(city => city.Id == cityId && dl.Enabled)
                     select e).Any()
                    );
            }
            return result;
        }

        private static DbDataReader ExecuteReader(ZamovStorage context, string storedProcedureName, params EntityParameter[] parameters)
        {
            DbCommand command = context.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            command.Parameters.AddRange(parameters);
            if (context.Connection.State != System.Data.ConnectionState.Open)
                context.Connection.Open();
            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }

        public static IQueryable<CategoryPresentation> GetLocalizedCategories(this ZamovStorage context, string language)
        {
            IQueryable<CategoryPresentation> result = (from category in context.Categories.Include("Parent").Include("Dealers").Include("Categories")
                                                       join name in context.Translations on category.Id equals name.ItemId
                                                       where category.Enabled
                                                       && name.Language == language
                                                       && name.TranslationItemTypeId == (int)ItemTypes.Category
                                                       select new CategoryPresentation
                                                       {
                                                           Id = category.Id,
                                                           Name = name.Text
                                                       });
            return result;
        }

        public static void RemoveDealer(this ZamovStorage context, int id)
        {
            EntityParameter parameter = new EntityParameter();
            parameter.ParameterName = "dealerId";
            parameter.IsNullable = false;
            parameter.Value = id;
            parameter.DbType = System.Data.DbType.Int32;
            ExecuteNonQuery(context, "ZamovStorage.deleteDealer", parameter);
        }

        public static void CleanupProductImages(this ZamovStorage context, int productId)
        {
            EntityParameter parameter = new EntityParameter();
            parameter.ParameterName = "productId";
            parameter.IsNullable = false;
            parameter.Value = productId;
            parameter.DbType = System.Data.DbType.Int32;
            ExecuteNonQuery(context, "ZamovStorage.ProductImages_Cleanup", parameter);
        }
        
        public static void CleanupProductManufacturer(this ZamovStorage context, int productId)
        {
            EntityParameter parameter = new EntityParameter();
            parameter.ParameterName = "productId";
            parameter.IsNullable = false;
            parameter.Value = productId;
            parameter.DbType = System.Data.DbType.Int32;
            ExecuteNonQuery(context, "ZamovStorage.ProductManufacturer_Cleanup", parameter);
        }

        public static void CleanupCategoryImages(this ZamovStorage context, int categoryId)
        {
            EntityParameter parameter = new EntityParameter();
            parameter.ParameterName = "categoryId";
            parameter.IsNullable = false;
            parameter.Value = categoryId;
            parameter.DbType = System.Data.DbType.Int32;
            ExecuteNonQuery(context, "ZamovStorage.CategoryImages_Cleanup", parameter);
        }

        public static void UpdateProducts(this ZamovStorage context, string updatesXml)
        {
            EntityParameter parameter = new EntityParameter();
            parameter.ParameterName = "updatesXml";
            parameter.IsNullable = false;
            parameter.Value = updatesXml;
            parameter.DbType = System.Data.DbType.String;
            ExecuteNonQuery(context, "ZamovStorage.UpdateProducts", parameter);
        }

		public static void UpdateProductsAdditional(this ZamovStorage context, string updatesXml)
		{
			EntityParameter parameter = new EntityParameter();
			parameter.ParameterName = "updatesXml";
			parameter.IsNullable = false;
			parameter.Value = updatesXml;
			parameter.DbType = System.Data.DbType.String;
			ExecuteNonQuery(context, "ZamovStorage.UpdateProductsAdditional", parameter);
		}

        public static void InsertImportedProducts(this ZamovStorage context, string updatesXml, int dealerId)
        {
            EntityParameter dealerIdParameter = new EntityParameter();
            dealerIdParameter.ParameterName = "dealerId";
            dealerIdParameter.IsNullable = false;
            dealerIdParameter.Value = dealerId;
            dealerIdParameter.DbType = System.Data.DbType.Int32;

            EntityParameter updatesXmlParameter = new EntityParameter();
            updatesXmlParameter.ParameterName = "updatesXml";
            updatesXmlParameter.IsNullable = false;
            updatesXmlParameter.Value = updatesXml;
            updatesXmlParameter.DbType = System.Data.DbType.String;

            ExecuteNonQuery(context, "ZamovStorage.InsertImportedProducts", dealerIdParameter, updatesXmlParameter);
        }

        public static void UpdateImportedProducts(this ZamovStorage context, string updatesXml)
        {
            EntityParameter updatesXmlParameter = new EntityParameter();
            updatesXmlParameter.ParameterName = "updatesXml";
            updatesXmlParameter.IsNullable = false;
            updatesXmlParameter.Value = updatesXml;
            updatesXmlParameter.DbType = System.Data.DbType.String;

            ExecuteNonQuery(context, "ZamovStorage.UpdateImportedProducts", updatesXmlParameter);
        }

        public static void CopyDealer(this ZamovStorage context, int dealerId, int cityId)
        {
            EntityParameter dealerIdParameter = new EntityParameter();
            dealerIdParameter.ParameterName = "dealerId";
            dealerIdParameter.IsNullable = false;
            dealerIdParameter.Value = dealerId;
            dealerIdParameter.DbType = DbType.Int32;

            EntityParameter cityIdParameter = new EntityParameter();
            cityIdParameter.ParameterName = "cityId";
            cityIdParameter.IsNullable = false;
            cityIdParameter.Value = cityId;
            cityIdParameter.DbType = DbType.Int32;

            ExecuteNonQuery(context, "ZamovStorage.CopyDealer", dealerIdParameter, cityIdParameter);
        }

        public static void ExpireOrders(this ZamovStorage context)
        {
            ExecuteNonQuery(context, "ZamovStorage.ExpireOrders");
        }

        public static bool MatchesPath(this Group g, string[] path)
        {
            bool result = false;
            if (path != null && path.Length == 1 && g.Name == path[0])
                result = true;
            else
            {
                g.ParentReference.Load();
                if (path != null && path.Length > 1 && g.Parent != null)
                    result = g.Parent.MatchesPath(path.Take(path.Length - 1).ToArray());
            }
            return result;
        }

        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
            (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                              : source.OrderBy(keySelector);
        }
    }
}
