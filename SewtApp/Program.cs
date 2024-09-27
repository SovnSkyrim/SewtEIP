using Avalonia;
using System;

namespace SewtApp;

class Program
{
    // Code d'initialisation. N'utilisez pas d'API Avalonia, de bibliothèques tierces ou de code dépendant de SynchronizationContext avant que AppMain ne soit appelé : les choses ne sont pas encore initialisées et pourraient causer des problèmes.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Configuration d'Avalonia, ne pas supprimer ; également utilisé par le concepteur visuel.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
