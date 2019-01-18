using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Gymdata.Accreditation.Util;

namespace Gymdata.Accreditation.Converter
{
    public class HexToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hexStr = value as string;
            return ColorUtils.HexStringToNullableColor(hexStr);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;
            return ColorUtils.ColorToHexString(color);
        }
    }

}
