using System.Collections.Generic;
using System.Drawing;
using Zamov.Models;

namespace Zamov.Helpers
{
    public class AdvertHelper
    {
        public static Dictionary<BannerPosition, Size> BannerDimentions = new Dictionary<BannerPosition, Size>();

        
        static AdvertHelper()
        {
            BannerDimentions.Add(BannerPosition.Top,new Size(468,60));
            BannerDimentions.Add(BannerPosition.BottomCenter, new Size(468, 60));
            BannerDimentions.Add(BannerPosition.BottomLeft, new Size(234, 60));
            BannerDimentions.Add(BannerPosition.BottomRight, new Size(234, 60));
        }
    }
}
