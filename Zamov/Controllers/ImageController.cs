using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Zamov.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace Zamov.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/

        public void ShowLogo(int id)
        {
            if (id == -1) return;
            using (ZamovStorage context = new ZamovStorage())
            {
                Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
                Response.ContentType = dealer.LogoType;
                MemoryStream stream = new MemoryStream(dealer.LogoImage);
                Bitmap image = new Bitmap(stream);
                int width = image.Width;
                int height = image.Height;
                if (width > 200)
                {
                    width = 200;
                    height = (200 * image.Height) / image.Width;
                }
                if (height > 100)
                {
                    width = (100 * width) / height;
                    height = 100;
                }
                Bitmap thumbnailImage = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(thumbnailImage);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, width, height);
                MemoryStream imageStream = new MemoryStream();
                thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageContent = new Byte[imageStream.Length];
                imageStream.Position = 0;
                imageStream.Read(imageContent, 0, (int)imageStream.Length);
                Response.ContentType = "image/jpeg";
                Response.BinaryWrite(imageContent);
            }
        }

        public void DealerImage(int id, int maxDimension)
        {
            string dealerImageCacheKey = "DealerImage_" + id + "_" + maxDimension;
            if (HttpContext.Cache[dealerImageCacheKey] == null)
            {
                Bitmap image = null;
                using (ZamovStorage context = new ZamovStorage())
                {
                    Dealer dealer = context.Dealers.Select(d => d).Where(d => d.Id == id).First();
                    Response.ContentType = dealer.LogoType;
                    MemoryStream stream = new MemoryStream(dealer.LogoImage);
                    image = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                }

                MemoryStream imageStream = ScaleImage(image, maxDimension);
                byte[] imageContent = new Byte[imageStream.Length];
                imageStream.Read(imageContent, 0, (int)imageStream.Length);
                HttpContext.Cache.Add(dealerImageCacheKey, imageContent, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                imageStream.Close();
            }
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite((byte[])HttpContext.Cache[dealerImageCacheKey]);
        }

        public void ShowImage(string imageType, byte[] image)
        {
            Response.ContentType = imageType;
            Response.BinaryWrite(image);
        }

        public void ProductImage(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                ProductImage image = context.ProductImages.Select(pi => pi).Where(pi => pi.Id == id).First();
                Response.ContentType = image.ImageType;
                Response.BinaryWrite(image.Image);
            }
        }

        public MemoryStream ScaleImage(Bitmap image, int maxDimension)
        {
            int width;
            int height;
            if (image.Width > image.Height)
            {
                width = maxDimension;
                height = (maxDimension * image.Height) / image.Width;

            }
            else if (image.Height > image.Width)
            {
                height = maxDimension;
                width = (maxDimension * image.Width) / image.Height;
            }
            else
                width = height = maxDimension;


            Bitmap thumbnailImage = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(thumbnailImage);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, thumbnailImage.Width, thumbnailImage.Height);
            MemoryStream imageStream = new MemoryStream();
            thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            imageStream.Position = 0;
            return imageStream;
        }

        public void ProductImageScaled(int id, int maxDimension)
        {
            Bitmap image = null;
            using (ZamovStorage context = new ZamovStorage())
            {
                ProductImage productImage = context.ProductImages.Select(pi => pi).Where(pi => pi.Product.Id == id).FirstOrDefault();
                if (productImage == null)
                    return;
                MemoryStream stream = new MemoryStream(productImage.Image);
                image = new Bitmap(stream);
                stream.Close();
                stream.Dispose();
            }

            MemoryStream imageStream = ScaleImage(image, maxDimension);
            byte[] imageContent = new Byte[imageStream.Length];
            imageStream.Read(imageContent, 0, (int)imageStream.Length);
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite(imageContent);
            imageStream.Close();
        }

        public void ProductImageDefault(int id, int maxDimension)
        {
            byte[] imageContent = null;
            string productImageCacheKey = "ProductImage_" + id + "_" + maxDimension;
            if (HttpContext.Cache[productImageCacheKey] == null)
            {
                Bitmap image = null;
                using (ZamovStorage context = new ZamovStorage())
                {
                    ProductImage productImage = context.ProductImages.Select(pi => pi).Where(pi => pi.Product.Id == id).First();
                    MemoryStream stream = new MemoryStream(productImage.Image);
                    image = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                }

                MemoryStream imageStream = ScaleImage(image, maxDimension);
                imageContent = new Byte[imageStream.Length];
                imageStream.Read(imageContent, 0, (int)imageStream.Length);
                HttpContext.Cache.Add(productImageCacheKey, imageContent, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                imageStream.Close();
            }
            Response.ContentType = "image/jpeg";
            Response.BinaryWrite((byte[])HttpContext.Cache[productImageCacheKey]);
        }

        public void CategoryImage(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                CategoryImage image = context.CategoryImages.Select(ci => ci).Where(ci => ci.Id == id).First();
                Response.ContentType = image.ImageType;
                Response.BinaryWrite(image.Image);
            }
        }

        public void CategoryImageByCategoryId(int id)
        {
            using (ZamovStorage context = new ZamovStorage())
            {
                CategoryImage image = context.CategoryImages.Select(ci => ci).Where(ci => ci.Category.Id == id).FirstOrDefault();
                if (image != null)
                {
                    Response.ContentType = image.ImageType;
                    Response.BinaryWrite(image.Image);
                }
            }
        }

    }
}
