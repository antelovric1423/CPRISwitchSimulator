using CPRISwitchSimulator.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPRISwitchSimulator
{
    public class ElementConfiguratorViewModel : NotifierBase
    {
        public ElementConfiguratorViewModel(TopologyModel.Element element)
        {
            _element = element;
            LineRates = (TopologyModel.LineRate[])Enum.GetValues(typeof(TopologyModel.LineRate));
            PortTypes = TopologyModel.GetPortTypesPerElementType(element.Type);
        }
        public void AddPort(string name, TopologyModel.PortType type, TopologyModel.LineRate lineRate)
        {
            TopologyModel.AddPort(_element, name, type, lineRate);
            OnPropertyChanged("Ports");
        }
        public void RemovePort(TopologyModel.Port port)
        {
            TopologyModel.RemovePort(port);
            OnPropertyChanged("Ports");
        }

        private TopologyModel.Element _element;
        public string Name
        {
            get { return _element.Name; }
            set
            {
                _element.Name = value;
                OnPropertyChanged("Name");
            }
        }
        public ObservableCollection<TopologyModel.Port> Ports
        {
            get { return _element.Ports; }
        }
        public List<TopologyModel.PortType> PortTypes { get; private set; }
        public TopologyModel.LineRate[] LineRates { get; private set; }
        public TopologyModel.Element Element { get { return _element; } }
    }
}
