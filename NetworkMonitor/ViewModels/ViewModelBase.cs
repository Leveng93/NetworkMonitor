using System.ComponentModel;

namespace NetworkMonitor.ViewModels
{
    /// <summary>
    /// Базовый класс модели-представления.
    /// </summary>
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };    // Событие изменения свойства.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);   // Уведомление об изменении свойства.
        }
    }
}
