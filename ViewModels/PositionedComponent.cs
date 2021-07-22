using CPRISwitchSimulator.Helpers;

namespace CPRISwitchSimulator.ViewModels
{
    public class PositionedComponent : NotifierBase
    {
        private double _posX;
        private double _posY;
        public double PosX
        {
            get { return _posX; }
            set
            {
                _posX = value;
                OnPropertyChanged("PosX");
            }
        }
        public double PosY
        {
            get { return _posY; }
            set
            {
                _posY = value;
                OnPropertyChanged("PosY");
            }
        }
    }
}
