using CPRISwitchSimulator.ViewModels;
using System.ComponentModel;

namespace CPRISwitchSimulator
{
    public class CloudConnectionViewModel : PositionedComponentConnection
    {
        public CloudConnectionViewModel(CloudViewModel cloudVM, ElementMainViewModel elementVM, double cloudPosCorrection, double elementPosCorrection)
        {
            _component1 = cloudVM;
            _component2 = elementVM;
            _component1PosCorrection = cloudPosCorrection;
            _component2PosCorrection = elementPosCorrection;

            _component1.PropertyChanged += new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged += new PropertyChangedEventHandler(_component2_PropertyChanged);
        }
        ~CloudConnectionViewModel()
        {
            _component1.PropertyChanged -= new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged -= new PropertyChangedEventHandler(_component2_PropertyChanged);
        }

        public CloudViewModel CloudVM { get { return _component1 as CloudViewModel; } }
        public ElementMainViewModel ElementVM { get { return _component2 as ElementMainViewModel; } }
    }
}
