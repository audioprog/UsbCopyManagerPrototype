using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AvaloniaApplication1.Models
{
    public class DivineServiceSettings : ReactiveObject
    {
        public ObservableCollection<string> Fields = new ObservableCollection<string>();

        public ObservableCollection<int> Widths = new ObservableCollection<int>();

        public string TableName = string.Empty;
    }
}
