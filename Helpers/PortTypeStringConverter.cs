using System;
using System.Globalization;
using System.Windows.Data;

namespace CPRISwitchSimulator
{
    class PortTypeStringConverter : IValueConverter
    {
        private static (string, TopologyModel.PortType)[] PortTypeStringMap = new (string, TopologyModel.PortType)[]
           {
            ("CONNECTOR_PORT" , TopologyModel.PortType.CONNECTOR_PORT),
            ("PROCESSING_PORT" , TopologyModel.PortType.PROCESSING_PORT)
           };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TopologyModel.PortType)
            {
                foreach (var it in PortTypeStringMap)
                {
                    if (it.Item2 == (TopologyModel.PortType)value)
                        return it.Item1;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                foreach (var it in PortTypeStringMap)
                {
                    if (it.Item1 == (string)value)
                        return it.Item2;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
