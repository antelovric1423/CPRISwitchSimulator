using System.ComponentModel;

namespace CPRISwitchSimulator.Helpers
{
    public class NotifierBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
