using CPRISwitchSimulator.ViewModels;
using System.ComponentModel;

namespace CPRISwitchSimulator
{
    public class ElementConnectionMainViewModel : PositionedComponentConnection
    {
        public ElementConnectionMainViewModel(ElementMainViewModel element1, ElementMainViewModel element2, double posCorrection)
        {
            _component1 = element1;
            _component2 = element2;
            _component1PosCorrection = posCorrection;
            _component2PosCorrection = posCorrection;

            _component1.PropertyChanged += new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged += new PropertyChangedEventHandler(_component2_PropertyChanged);
        }
        ~ElementConnectionMainViewModel()
        {
            _component1.PropertyChanged -= new PropertyChangedEventHandler(_component1_PropertyChanged);
            _component2.PropertyChanged -= new PropertyChangedEventHandler(_component2_PropertyChanged);
        }

        public ElementMainViewModel Element1
        {
            get { return _component1 as ElementMainViewModel; }
            set
            {
                _component1 = value;
                OnPropertyChanged("Element1");
            }
        }
        public ElementMainViewModel Element2
        {
            get { return _component2 as ElementMainViewModel; }
            set
            {
                _component2 = value;
                OnPropertyChanged("Element2");
            }
        }
    }
}
