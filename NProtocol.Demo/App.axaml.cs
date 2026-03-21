using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using NProtocol.Demo.Services;
using NProtocol.Demo.ViewModels;
using NProtocol.Demo.ViewModels.Pages;
using NProtocol.Demo.Views;
using NProtocol.Demo.Views.Pages;
using System.Linq;

namespace NProtocol.Demo
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            RegisterPage();
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
        private void RegisterPage()
        {
            NavigateService.Default.RegisterPage("Fins", typeof(FinsDemoView), typeof(FinsDemoViewModel));
            NavigateService.Default.RegisterPage("Modbus", typeof(ModbusDemoView), typeof(ModbusDemoViewModel));
        }
    }
}