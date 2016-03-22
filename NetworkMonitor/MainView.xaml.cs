using System.Windows;
using NetworkMonitor.Models;

namespace NetworkMonitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkPacketsReceiver monitor = NetworkPacketsReceiver.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
