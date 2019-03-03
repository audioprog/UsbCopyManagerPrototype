using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApplication1.Models
{
    public class SettingsAudioPath : ReactiveObject
    {
        private string path;

        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }
    }
}
