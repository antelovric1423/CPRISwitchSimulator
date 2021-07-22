using CPRISwitchSimulator.Helpers;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public enum PortType
        {
            PROCESSING_PORT,
            CONNECTOR_PORT
        }
        public enum LineRate
        {
            CPRI_2_4G,
            CPRI_4_9G,
            CPRI_9_8G,
            CPRI_10_1G,
            CPRI_24_3G
        }
        public class Port : NotifierBase
        {
            private string _name;
            private PortType _type;
            private LineRate _maxLineRate;
            public Element Parent
            {
                get;
                protected set;
            }
            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
            public PortType Type
            {
                get { return _type; }
                set
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
            public LineRate MaxLineRate
            {
                get { return _maxLineRate; }
                set
                {
                    _maxLineRate = value;
                    OnPropertyChanged("MaxLineRate");
                }
            }
        }
    }
}
