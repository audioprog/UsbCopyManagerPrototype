using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AvaloniaApplication1.Models
{
    public class Settings : ReactiveObject
    {
        public ObservableCollection<SettingsAudioPath> Paths { get; } = new ObservableCollection<SettingsAudioPath>();

        public DivineServiceSettings DivineService { get; } = new DivineServiceSettings()
        {
            TableName = "GottesdienstId",
            Fields = new ObservableCollection<string>()
            {
                "Id", "Datum", "Zeit", "Beschreibung"
            },
            Widths = new ObservableCollection<int>()
            {
                0, 30, 100, 100
            }
        };
    }
}
