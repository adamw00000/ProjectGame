using Avalonia;
using Avalonia.Markup.Xaml;

namespace ProjectGameGUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
