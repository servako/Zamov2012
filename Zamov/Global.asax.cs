using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zamov
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("captcha.ashx");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "DealersGroups", // Route name
                "Admin/DealersGroups/{page}",
                new { controller = "Admin", action = "DealersGroups", page = 1 }
                );
            
            routes.MapRoute(
                "DealersProducts", // Route name
                "Admin/DealersProducts/{page}",
                new { controller = "Admin", action = "DealersProducts", page = 1 }
                );
            
            routes.MapRoute(
                "ProductsAddit",
                "Admin/Products/{idGroup}",
                new { controller = "Admin", action = "ProductsAddit", idGroup = "-1" }
            );

            routes.MapRoute(
                "SeoSetting",
                "Admin/SeoSettings/{id}/{seoEntityType}",
                new { controller = "Admin", action = "SeoSettings", id = "", seoEntityType="" }
            );

			routes.MapRoute(
				"ProductsAdditional",
				"DealerCabinet/ProductsAdditional/{idGroup}",
				new { controller = "DealerCabinet", action = "ProductsAdditional", idGroup = "-2" }
			);

            routes.MapRoute(
            "ProductImageDefault",                                              // Route name
                "Image/ProductImageDefault/{id}/{maxDimension}",                           // URL with parameters
                new { controller = "Image", action = "ProductImageScaled", id = "", maxDimension = "100" }  // Parameter defaults
            );

            routes.MapRoute(
            "DealerImage",                                              // Route name
                "Image/DealerImage/{id}/{maxDimension}",                           // URL with parameters
                new { controller = "Image", action = "DealerImage", id = "", maxDimension = "100" }  // Parameter defaults
            );

            routes.MapRoute(
            "ProductImageScaled",                                              // Route name
                "Image/ProductImageScaled/{id}/{maxDimension}",                           // URL with parameters
                new { controller = "Image", action = "ProductImageScaled", id = "", maxDimension = "100" }  // Parameter defaults
            );

            routes.MapRoute(
			"ProdBrands",                                              // Route name
			"Products/Brands/{id}",
				new { controller = "Products", action = "Brands", id="" }
			);

			routes.MapRoute(
            "AddToCartForProductAdditional",                                              // Route name
                "Products/AddToCartAdditional",                           // URL with parameters
                new { controller = "Products", action = "AddToCartForProductAdditional" }  // Parameter defaults
            );

            routes.MapRoute(
             "AddToCart",                                              // Route name
                 "Products/AddToCart",                           // URL with parameters
                 new { controller = "Products", action = "AddToCart" }  // Parameter defaults
             );

            routes.MapRoute(
            "ProductDescription",                                              // Route name
                "Products/Description/{id}/{vers}",                           // URL with parameters
                new { controller = "Products", action = "Description", id = "", vers="1" }  // Parameter defaults
            );

            routes.MapRoute(
              "ShowFilters",                                              // Route name
              "Products/Filters/{idCity}",
                new { controller = "Products", action = "Filters", idCity = 1 }
            );

            routes.MapRoute(
                "ShowProductsCateg",
                "Products/ShowCategory/{idCity}/{id}/{page}",
                new { controller = "Products", action = "ShowProductsCategory", idCity = 1, id = "", page = 1 }
            );

            routes.MapRoute(
                "ShowProducts",                                              // Route name
                "Products/Show/{idCity}/{id}/{page}",
                new { controller = "Products", action = "ShowProducts", idCity = 1, id = "", page = 1 }
                //new { id = @"\d{1,5}" }  // Parameter defaults
            );

            routes.MapRoute(
                "ShowProductPrices",                                              // Route name
                "Products/ShowPrices/{idCity}/{id}/{page}",
                new { controller = "Products", action = "ShowProductPrices", idCity = 1, id = "", page = 1 }
                //new { id = @"\d{1,5}" }  // Parameter defaults
            );

            routes.MapRoute(
                "Products",                                              // Route name
                "Products/{dealerId}/{categoryId}/{groupId}",                           // URL with parameters
                new { controller = "Products", action = "Index", dealerId = "", categoryId = "", groupId = "" }
                //new { dealerId = @"\d{1,5}", categoryId = @"\d{1,5}", groupId = @"\d{1,5}" }// Parameter defaults
            );

            routes.MapRoute(
                "Dealers",                                              // Route name
                "Dealers/{id}",
                new { controller = "Dealers", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Dealer",                                              // Route name
                "Dealer/{id}",
                new { controller = "Dealer", action = "Index", id = "" }  // Parameter defaults
            );
            routes.MapRoute(
                "DeleteFeedback",                                              // Route name
                "Feedback/DeleteFeedback",
                new { controller = "Feedback", action = "DeleteFeedback", dealerId = "", feedbackId = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Feedback",                                              // Route name
                "Feedback/{id}",
                new { controller = "Feedback", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "FeedbackProductsAdditional",                           // Route name
                "Feedbacks/{prodId}",
                new { controller = "Feedback", action = "FeedbackProductsAdditional", prodId = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "AboutUs",                                              // Route name
                "Home/About",                           // URL with parameters
                new { controller = "Home", action = "About" }  // Parameter defaults
            );

            routes.MapRoute(
                "Ukrainian",                                              // Route name
                "Home/SetUkrainian",                           // URL with parameters
                new { controller = "Home", action = "SetUkrainian" }  // Parameter defaults
            );

            routes.MapRoute(
                "Russian",                                              // Route name
                "Home/SetRussian",                           // URL with parameters
                new { controller = "Home", action = "SetRussian" }  // Parameter defaults
            );

            routes.MapRoute(
                "Agreement",                                              // Route name
                "Home/Agreement",                           // URL with parameters
                new { controller = "Home", action = "Agreement" }  // Parameter defaults
            );


            routes.MapRoute(
                "Contacts",                                              // Route name
                "Home/Contacts",                           // URL with parameters
                new { controller = "Home", action = "Contacts" }  // Parameter defaults
            );


            routes.MapRoute(
                "CityIdSelection",                                              // Route name
                "Home/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );



        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}