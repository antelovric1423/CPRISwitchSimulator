using System;
using System.Globalization;
using System.Windows.Data;

namespace CPRISwitchSimulator
{
    class LineRateStringConverter : IValueConverter
    {
        private static (string, TopologyModel.LineRate)[] LineRateStringMap = new (string, TopologyModel.LineRate)[]
           {
            ("CPRI_2_4G" , TopologyModel.LineRate.CPRI_2_4G),
            ("CPRI_4_9G" , TopologyModel.LineRate.CPRI_4_9G),
            ("CPRI_9_8G" , TopologyModel.LineRate.CPRI_9_8G),
            ("CPRI_10_1G" , TopologyModel.LineRate.CPRI_10_1G),
            ("CPRI_24_3G" , TopologyModel.LineRate.CPRI_24_3G)
           };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TopologyModel.LineRate)
            {
                foreach (var it in LineRateStringMap)
                {
                    if (it.Item2 == (TopologyModel.LineRate)value)
                        return it.Item1;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                foreach (var it in LineRateStringMap)
                {
                    if (it.Item1 == (string)value)
                        return it.Item2;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
