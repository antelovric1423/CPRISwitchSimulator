using CPRISwitchSimulator.Helpers;
using System;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public class Link : NotifierBase
        {
            public Link(ConnectorPort port1, ConnectorPort port2)
            {
                if (port1 == null || port2 == null)
                    throw new ArgumentNullException("ERROR: Port name not unique for element!");

                _port1 = port1;
                _port2 = port2;
                LineRate = port1.MaxLineRate > port2.MaxLineRate ? port2.MaxLineRate : port1.MaxLineRate;
                Capacity = new Capacity();
                Capacity.SetLineRate(LineRate);
            }

            public ConnectorPort GetOppositePort(ConnectorPort port)
            {
                if (port != Port1 && port != Port2)
                    throw new ArgumentOutOfRangeException("Port is not part of link!");

                if (port == Port1)
                    return Port2;
                else
                    return Port1;
            }

            private ConnectorPort _port1;
            private ConnectorPort _port2;
            private Capacity _capacity;
            private LineRate _lineRate;
            public ConnectorPort Port1
            {
                get { return _port1; }
                private set
                {
                    _port1 = value;
                    OnPropertyChanged("Port1");
                }
            }
            public ConnectorPort Port2
            {
                get { return _port2; }
                private set
                {
                    _port2 = value;
                    OnPropertyChanged("Port2");
                }
            }
            public LineRate LineRate
            {
                get { return _lineRate; }
                private set
                {
                    _lineRate = value;
                    OnPropertyChanged("LineRate");
                }
            }
            public Capacity Capacity { get { return _capacity; } private set { _capacity = value; } }
        }
    }
}
