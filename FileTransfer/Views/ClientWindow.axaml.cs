


using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using FileTransfer.ViewModels;
using System.ComponentModel;

namespace FileTransfer.Views
{
    public partial class ClientWindow : Window
    {
        ClientWindowViewModel clientWindowViewModel;
        string[] fullFilePaths;
        public ClientWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            clientWindowViewModel = new ClientWindowViewModel();
           
            DataContext = clientWindowViewModel;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if (clientWindowViewModel.IsConnected)
            {
                //MessageBox.Show("�Ѿ����ӣ������ظ�����", "����");
                return;
            }
            clientWindowViewModel.Connect(ChangeBtnColor);
        }

        private void SendText(object sender, RoutedEventArgs e)
        {
            clientWindowViewModel.SendText();
        }
        private void OpenFiles(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Title = "һ��ֻ�����ѡ����������һ��Ҳֻ����ǰ����";
            //openFileDialog.Multiselect = true;
            var res= openFileDialog.ShowAsync(this);
            //var res = openFileDialog.ShowDialog();
            if (res.Result!=null)
            {

                fullFilePaths = res.Result;
                foreach (string s in fullFilePaths)
                {
                    string tmp = s;
                    tmp += "\r\n";
                    clientWindowViewModel.ShowContent += tmp;

                }
            }

        }

        private void SendFile(object sender, RoutedEventArgs e)
        {
            if (fullFilePaths != null)
                clientWindowViewModel.SendFileRequest(fullFilePaths);
            //else
                //MessageBox.Show("����δѡ���ļ�,��ѡ���ļ�");
        }
        void ChangeBtnColor(bool connected)
        {
            if (connected)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ConnBtn.Content = "������";
                    ConnBtn.Background = Brushes.LightBlue;
                });
            }
            else
            {
                
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ConnBtn.Content = "δ����";
                    ConnBtn.Background = Brushes.Red;
                });

            }

        }
        protected override void OnClosing(CancelEventArgs e)
        {

            clientWindowViewModel.Close();
        }

        private void ClearSendFileList(object sender, RoutedEventArgs e)
        {
            fullFilePaths = null;
            clientWindowViewModel.ShowContent = "";
        }
    }
}
