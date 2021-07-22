using CPRISwitchSimulator.ViewModels;
using System.ComponentModel;

namespace CPRISwitchSimulator
{
    public class CellConnectionViewModel : PositionedComponentConnection
    {
        public CellConnectionViewModel(CellMainViewModel cellVM, ElementMainViewModel elementVM, double cellPosCorrection, double elementPosCorrection)
        {
            _component1 = cellVM;
            _component2 = elementVM;
            _component1PosCorrection = cellPosCorrection;
            _component2PosCorrection = elementPosCorrection;

            _component1.PropertyChanged += new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged += new PropertyChangedEventHandler(_component2_PropertyChanged);
        }
        ~CellConnectionViewModel()
        {
            _component1.PropertyChanged -= new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged -= new PropertyChangedEventHandler(_component2_PropertyChanged);
        }
        public CellMainViewModel CellVM { get { return _component1 as CellMainViewModel; } }
        public ElementMainViewModel ElementVM { get { return _component2 as ElementMainViewModel; } }
    }
}
