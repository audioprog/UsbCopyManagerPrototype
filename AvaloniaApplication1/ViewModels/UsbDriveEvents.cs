using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.ViewModels
{
    public enum VolumeEventType
    {
        Inserted,
        Removed
    }

    public delegate void UsbVolumeChangeEventDelegate(object sender, UsbVolumeChangeEventArgs e);


    public class UsbVolumeChangeEventArgs : EventArgs
    {
        public UsbVolumeChangeEventArgs(VolumeEventType eventType, string name, string driveLetter)
        {
            EventType = eventType;
            Name = name;
            DriveLetter = driveLetter;
        }

        public VolumeEventType EventType { get; }

        public string Name { get; }

        public string DriveLetter { get; }
    }


    public class UsbDriveEvents : IDisposable
    {
        ~UsbDriveEvents()
        {
            Dispose();
        }

        private string cache = string.Empty;

        private Process process;

        public UsbVolumeChangeEventDelegate DriveEvent;

        public UsbDriveEvents()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                process = new Process();
                process.StartInfo.FileName = "udevadm";
                process.StartInfo.Arguments = "monitor --udev --property";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, data) => {
                    // start mit "UDEV  ["
                    cache += data.Data;
                    if (data.Data.StartsWith("ID_FS_TYPE="))
                    {

                    }//DEVNAME=
                    Console.WriteLine(data.Data);
                };
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (sender, data) => {
                    Console.WriteLine(data.Data);
                };
                process.Start();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = @"..\..\..\..\WindowsUsbWatcher\bin\Debug\netcoreapp2.1\WindowsUsbWatcher.dll";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, data) => {
                    string[] splitted = data.Data.Split('|');
                    if (splitted.Length > 1)
                    {
                        switch (splitted[0])
                        {
                            case "Inserted":
                                DriveEvent?.Invoke(this, new UsbVolumeChangeEventArgs(VolumeEventType.Inserted, splitted[1], splitted[splitted.Length - 1]));
                                break;
                            case "Removed":
                                DriveEvent?.Invoke(this, new UsbVolumeChangeEventArgs(VolumeEventType.Removed, "", splitted[1]));
                                break;
                        }
                    }
                    Console.WriteLine(data.Data);
                };
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (sender, data) => {
                    Console.WriteLine(data.Data);
                };
                process.Start();
            }
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
