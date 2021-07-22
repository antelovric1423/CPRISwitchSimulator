using CPRISwitchSimulator.ViewModels;

namespace CPRISwitchSimulator
{
    public class CellMainViewModel : PositionedComponent
    {
        public CellMainViewModel(TopologyModel.Cell cell, double posX, double posY)
        {
            Cell = cell;
            PosX = posX;
            PosY = posY;
        }

        private TopologyModel.Cell _cell;
        public TopologyModel.Cell Cell
        {
            get { return _cell; }
            set
            {
                _cell = value;
                OnPropertyChanged("Cell");
            }
        }
    }
}
