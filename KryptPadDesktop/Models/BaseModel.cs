using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadDesktop.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            var handle = PropertyChanged;
            if (handle != null)
            {
                handle(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
