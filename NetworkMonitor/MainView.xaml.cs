using System.Windows;

namespace NetworkMonitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkDataMonitor monitor = NetworkDataMonitor.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
