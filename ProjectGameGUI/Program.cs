using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Logging.Serilog;
using GameLib;
using ProjectGameGUI.ViewModels;
using ProjectGameGUI.Views;

namespace ProjectGameGUI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Task inputReaderTask = InteractiveInputProvider.ReadInput();
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());

            await inputReaderTask;
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
