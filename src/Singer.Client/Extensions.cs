using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Singer.Client
{
    public static class Extensions
    {
        /// <summary> 获取语言包文字 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string L(this string key)
        {
            return Application.Current.FindResource(key)?.ToString();
        }

        public static BitmapImage BitmapImageFromBase64(this string base64)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                var arr = Convert.FromBase64String(base64);
                using (var ms = new MemoryStream(arr))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BitmapImage BitmapToBitmapImage(this Image bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        /// <summary> 通过类型获取子集控件 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> FindControls<T>(this DependencyObject obj) where T : FrameworkElement
        {
            var childList = new List<T>();
            var type = typeof(T);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var ele = child as T;
                if (ele != null && ele.GetType() == type)
                {
                    childList.Add(ele);
                }
                childList.AddRange(FindControls<T>(child));
            }
            return childList;
        }

        /// <summary> 通过名称获取子集控件 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<T> FindControls<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            var childList = new List<T>();

            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var ele = child as T;
                if (ele != null && (ele.Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add(ele);
                }
                childList.AddRange(FindControls<T>(child, name));
            }
            return childList;
        }

        /// <summary> 通过名称获取子集控件 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T FindControl<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var ele = child as T;
                if (ele != null && (ele.Name == name || string.IsNullOrEmpty(name)))
                {
                    return ele;
                }
                var grandChild = FindControl<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        /// <summary> 通过名称获取子集控件 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FindControl<T>(this DependencyObject obj) where T : FrameworkElement
        {
            var type = typeof(T);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var ele = child as T;
                if (ele != null && ele.GetType() == type)
                {
                    return ele;
                }
                var grandChild = FindControl<T>(child);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        public static T FindParent<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                var ele = parent as T;
                if (ele != null && (ele.Name == name || string.IsNullOrEmpty(name)))
                {
                    return ele;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static T FindParent<T>(this DependencyObject obj) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(obj);
            var type = typeof(T);
            while (parent != null)
            {
                var ele = parent as T;
                if (ele != null && ele.GetType() == type)
                {
                    return ele;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
