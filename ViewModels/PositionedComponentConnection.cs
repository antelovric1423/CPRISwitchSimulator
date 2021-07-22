using CPRISwitchSimulator.Helpers;
using System.ComponentModel;

namespace CPRISwitchSimulator.ViewModels
{
    public class PositionedComponentConnection : NotifierBase
    {
        protected void _component1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PosX")
                OnPropertyChanged("PosX1");
            else if (e.PropertyName == "PosY")
                OnPropertyChanged("PosY1");
        }
        protected void _component2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PosX")
                OnPropertyChanged("PosX2");
            else if (e.PropertyName == "PosY")
                OnPropertyChanged("PosY2");
        }
        protected PositionedComponent _component1;
        protected PositionedComponent _component2;
        protected double _component1PosCorrection;
        protected double _component2PosCorrection;
        public double PosX1 { get { return _component1.PosX + _component1PosCorrection; } }
        public double PosY1 { get { return _component1.PosY + _component1PosCorrection; } }
        public double PosX2 { get { return _component2.PosX + _component2PosCorrection; } }
        public double PosY2 { get { return _component2.PosY + _component2PosCorrection; } }
    }
}
