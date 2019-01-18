using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Gymdata.Accreditation.Util;

namespace Gymdata.Accreditation.Converter
{
    public class ColorToHexConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;
            return ColorUtils.ColorToHexString(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexStr = value as string;
            return ColorUtils.HexStringToNullableColor(hexStr);
        }
    }

}
