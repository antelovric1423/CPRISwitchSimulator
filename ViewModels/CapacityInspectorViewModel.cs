using CPRISwitchSimulator.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPRISwitchSimulator
{
    public class CapacityInspectorViewModel : NotifierBase
    {
        public CapacityInspectorViewModel(TopologyModel.Capacity capacity)
        {
            IndexedCapacityData = new ObservableCollection<Tuple<uint, string>>();

            for (uint i = 0; i < capacity.AxcContainers.Length; i++)
            {
                uint allocationRef = capacity.AxcContainers[i].AllocationRef;
                IndexedCapacityData.Add(new Tuple<uint, string>(i, allocationRef != 0 ? Convert.ToString(allocationRef) : ""));
            }
        }

        public ObservableCollection<Tuple<uint, string>> IndexedCapacityData { get; private set; }
    }
}
