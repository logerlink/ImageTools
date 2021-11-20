
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ImageTools.UI.Helper
{
    class ConverterHelper
    {
    }
    /// <summary>
    /// true 显示
    /// true 返回 Visible
    /// </summary>
    public class BoolTrueVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultValue = Visibility.Collapsed;
            try
            {
                var str = value?.ToString();
                // true 显示  false 隐藏
                bool.TryParse(parameter?.ToString(), out bool isTrue);
                bool.TryParse(str?.ToString(), out bool strIsTrue);
                if (string.IsNullOrWhiteSpace(str) || !strIsTrue)
                {
                    return isTrue ? defaultValue : Visibility.Visible;
                }
                else
                {
                    return !isTrue ? defaultValue : Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
