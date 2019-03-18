using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using Serilog;
using System;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.Closed += (a, b) => {
                if (this.DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.Dispose();
                }
            };
        }

        private void InitializeComponent()
        {
            try
            {
                AvaloniaXamlLoader.Load(this);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.ToString());
            }
        }
    }
}
