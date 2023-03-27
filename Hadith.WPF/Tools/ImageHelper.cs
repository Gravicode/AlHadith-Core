using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Hadith.WPF.Tools
{
    public class ImageHelper
    {
        public static BitmapImage BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage(source);
            bitmap.CacheOption = BitmapCacheOption.OnDemand;
            //bitmap.BeginInit();
            //bitmap.UriSource = source;
            //bitmap.CacheOption = BitmapCacheOption.OnLoad;
            //bitmap.EndInit();
            return bitmap;
        }
    }
}
