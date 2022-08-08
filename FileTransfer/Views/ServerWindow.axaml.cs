using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FileTransfer.Tools;
using FileTransfer.ViewModels;
using System.ComponentModel;

namespace FileTransfer.Views
{
    public partial class ServerWindow : Window
    {
        ServerWindowViewModel serverWindowViewModel;

        public ServerWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
           
            serverWindowViewModel = new ServerWindowViewModel();
            DataContext = serverWindowViewModel;
        }



        private void BindPort(object sender, RoutedEventArgs e)
        {
            if (serverWindowViewModel.IsBound)
            {
                MyMessageBox.Show("�Ѿ��󶨣������ظ���", "����");
                return;
            }
            bool res = serverWindowViewModel.Bind();
            if (!res)
            {
                MyMessageBox.Show("�󶨶˿�ʧ�ܣ��볢�Ը����˿�", "����");
            }
            else
            {
                Button btn = (Button)(sender);
                btn.Content = "�Ѱ�";
                btn.Background = Brush.Parse("LightBlue");
                serverWindowViewModel.Listen(stackTag,SendText,this);
            }

        }

        private void SendText(object sender, RoutedEventArgs e)
        {
            serverWindowViewModel.SendText((Button)sender);
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            serverWindowViewModel.Close();
        }

    }
}
