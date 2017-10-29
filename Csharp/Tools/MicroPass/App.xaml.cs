using System.Windows;
using System.Windows.Threading;

namespace MicroPass
{
    /// <summary>
    /// Handles global errors to allow for exceptions as messages.
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMsg = $"{e.Exception.GetType()}: {e.Exception.Message}";
            MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK);
            e.Handled = true;
        }
    }
}
