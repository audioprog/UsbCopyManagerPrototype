using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApplication1.ViewModels
{
    public class ViewMainHeader : ReactiveObject
    {
        private DateTime selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedDate, value);
                UpdateDateString();
            }
        }

        private string selectedDateString = DateTime.Now.ToString("dd.MM.yyyy");

        public string SelectedDateString
        {
            get => selectedDateString;
            private set => this.RaiseAndSetIfChanged(ref selectedDateString, value);
        }


        public string[] DayTimes { get; } = new string[] { "Morgen", "Nachmittag", "Abend", "Teil 1", "Teil 2", "Teil 3" };


        private string dayTime = "Morgen";

        public string DayTime
        {
            get => dayTime;
            set
            {
                this.RaiseAndSetIfChanged(ref dayTime, value);
                UpdateDateString();
            }
        }


        private void UpdateDateString()
        {
            SelectedDateString = selectedDate.ToString("dd.MM.yyyy") + " " + dayTime;
        }
    }
}
