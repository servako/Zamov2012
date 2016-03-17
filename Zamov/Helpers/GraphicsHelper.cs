using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Zamov.Models;

namespace Zamov.Helpers
{
    public static class GraphicsHelper
    {
        public static string GetImage(this HtmlHelper helper, string fileName, string alt)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            var sb = new StringBuilder();
            string formatString = "<img src=\"{0}\" alt=\"{1}\" />";
            string path = Path.Combine("/Content/Banners/", fileName);
            sb.AppendFormat(formatString, path, alt);
            return sb.ToString();
        }
        
        public static string RegisterFlashScript(this HtmlHelper helper, string fileName, string containerName, BannerPosition bannerPosition)
        {

            Size size = AdvertHelper.BannerDimentions[bannerPosition];

            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            var sb = new StringBuilder();
            string formatString ="<script type=\"text/javascript\">$(function() {swfobject = new SWFObject(\"{0}\", \"b1\", \"235\", \"360\", \"9.0.0\");swfobject.write(\"b1\");});</script>";
            string path = Path.Combine("/Content/Banners/", fileName);
            //sb.AppendFormat(formatString, path);

            sb.Append("<script type=\"text/javascript\">$(function() {swfobject = new SWFObject(\"" + path + "\", \"" + containerName + "\", \"" + size.Width + "\", \"" + size.Height + "\", \"9.0.0\"); swfobject.addParam(\"wmode\", \"transparent\");   swfobject.write(\"" + containerName + "\");});</script>");
            sb.Append("<div id=\"" + containerName + "\"></div>");
            return sb.ToString();
        }


    }
}
