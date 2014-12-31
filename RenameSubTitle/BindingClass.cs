using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenameSubTitle
{
    public class BindingClass : PropertyChangedBase
    {
        private bool _showAgain;
        public bool ShowAgain
        {
            get { return _showAgain; }
            set
            {
                _showAgain = !value;
                NotifyPropertyChanged("ShowAgain");
            }
        }
    }
}
