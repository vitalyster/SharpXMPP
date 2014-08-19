using SharpXMPP.WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SharpXMPP.WPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected XMPPContext db = new XMPPContext();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
