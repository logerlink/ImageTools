using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ImageTools.UI.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public virtual void Dispose()
        {

        }
    }
}
