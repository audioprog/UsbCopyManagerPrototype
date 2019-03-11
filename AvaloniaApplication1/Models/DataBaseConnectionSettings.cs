using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaApplication1.Models
{
    public class DataBaseConnectionSettings : ReactiveObject
    {

        private string serverName = "nas-bbh";

        public string ServerName
        {
            get => serverName;
            set => this.RaiseAndSetIfChanged(ref serverName, value);
        }


        private string databaseName = string.Empty;

        public string DatabaseName
        {
            get => databaseName;
            set => this.RaiseAndSetIfChanged(ref databaseName, value);
        }


        private string userName = string.Empty;

        public string UserName
        {
            get => userName;
            set => this.RaiseAndSetIfChanged(ref userName, value);
        }


        private string password = string.Empty;

        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }
    }
}
