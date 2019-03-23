using System;
using System.Diagnostics;
using System.Linq;
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
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, data) => {
                    if (data?.Data == null)
                    {
                        return;
                    }

                    cache += data.Data + "\n";

                    string[] cachArray = cache.Split("UDEV  [");

                    if (cachArray.Length > 0)
                    {
                        bool lastItemIsMatched = false;
                        for (int i = 0; i < cachArray.Length; i++)
                        {
                            string block = cachArray[i];

                            int indexIdFsType = block.IndexOf("ID_FS_TYPE=");
                            int indexDevName = block.IndexOf("DEVNAME=");

                            if (indexDevName > -1 && indexIdFsType > -1)
                            {
                                indexIdFsType += 11;
                                int endIndex = block.IndexOf('\n', indexIdFsType);
                                string idFsType = endIndex == -1 ?
                                 block.Substring(indexIdFsType) : block.Substring(indexIdFsType, endIndex - indexIdFsType);

                                indexDevName += 8;
                                endIndex = block.IndexOf('\n', indexDevName);
                                string devName = endIndex == -1 ? 
                                 block.Substring(indexDevName) : block.Substring(indexDevName, endIndex - indexDevName);
                                
                                if (i == cachArray.Length - 1)
                                {
                                    lastItemIsMatched = true;
                                }

                                DriveEvent?.Invoke(this, new UsbVolumeChangeEventArgs(VolumeEventType.Inserted, devName, idFsType));
                            }
                            if (lastItemIsMatched)
                            {
                                cache = string.Empty;
                            }
                            else if (cachArray.Length > 1)
                            {
                                cache = "UDEV  [" + cachArray.Last();
                            }
                        }
                    }
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
