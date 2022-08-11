using FileTransferWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileTransferWpf.Views
{
    /// <summary>
    /// ServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        ServerWindowViewModel serverWindowViewModel = new ServerWindowViewModel();
        public ServerWindow()
        {
            InitializeComponent();
            //serverWindowViewModel = new ServerWindowViewModel();
            DataContext= serverWindowViewModel;

            
        }

        private void BindPort(object sender, RoutedEventArgs e)
        {
            if(serverWindowViewModel.IsBound)
            {
                MessageBox.Show("已经绑定，无需重复绑定", "警告");
                return;
            }
            bool res= serverWindowViewModel.Bind();
           //serverWindowViewModel.SetBufSize(unitSize.SelectedIndex);
            if(!res)
            {
                MessageBox.Show("绑定端口失败，请尝试更换端口", "错误");
            }else
            {
                Button btn = (Button)(sender);
                btn.Content = "已绑定";
                btn.Background = Brushes.LightBlue;
                serverWindowViewModel.Listen(stackTag);
            }

        }

 

        protected override void OnClosing(CancelEventArgs e)
        {

            serverWindowViewModel.Close();
        }

        private void unitSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serverWindowViewModel.SetBufSize(unitSize.SelectedIndex);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            serverWindowViewModel.SetBufSize(unitSize.SelectedIndex);
        }
    }
}
