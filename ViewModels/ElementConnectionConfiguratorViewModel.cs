using CPRISwitchSimulator.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPRISwitchSimulator
{
    public class ElementConnectionConfiguratorViewModel : NotifierBase
    {
        public ElementConnectionConfiguratorViewModel(ElementConnectionMainViewModel connectionVM)
        {
            _element1 = connectionVM.Element1.Element;
            _element2 = connectionVM.Element2.Element;
            Links = new ObservableCollection<TopologyModel.Link>(_element1.GetLinksTowardElement(_element2));
            Element1AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element1.GetUnusedConnectorPorts());
            Element2AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element2.GetUnusedConnectorPorts());
        }
        public void AddLink(TopologyModel.ConnectorPort port1, TopologyModel.ConnectorPort port2)
        {
            if (port1 == null || port2 == null)
                throw new ArgumentNullException("Ports are null");

            TopologyModel.CreateLink(port1, port2);

            Links = new ObservableCollection<TopologyModel.Link>(_element1.GetLinksTowardElement(_element2));
            OnPropertyChanged("Links");
            Element1AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element1.GetUnusedConnectorPorts());
            OnPropertyChanged("Element1AvailablePorts");
            Element2AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element2.GetUnusedConnectorPorts());
            OnPropertyChanged("Element2AvailablePorts");
        }
        public void RemoveLink(TopologyModel.Link link)
        {
            TopologyModel.RemoveLink(link);

            Links = new ObservableCollection<TopologyModel.Link>(_element1.GetLinksTowardElement(_element2));
            OnPropertyChanged("Links");
            Element1AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element1.GetUnusedConnectorPorts());
            OnPropertyChanged("Element1AvailablePorts");
            Element2AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element2.GetUnusedConnectorPorts());
            OnPropertyChanged("Element2AvailablePorts");
        }
        public void RemoveAllLinks()
        {
            TopologyModel.RemoveLinks(new List<TopologyModel.Link>(Links));

            Links = new ObservableCollection<TopologyModel.Link>(_element1.GetLinksTowardElement(_element2));
            OnPropertyChanged("Links");
            Element1AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element1.GetUnusedConnectorPorts());
            OnPropertyChanged("Element1AvailablePorts");
            Element2AvailablePorts = new ObservableCollection<TopologyModel.ConnectorPort>(_element2.GetUnusedConnectorPorts());
            OnPropertyChanged("Element2AvailablePorts");
        }

        private TopologyModel.Element _element1;
        private TopologyModel.Element _element2;
        public ObservableCollection<TopologyModel.ConnectorPort> Element1AvailablePorts { get; private set; }
        public ObservableCollection<TopologyModel.ConnectorPort> Element2AvailablePorts { get; private set; }
        public TopologyModel.Element Element1 { get { return _element1; } }
        public TopologyModel.Element Element2 { get { return _element2; } }
        public ObservableCollection<TopologyModel.Link> Links { get; private set; }
    }
}
