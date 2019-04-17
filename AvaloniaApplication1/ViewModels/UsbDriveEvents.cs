using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using AvaloniaApplication1.Extensions;

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
        public UsbVolumeChangeEventArgs()
        {}

        public UsbVolumeChangeEventArgs(VolumeEventType eventType, string name, string driveLetter)
        {
            EventType = eventType;
            Name = name;
            DriveLetter = driveLetter;
        }

        public VolumeEventType EventType { get; set; }

        public string Name { get; set; }

        public string DriveLetter { get; set; }
    }


    public class UsbDriveEvents : IDisposable
    {
        ~UsbDriveEvents()
        {
            Dispose();
        }

        private UsbVolumeChangeEventArgs newEvent = null;

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
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, data) => {
                    if (data?.Data == null)
                    {
                        return;
                    }

                    if (data.Data.StartsWith("UDEV  ["))
                    {
                        newEvent = new UsbVolumeChangeEventArgs();
                        string type = data.Data.Section(']', 1, 1).TrimStart().Section(' ', 0, 0);
                        if (type == "add")
                        {
                            newEvent.EventType = VolumeEventType.Inserted;
                        }
                        else if (type == "remove")
                        {
                            newEvent.EventType = VolumeEventType.Removed;
                        }
                    }
                    else if (newEvent != null)
                    {
                        if (data.Data.StartsWith("ID_FS_TYPE="))
                        {
                            newEvent.DriveLetter = data.Data.Substring(11);
                        }
                        else if (data.Data.StartsWith("DEVNAME="))
                        {
                            newEvent.Name = data.Data.Substring(8);
                        }

                        if (newEvent.DriveLetter != null && newEvent.Name != null)
                        {
                            DriveEvent?.Invoke(this, newEvent);
                            newEvent = null;
                        }
                    }
                    
                    //Console.WriteLine(data.Data);
                };
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (sender, data) => {
                    Console.WriteLine(data.Data);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
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
