using Avalonia;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Serilog;

namespace AvaloniaApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
             => AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .UseDataGrid();
    }
}
