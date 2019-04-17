using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace WindowsUsbWatcher
{
    class Program

    {
        public enum EventType
        {
            Inserted = 2,
            Removed = 3
        }

        static void Main(string[] args)

        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");

            watcher.EventArrived += (s, e) =>
            {
                string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();

                DriveInfo[] drives = DriveInfo.GetDrives();

                IEnumerable<DriveInfo> infos = from driveInfo in drives where driveInfo.Name.StartsWith(driveName) select driveInfo;

                if (infos.Count() > 0)
                {
                    driveName = infos.First().VolumeLabel + "|" + driveName;
                }

                EventType eventType = (EventType)(Convert.ToInt16(e.NewEvent.Properties["EventType"].Value));

                string eventName = Enum.GetName(typeof(EventType), eventType);

                Console.WriteLine("{0}|{1}", eventName, driveName);
            };

            watcher.Query = query;
            watcher.Start();

            for (; ; );

        }
    }
}
