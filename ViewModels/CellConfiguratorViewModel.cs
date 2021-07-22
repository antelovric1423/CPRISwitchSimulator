using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CPRISwitchSimulator.Helpers;

namespace CPRISwitchSimulator
{
    public class CellConfiguratorViewModel : NotifierBase
    {
        public CellConfiguratorViewModel(TopologyModel.Cell cell)
        {
            _cell = cell;
        }

        private TopologyModel.Cell _cell;
        public string Name
        {
            get { return _cell.Name; }
            set
            {
                _cell.Name = value;
                OnPropertyChanged("Name");
            }
        }
        public Array RatTypes 
        { 
            get { return Enum.GetValues(typeof(TopologyModel.RatType)); }
        }
        public Array Bandwidths
        {
            get { return Enum.GetValues(typeof(TopologyModel.CarrierBandwidth)); }
        }
        public TopologyModel.RatType RatType
        {
            get { return _cell.RatType; }
            set
            {
                _cell.RatType = value;
                OnPropertyChanged("RatType");
            }
        }
        public TopologyModel.CarrierBandwidth Bandwidth
        {
            get { return _cell.Bandwidth; }
            set
            {
                _cell.Bandwidth = value;
                OnPropertyChanged("Bandwidth");
            }
        }
    }
}
