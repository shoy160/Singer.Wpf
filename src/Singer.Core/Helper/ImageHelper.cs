using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Singer.Core.Helper
{
    public class ImageHelper
    {
        /// <summary> 图片 转为 base64编码的文本 </summary>
        /// <param name="imagefilename"></param>
        /// <returns></returns>
        public static string Base64(string imagefilename)
        {
            try
            {
                var bmp = new Bitmap(imagefilename);
                var arr = ImageArray(bmp);
                return Convert.ToBase64String(arr);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static byte[] ImageArray(Bitmap bmp)
        {
            using (bmp)
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, bmp.RawFormat);
                    var arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    return arr;
                }
            }
        }

        public static Bitmap FromBase64(string base64)
        {
            try
            {
                var arr = Convert.FromBase64String(base64);
                using (var ms = new MemoryStream(arr))
                {
                    return new Bitmap(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
