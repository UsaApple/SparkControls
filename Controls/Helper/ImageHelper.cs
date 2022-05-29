using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SparkControls.Controls.Helper
{
    internal class ImageHelper
    {
        //为了加快处理,图标做一次缓存
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, Image> _dict = new System.Collections.Concurrent.ConcurrentDictionary<int, Image>();

        /// <summary>
        /// 将透明底色的图片进行置白处理
        /// </summary>
        /// <param name="image">原始图片</param>
        /// <param name="doImage">处理后的图标</param>
        /// <returns>返回成功true,未处理false</returns>
        public static bool DoImageToWhite(Image image, out Image doImage)
        {
            if (image == null || !IsTransparentPalette(image))
            {
                doImage = image;
                return false;
            }

            var hasCode = image.GetHashCode();
            if (_dict.TryGetValue(hasCode, out doImage))
            {
                return true;
            }

            Bitmap oldBitmap = new Bitmap(image);

            int width = image.Width;
            int height = image.Height;
            Bitmap newBitmap = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y;
            Color pixel;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = oldBitmap.GetPixel(x, y);//获取当前坐标的像素值
                    if (pixel.A == 0) continue;


#if false
                    var resultR = 255 - pixel.R;//反红
                    var resultG = 255 - pixel.G;//反绿
                    var resultB = 255 - pixel.B;//反蓝
                    var doColor = Color.FromArgb(resultR, resultG, resultB);
#else
                    Color doColor = Color.Empty;
                    if (pixel == Color.White || (pixel.R > 200 && pixel.G > 200 && pixel.B > 200))
                    {//如果本身是白色,改成透明,或rgb都大于200的
                        doColor = Color.Transparent;
                    }
                    else
                    {
                        doColor = Color.White;
                    }
                    //Trace.WriteLine(pixel);
#endif
                    newBitmap.SetPixel(x, y, doColor);//绘图
                }
            }
            doImage = newBitmap;
            _dict.TryAdd(hasCode, doImage);
            return true;
        }

        /// <summary>
        /// 判断是不是透明的图标
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool IsTransparentPalette(Image image)
        {
            if (Enum.TryParse<System.Drawing.Imaging.ImageFlags>(image.Flags.ToString(), out var flags))
            {
                return flags.HasFlag(System.Drawing.Imaging.ImageFlags.HasAlpha);
            }
            return false;

            //Gif图标的判断
            //if (palette.Flags != 1)
            //    return false;
            //int total_colors = palette.Entries.GetLength(0);
            //for (int i = 0; i < total_colors - 1; i++)
            //{
            //    if (palette.Entries[i].A != 0)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }
    }
}
