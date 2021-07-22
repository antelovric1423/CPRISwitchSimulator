namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public class ConnectorPort : Port
        {
            public ConnectorPort(Element parent, string name, LineRate maxLineRate)
            {
                Parent = parent;
                Name = name;
                MaxLineRate = maxLineRate;
                Link = null;
                Type = PortType.CONNECTOR_PORT;
            }
            public ConnectorPort GetTargetPort()
            {
                if (Link == null)
                    return null;

                if (Link.Port1 == this)
                    return Link.Port2;

                return Link.Port1;
            }

            private Link _link;
            public Link Link
            {
                get { return _link; }
                set
                {
                    _link = value;
                    OnPropertyChanged("Link");
                }
            }
        }
    }
}
