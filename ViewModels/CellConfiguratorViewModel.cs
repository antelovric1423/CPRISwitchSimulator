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
            Cell = cell;
        }

        public string Name
        {
            get { return Cell.Name; }
            set
            {
                Cell.Name = value;
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
            get { return Cell.RatType; }
            set
            {
                Cell.RatType = value;
                OnPropertyChanged("RatType");
            }
        }
        public TopologyModel.CarrierBandwidth Bandwidth
        {
            get { return Cell.Bandwidth; }
            set
            {
                Cell.Bandwidth = value;
                OnPropertyChanged("Bandwidth");
            }
        }
        public TopologyModel.Cell Cell { get; private set; }
    }
}
