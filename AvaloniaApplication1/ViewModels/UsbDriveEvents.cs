using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

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

        private Timer timer = new Timer();

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
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, data) => {
                    // start mit "UDEV  ["
                    if (data?.Data == null)
                    {
                        return;
                    }
                    
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

            timer.Interval = 100;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            return;
            if (process.StandardOutput.Peek() > 0)
            {
                System.Text.StringBuilder data = new System.Text.StringBuilder();
                try
                {
                    while (process.StandardOutput.Peek() > 0)
                    {
                        data.Append((char)process.StandardOutput.Read());
                    }
                }
                finally
                {
                    cache += data;
                }
                Console.WriteLine(data);
            }
            timer.Start();
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
