using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.ViewModels
{
    public class UsbDriveEvents
    {
        private Process process;

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
                    Console.WriteLine(data.Data);
                };
                process.StartInfo.RedirectStandardError = true;
                process.ErrorDataReceived += (sender, data) => {
                    Console.WriteLine(data.Data);
                };
                process.Start();
            }
        }
    }
}
