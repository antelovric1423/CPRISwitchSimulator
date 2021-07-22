using CPRISwitchSimulator.ViewModels;

namespace CPRISwitchSimulator
{
    public class ElementMainViewModel : PositionedComponent
    {
        public ElementMainViewModel(TopologyModel.Element element, double posX, double posY)
        {
            Element = element;
            PosX = posX;
            PosY = posY;
        }

        private TopologyModel.Element _element;
        public TopologyModel.Element Element
        {
            get { return _element; }
            set
            {
                _element = value;
                OnPropertyChanged("Element");
            }
        }
    }
}
