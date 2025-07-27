using System.ComponentModel;

namespace CommonInterfacesBronze
{
    public interface IBaseViewModel : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        public string Title { get; }
        public bool IsDisposable { get; }
        public bool IsBusy { get; }
        public bool IsNotBusy { get; }
        public string BusyPrompt { get; }
        public bool Initilized { get; }
    }

}
