using FileTransferWpf.ViewModels;
using FileTransferWpf.views;
using FileTransferWpf.Views;
using System.Windows;

namespace FileTransferWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mainWindowViewModel;
        //string 
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
            
        }

        private void NavicateToClientWindow(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Show();


        }
        private void NavicateToServerWindow(object sender, RoutedEventArgs e)
        {
            ServerWindow serverWindow = new ServerWindow();
            serverWindow.Show();
        }
    }
}
