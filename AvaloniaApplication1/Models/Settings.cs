using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Serilog;

namespace AvaloniaApplication1.Models
{
    public class Settings : ReactiveObject
    {
        public static Settings LoadSettings()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "UsbCopyManager");
            string settingsFile = Path.Combine(path, "Settings.xml");

            Settings newSettings = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (TextReader reader = new StreamReader(settingsFile))
                {
                    newSettings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Log.Logger.Debug(dirNotFound.ToString());

                newSettings = new Settings();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.ToString());

                newSettings = new Settings();
            }
            return newSettings;
        }

        public ObservableCollection<SettingsAudioPath> Paths { get; set; } = new ObservableCollection<SettingsAudioPath>();

        public DivineServiceSettings DivineService { get; set; } = new DivineServiceSettings()
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


        public DataBaseConnectionSettings DBConnectionSettings { get; set; } = new DataBaseConnectionSettings();


        public bool SaveSettings()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "UsbCopyManager");
            string settingsFile = Path.Combine(path, "Settings.xml");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                TextWriter writer = new StreamWriter(settingsFile);
                serializer.Serialize(writer, this);
                writer.Close();

                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.ToString());
            }

            return false;
        }
    }
}
