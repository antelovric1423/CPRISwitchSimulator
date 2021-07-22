using CPRISwitchSimulator.ViewModels;

namespace CPRISwitchSimulator
{
    public class CloudViewModel : PositionedComponent
    {
        public CloudViewModel(double posX, double posY)
        {
            PosX = posX;
            PosY = posY;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
