using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubparRacing.Services;
using SubparRacing.ViewModels.Pages;
using SubparRacing.ViewModels.Windows;
using SubparRacing.Views.Pages;
using SubparRacing.Views.Windows;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Wpf.Ui;

namespace SubparRacing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static TelemetryService TelemetryService { get; private set; }

        static readonly byte[] handshake = { (byte)'s', (byte)'u', (byte)'b', (byte)'p', (byte)'a', (byte)'r' };
        public static readonly ArduinoConnection arduinoConnection = new ArduinoConnection(handshake);

        public static bool arduinoIsConnected = false;

        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            TelemetryService = new TelemetryService();
            _host.Start();
            
            //TelemetryService.Start();

            arduinoConnection.ArduinoConnected += OnArduinoConnected;
            arduinoConnection.ArduinoDisconnected += OnArduinoDisconnected;

            Debug.Write("Connecting to Arduino...");
            arduinoConnection.Start();

            RandomDataSender randomDataSender = new RandomDataSender(arduinoConnection);
            //randomDataSender.Start();

        }

        private void OnArduinoConnected(object connection, ArduinoConnection.ConnectionEventArgs connectionInformation)
        {
            arduinoIsConnected = true;

            Debug.WriteLine($"Arduino connected on port {connectionInformation.ArduinoPort?.PortName}!");
        }

        private void OnArduinoDisconnected(object connection, ArduinoConnection.ConnectionEventArgs connectionInformation)
        {
            arduinoIsConnected = false;

            Debug.WriteLine("Arduino disconnected!");
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            arduinoConnection.Stop();
            TelemetryService.Stop();

            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
