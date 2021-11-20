using System;
using System.Collections.Generic;
using System.Text;

namespace ImageTools.UI.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// 将文件大小转换成容易查看格式
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ConvertFileSize(double size)
        {
            if (size > 1024 * 1024 * 1024)
            {
                return $"{Math.Round(size / (1024 * 1024 * 1024), 2)}G";
            }
            if (size > 1024 * 1024)
            {
                return $"{Math.Round(size / (1024 * 1024), 1)}M";
            }
            return size > 1024 ? $"{Math.Round(size / 1024, 2)}K" : $"{size}B";
        }
    }
}
