﻿using FileTransferWpf.Elements;
using FileTransferWpf.GlobalConfig;
using FileTransferWpf.Header;
using FileTransferWpf.Models;
using FileTransferWpf.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileTransferWpf.ViewModels
{
    internal class ServerWindowViewModel
    {
        Server server = new Server();

        public int Port { get { return server.Port; } set { server.Port = value; } }
        public Socket ServerSocket { get { return server.ServerSocket; } set { server.ServerSocket = value; } }
        public bool IsBound => ServerSocket.IsBound;

        public Dictionary<byte[], UUIDRecvFileModel> uuidRecvDict = new Dictionary<byte[], UUIDRecvFileModel>(new ByteCmp());

        Dictionary<StackPanel,Socket> stackPanelSocketDict = new Dictionary<StackPanel,Socket>();
        List<Socket> clients = new List<Socket>();
        //CancellationTokenSource cts = new CancellationTokenSource();
        /// <summary>
        /// 绑定端口
        /// </summary>
        public bool Bind()
        {

            EndPoint endPoint = new System.Net.IPEndPoint(IPAddress.Any, Port);
            try
            {
                ServerSocket.Bind(endPoint);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }


        public void Close()
        {
            //cts.Cancel();
            foreach(Socket socket in stackPanelSocketDict.Values)
            { 
                socket.Close();
            }

            ServerSocket.Close();

        }

        public void Listen(StackPanel panel, RoutedEventHandler @event)
        {
            ServerSocket.Listen();
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        try{
                            Socket client = ServerSocket.Accept();
                            //clients.
                            StackPanel stackPanel=  AddElements.AddElement(panel, @event);
                            stackPanelSocketDict.Add(stackPanel, client);
                            Task.Factory.StartNew(() =>
                            {
                                ReceiveByOneClient(panel,client, stackPanel);
                            }
                           );

                        }catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                            return;
                        }

                    }
                }
                );
         


        }
        void ReceiveByOneClient(StackPanel parentPanel,Socket client, StackPanel panel)
        {
            byte[] buf = new byte[Config.FILE_BUFFER_SIZE];
            ProgressBar? progressBar = null;

            byte[] uuidbytes=null;
            while (true)
            {

                try
                {
                    int len = client.Receive(buf);
                    int type = RecvHandle.GetDataType(buf);
                    switch (type)
                    {
                        case InfoHeader.TEXT:
                            ShowContent(buf, panel, len);
                            break;
                        case InfoHeader.FILE:
                            RecvFile recv = RecvHandle.GetRecvFileInfo(buf);
                            string msg = string.Format("是否接收文件{0},文件大小{1}字节", recv.filename, recv.filesize);
                            MessageBoxResult messageBoxResult = MessageBox.Show(msg, "消息", MessageBoxButton.YesNo);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.FileName = recv.filename;
                                bool? res = saveFileDialog.ShowDialog();
                                if (res.HasValue && res.Value)
                                {

                                    //创建文件
                                    FileStream fileStream = File.Create(saveFileDialog.FileName);

                                    UUIDRecvFileModel uUIDFileModel = new UUIDRecvFileModel();
                                    uUIDFileModel.filesize = recv.filesize;
                                    uUIDFileModel.packnum = recv.packagenum;
                                    uUIDFileModel.stream = fileStream;
                                    uUIDFileModel.start = DateTime.Now;

                                    //存在dict中
                                    uuidRecvDict.Add(recv.uuidbytes, uUIDFileModel);
                                    progressBar = GetProgressFromStackPanel(panel);

                                    byte[] data = SendHandle.AllowRecv(recv.uuidbytes);
                                    client.Send(data, SocketFlags.None);
                                    break;
                                }
                            }
                            client.Send(SendHandle.RefuseRecv(), SocketFlags.None);
                            break;
                        case InfoHeader.CONTINUE_RECV:
                            uuidbytes = buf[8..16];
                            int packnum = BitConverter.ToInt32(buf, 4);
                            if (packnum != uuidRecvDict[uuidbytes].packOrder)
                            {
                                client.Send(SendHandle.SendResendPack(uuidbytes, 0, uuidRecvDict[uuidbytes].hasRecvSize));
                                break;
                            }
                            uuidRecvDict[uuidbytes].packOrder++;
                            uuidRecvDict[uuidbytes].hasRecvSize += len - 16;
                            double percent = uuidRecvDict[uuidbytes].hasRecvSize * 1.0 / uuidRecvDict[uuidbytes].filesize * 100;

                            SetBarValue(progressBar, percent);


                            uuidRecvDict[uuidbytes].stream.Write(buf, 16, len - 16);
                            uuidRecvDict[uuidbytes].stream.Flush();

                            //发送接受成功
                            byte[] recvOk = SendHandle.SendRecvOk(uuidbytes);
                            client.Send(recvOk, SocketFlags.None);
                            break;
                        case InfoHeader.FINISHED:

                            uuidbytes = buf[8..16];
                            if (!uuidRecvDict.ContainsKey(uuidbytes))
                            {
                                break;
                            }
                            if (uuidRecvDict[uuidbytes].hasRecvSize == uuidRecvDict[uuidbytes].filesize)
                            {
                                TimeSpan duration = DateTime.Now - uuidRecvDict[uuidbytes].start;
                                MessageBox.Show("接收完成,花费" + duration.TotalSeconds + "s");
                                client.Send(SendHandle.SendCloseSend(uuidbytes));
                                uuidRecvDict[uuidbytes].stream.Close();
                                uuidRecvDict.Remove(uuidbytes);
                                SetBarValue(progressBar, 0);
                            }
                            else
                            {
                                // MessageBox.Show("接收失败");
                                client.Send(SendHandle.SendResendPack(uuidbytes, 0, uuidRecvDict[uuidbytes].hasRecvSize));
                            }

                            break;

                        default:
                            if (uuidRecvDict.ContainsKey(uuidbytes))
                            {

                                client.Send(SendHandle.SendResendPack(uuidbytes, 0, uuidRecvDict[uuidbytes].filesize));
                                break;
                            }

                            throw new Exception("不合法请求头");
                    }



                }
                catch (Exception e)
                {

                    return;

                }


            }

        }  
        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="sender"></param>
       public void SendText(Button sender)
        {
            StackPanel parent=  sender.Parent as StackPanel;
            TextBox textBox = parent.FindByName<TextBox>("Content");
            byte[] data = SendHandle.AddTextInfoHeader(textBox.Text);
            stackPanelSocketDict[parent].SendAsync(data, SocketFlags.None);
                
        }
        public void SetBarValue(ProgressBar bar,double value)
        {
            Application.Current.Dispatcher.Invoke( () => { bar.Value = value;});
        }

        /// <summary>
        /// 显示接收文本
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="panel"></param>
        /// <param name="len"></param>
         void ShowContent(byte[] buf,StackPanel panel,int len)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TextBlock texblock = panel.FindByName<TextBlock>("ShowRecvText");
                texblock.Text = RecvHandle.GetProcessedText(buf, len);
            });

        }
        ProgressBar GetProgressFromStackPanel(StackPanel panel)
        {
            return Application.Current.Dispatcher.Invoke<ProgressBar>(() =>{ 
                return panel.FindByName<ProgressBar>("ProgressBar");} );
        }
        
  

        
    }
}