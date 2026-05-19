using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Receptserver.Laegehus.App.Services;
using Receptserver.Laegehus.App.ViewModels;

namespace Receptserver.Laegehus.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var apiBaseUrl = Environment.GetEnvironmentVariable("RECEPTSERVER_API_URL")
                ?? "http://localhost:5078";

            var http = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            var apiClient = new LaegehusApiClient(http);
            var viewModel = new MainWindowViewModel(apiClient);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            // Indlæs lægehuse og apoteker så snart vinduet vises.
            _ = viewModel.LoadInitialDataAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
