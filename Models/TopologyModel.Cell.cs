using CPRISwitchSimulator.Helpers;
using System;

namespace CPRISwitchSimulator
{
    public partial class TopologyModel
    {
        public enum CellState
        {
            DISABLED,
            ENABLED
        }
        public enum RatType
        {
            NOT_SET,
            LTE,
            NR
        };
        public enum CarrierBandwidth
        {
            NOT_SET,
            MHZ_5,
            MHZ_10,
            MHZ_15,
            MHZ_20,
            MHZ_50,
            MHZ_100
        }
        public class Cell : NotifierBase
        {
            public Cell(uint id)
            {
                Id = id;
                Name = null;
                RatType = RatType.NOT_SET;
                Bandwidth = CarrierBandwidth.NOT_SET;
                AttachedElement = null;
                State = CellState.DISABLED;
                ErrorMessage = "Cell not attached to RE";
            }
            public AxcContainerFormat GetAxcContainerFormat()
            {
                return AxcContainerFormat.FORMAT_20BIT;
            }
            public uint GetReqAxcContainerCount()
            {
                switch (RatType)
                {
                    case RatType.LTE:
                        return GetLTEAxCRequirement();
                    case RatType.NR:
                        return GetNRAxCRequirement();
                    default:
                        throw new ArgumentException("Cell parameters not set or invalid.");
                }
            }
            private uint GetNRAxCRequirement()
            {
                switch (Bandwidth)
                {
                    case CarrierBandwidth.MHZ_5:
                        return 4;
                    case CarrierBandwidth.MHZ_10:
                        return 8;
                    case CarrierBandwidth.MHZ_15:
                        return 12;
                    case CarrierBandwidth.MHZ_20:
                        return 12;
                    case CarrierBandwidth.MHZ_50:
                        return 30;
                    case CarrierBandwidth.MHZ_100:
                        return 60;
                    default:
                        throw new ArgumentException("Cell parameters not set or invalid.");
                }
            }
            private uint GetLTEAxCRequirement()
            {
                switch (Bandwidth)
                {
                    case CarrierBandwidth.MHZ_5:
                        return 2;
                    case CarrierBandwidth.MHZ_10:
                        return 3;
                    case CarrierBandwidth.MHZ_15:
                        return 4;
                    case CarrierBandwidth.MHZ_20:
                        return 5;
                    default:
                        throw new ArgumentException("Cell parameters not set or invalid.");
                }
            }

            private uint _id;
            private string _name;
            private RatType _ratType;
            private CarrierBandwidth _bandwidth;
            private Element _attachedElement;
            private CellState _state;
            private string _errorMessage;
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
            public RatType RatType
            {
                get { return _ratType; }
                set
                {
                    if (value == RatType.LTE && Bandwidth > CarrierBandwidth.MHZ_20)
                        throw new ArgumentException("LTE cell does not support more than 20MHz.");

                    _ratType = value;
                    OnPropertyChanged("RatType");
                }
            }
            public CarrierBandwidth Bandwidth
            {
                get { return _bandwidth; }
                set
                {
                    if (value > CarrierBandwidth.MHZ_20 && RatType == RatType.LTE)
                        throw new ArgumentException("LTE cell does not support more than 20MHz.");

                    _bandwidth = value;
                    OnPropertyChanged("Bandwidth");
                }
            }
            public Element AttachedElement
            {
                get { return _attachedElement; }
                set
                {
                    if (value != null)
                    {
                        if (value.Type != ElementType.RE)
                            throw new ArgumentException("Cell can only be attached to RE.");
                        if (RatType == RatType.NOT_SET || Bandwidth == CarrierBandwidth.NOT_SET)
                            throw new ArgumentException("Cell can only be attached to element when RAT type and Bandwidth are set.");
                    }
                    else
                    {
                        State = CellState.DISABLED;
                        ErrorMessage = "Cell not attached to RE";
                    }

                    _attachedElement = value;
                    OnPropertyChanged("AttachedElement");
                }
            }
            public CellState State
            {
                get { return _state; }
                set
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
            public string ErrorMessage
            {
                get { return _errorMessage; }
                set
                {
                    _errorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }
    }
}
