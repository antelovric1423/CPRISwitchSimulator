using System;
using System.Globalization;
using System.Windows.Data;

namespace CPRISwitchSimulator
{
    class CarrierBandwidthStringConverter : IValueConverter
    {
        private static (string, TopologyModel.CarrierBandwidth)[] CarrierBandwidthTypeStringMap = new (string, TopologyModel.CarrierBandwidth)[]
           {
            ("NOT_SET" , TopologyModel.CarrierBandwidth.NOT_SET),
            ("5 MHz" , TopologyModel.CarrierBandwidth.MHZ_5),
            ("10 MHz" , TopologyModel.CarrierBandwidth.MHZ_10),
            ("15 MHz" , TopologyModel.CarrierBandwidth.MHZ_15),
            ("20 MHz" , TopologyModel.CarrierBandwidth.MHZ_20),
            ("50 MHz" , TopologyModel.CarrierBandwidth.MHZ_50),
            ("100 MHz" , TopologyModel.CarrierBandwidth.MHZ_100)
           };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueString = Enum.GetName(value.GetType(), value);

            foreach (var it in CarrierBandwidthTypeStringMap)
            {
                if (Enum.GetName(typeof(TopologyModel.CarrierBandwidth), it.Item2) == valueString)
                    return it.Item1;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                foreach (var it in CarrierBandwidthTypeStringMap)
                {
                    if (it.Item1 == (string)value)
                        return it.Item2;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
