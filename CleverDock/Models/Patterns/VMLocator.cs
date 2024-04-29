using CleverDock.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CleverDock.Patterns;

public class VMLocator
{
    private static readonly IServiceProvider provider;

    static VMLocator()
    {
        var container = new ServiceCollection();
        container.AddSingleton<MainViewModel>();
        container.AddSingleton<ThemeSettingsViewModel>();
        provider = container.BuildServiceProvider();
    }

    public static MainViewModel Main => provider.GetRequiredService<MainViewModel>();

    public static ThemeSettingsViewModel ThemeSettings => provider.GetRequiredService<ThemeSettingsViewModel>();
}