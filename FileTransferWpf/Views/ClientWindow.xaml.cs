﻿using FileTransferWpf.ViewModels;
using Microsoft.Win32;
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

namespace FileTransferWpf.views
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window
    {
        ClientWindowViewModel clientWindowViewModel;
        string[] fullFilePaths;

        public ClientWindow()
        {
            InitializeComponent();
            clientWindowViewModel = new ClientWindowViewModel();
            clientWindowViewModel.panel = ClientStackTag;
            DataContext = clientWindowViewModel;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if(clientWindowViewModel.IsConnected)
            {
                MessageBox.Show("已经连接，请勿重复操作", "警告");
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
            //openFileDialog.Title = "一次只能最多选两个，否则一次也只发送前两个";
            //openFileDialog.Multiselect = true;
            
            bool? res=openFileDialog.ShowDialog();
            if(res.HasValue)
            {

                fullFilePaths = openFileDialog.FileNames;
                foreach(string s in fullFilePaths)
                {
                    string tmp=s ;
                    tmp += "\r\n";
                    clientWindowViewModel.ShowContent += tmp;

                }
            }

        }

        private void SendFile(object sender, RoutedEventArgs e)
        {
            if (fullFilePaths != null)
                clientWindowViewModel.SendFileRequest(fullFilePaths);
            else
                MessageBox.Show("您还未选择文件,请选择文件");
        }
        void ChangeBtnColor(bool connected)
        {
            if (connected)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnBtn.Content = "已连接";
                    ConnBtn.Background = Brushes.LightBlue;
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnBtn.Content = "未连接";
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