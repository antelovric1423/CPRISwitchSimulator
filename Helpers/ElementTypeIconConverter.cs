using System;
using System.Globalization;
using System.Windows.Data;

namespace CPRISwitchSimulator
{
    class ElementTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TopologyModel.ElementType)
            {
                return GetIconPath((TopologyModel.ElementType)value);
            }
            throw new ArgumentOutOfRangeException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public static string GetIconPath(TopologyModel.ElementType elementType)
        {
            switch (elementType)
            {
                case TopologyModel.ElementType.REC:
                    return "pack://application:,,,/resources/REC.png";
                case TopologyModel.ElementType.RE:
                    return "pack://application:,,,/resources/RE.png";
                case TopologyModel.ElementType.SWITCH:
                    return "pack://application:,,,/resources/switch.png";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
