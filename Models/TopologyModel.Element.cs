using CPRISwitchSimulator.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public enum ElementType
        {
            NOT_SET,
            REC,
            RE,
            SWITCH
        }
        public class Element : NotifierBase
        {
            public Element(uint id, ElementType type)
            {
                Id = id;
                Type = type;
                Name = null;
                Ports = new ObservableCollection<Port>();
            }
            public void AddPort(PortType portType, string name, LineRate maxLineRate)
            {
                Port port;

                foreach (var it in Ports)
                {
                    if (it.Name == name)
                        throw new InvalidOperationException("ERROR: Port name not unique for element!");
                }

                switch (portType)
                {
                    case PortType.PROCESSING_PORT:
                        port = new ProcessingPort(this, name, maxLineRate);
                        break;
                    case PortType.CONNECTOR_PORT:
                        port = new ConnectorPort(this, name, maxLineRate);
                        break;
                    default:
                        throw new InvalidOperationException("ERROR: Invalid port type!");
                }

                Ports.Add(port);
                OnPropertyChanged("Ports");
            }
            public void RemovePort(Port port)
            {
                _ = Ports.Remove(port);
                OnPropertyChanged("Ports");
            }
            public List<Link> GetLinksTowardElement(Element element)
            {
                List<Link> links = new List<Link>();

                foreach (var it in Ports)
                {
                    if (!(it is ConnectorPort))
                        continue;

                    ConnectorPort port = (ConnectorPort)it;

                    if (port.Link == null)
                        continue;

                    ConnectorPort targetPort = port.GetTargetPort();
                    if (targetPort.Parent == element)
                        links.Add(port.Link);
                }

                return links;
            }
            public bool HasLinks()
            {
                foreach (var it in Ports)
                {
                    if (!(it is ConnectorPort))
                        continue;

                    ConnectorPort port = (ConnectorPort)it;

                    if (port.Link == null)
                        continue;

                    return true;
                }

                return false;
            }
            public List<ConnectorPort> GetUnusedConnectorPorts()
            {
                List<ConnectorPort> ports = new List<ConnectorPort>();

                foreach (var it in Ports)
                {
                    if (!(it is ConnectorPort))
                        continue;

                    ConnectorPort port = it as ConnectorPort;

                    if (port.Link == null)
                        ports.Add(port);
                }

                return ports;
            }
            
            private uint _id;
            private string _name;
            private ElementType _type;
            private ObservableCollection<Port> _ports;
            public uint Id
            {
                get { return _id; }
                private set
                {
                    _id = value;
                    OnPropertyChanged("Key");
                }
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
            public ElementType Type
            {
                get { return _type; }
                set
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
            public ObservableCollection<Port> Ports
            {
                get { return _ports; }
                set
                {
                    _ports = value;
                    OnPropertyChanged("Ports");
                }
            }
        }
    }
}
