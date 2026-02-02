using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace AkhenTraderElite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AkhenTraderElite",
            "logs",
            "startup.log");

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeLogging();
            Log("App starting.");

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                Log("UnhandledException: " + args.ExceptionObject);
                FlushLog();
            };

            DispatcherUnhandledException += (_, args) =>
            {
                Log("DispatcherUnhandledException: " + args.Exception);
                MessageBox.Show(
                    "The app encountered a startup error. Please check the log at:\n" + LogFilePath,
                    "AKHENS TRADER",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
                FlushLog();
                Shutdown(-1);
            };

            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                Log("UnobservedTaskException: " + args.Exception);
                args.SetObserved();
                FlushLog();
            };

            try
            {
                base.OnStartup(e);
                Log("App startup completed.");
            }
            catch (Exception ex)
            {
                Log("Startup exception: " + ex);
                FlushLog();
                MessageBox.Show(
                    "Startup failed. Please check the log at:\n" + LogFilePath,
                    "AKHENS TRADER",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(-1);
            }
        }

        private static void InitializeLogging()
        {
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Log("=== Session Start " + DateTime.Now.ToString("O") + " ===");
            Log("Process: " + Environment.ProcessPath);
            Log("OS: " + Environment.OSVersion);
            Log("64-bit: " + Environment.Is64BitProcess);
        }

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFilePath, message + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // Ignore logging failures to avoid recursive crashes.
            }
        }

        private static void FlushLog()
        {
            try
            {
                Debug.Flush();
            }
            catch
            {
                // no-op
            }
        }
    }
}
