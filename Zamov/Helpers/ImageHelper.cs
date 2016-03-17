using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Zamov.Helpers
{
    public static class ImageHelper
    {
        public static string LoadImageFromURL(string url, string name)
        {
            string returnUrl = "/Content/img/noImage.png";

            var ar = url.Split('.');
            string ext = ar[ar.Length - 1];
            if (!String.IsNullOrEmpty(ext))
            {
                Uri uri = new Uri(url);
                try
                {
                    //create a stream using a http web request
                    System.IO.Stream stream = System.Net.WebRequest.Create(uri).GetResponse().GetResponseStream();

                    if (stream != null)
                    {
                        //create an image object from stream
                        Image img = Image.FromStream(stream);

                        returnUrl = String.Format(@"/Content/ProductImages/{0}.{1}", name, ext);
                        img.Save(HttpContext.Current.Server.MapPath(returnUrl));

                        stream.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
            return returnUrl;
        }
    }
}