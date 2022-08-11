using FileTransferWpf.Elements;
using FileTransferWpf.GlobalConfig;
using FileTransferWpf.Header;
using FileTransferWpf.Models;
using FileTransferWpf.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileTransferWpf.ViewModels
{
    internal class ClientWindowViewModel: INotifyPropertyChanged
    {

        Client client = new Client();

        public Dictionary<byte[], UUIDSendFileModel> uuidSendDict = new Dictionary<byte[], UUIDSendFileModel>(new ByteCmp());
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Ip { get { return client.Ip; } set { client.Ip = value; } }
        public int Port { get { return client.Port; } set { client.Port = value; } }
        public Socket ClientSocket { get { return client.ClientSocket; } set { client.ClientSocket = value; } }
    
        public string TextInput { get; set; }="" ;
        public bool IsConnected => ClientSocket.Connected;


        public int fileBufSize { set; get; } = 32;

        byte[] filebuf;//= new byte[Config.FILE_BUFFER_SIZE];
        // List<string> 
        public StackPanel panel;
        string content;
        public string ShowContent 
        { 
            get { return content; }
            set {
                content = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ShowContent"));
            } 
        }



        public void Connect(ChangeBtnColor changeBtnColor)
        {    
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
            try
            {
                ClientSocket.Connect(endPoint);
                //绑定后开始接收
                Recv(changeBtnColor);
                changeBtnColor(true);
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
                          
        }
        /// <summary>
        /// 发送文本
        /// </summary>
        public void SendText()
        {
            SendHandle.AddTextInfoHeader(TextInput);
            byte[] data = SendHandle.AddTextInfoHeader(TextInput);
            ClientSocket.SendAsync(data, SocketFlags.None);
        }
        /// <summary>
        /// 发送"发送文件请求"
        /// </summary>
        /// <param name="file"></param>
        public void SendFileRequest(string[] fullFilePaths)
        {


            foreach (string fullFilePath in fullFilePaths)
            {

                string uuid = Guid.NewGuid().ToString()[0..8];
                FileInfo fileInfo = new FileInfo(fullFilePath);

                JsonObject js = new JsonObject();
                js.Add("filename", fileInfo.Name);
                js.Add("filesize", fileInfo.Length);


                js.Add("uuid", uuid);



                UUIDSendFileModel uUIDSendFileModel = new UUIDSendFileModel();
                uUIDSendFileModel.filepath = fullFilePath;
                uUIDSendFileModel.totalpacknum = (int)(fileInfo.Length / (Config.FILE_BUFFER_SIZE - 16) + 1);
                //uUIDSendFileModel.
                uuidSendDict.Add(Encoding.UTF8.GetBytes(uuid), uUIDSendFileModel);

                byte[] data = SendHandle.AddSendFileInfoHead(js, uuid);
                ClientSocket.Send(data, SocketFlags.None);
            }
            MessageBox.Show("已发送,等待对方同意接收");
        }

        /// <summary>
        /// 接受请求
        /// </summary>
        public void Recv(ChangeBtnColor changeBtnColor)
        {
            

            Task.Factory.StartNew(() =>
            {
                byte[] buf = new byte[Config.TEXT_BUFER_SIZE];
                while (true)
                {
                    try
                    {
                        int len = ClientSocket.Receive(buf);
                        int type = RecvHandle.GetDataType(buf);
                        byte[] uuidBytes = null;
                        switch (type)
                        {
                            case InfoHeader.TEXT:
                                ShowContent = RecvHandle.GetProcessedText(buf, len);
                                break;
                            case InfoHeader.FILE:
                                MessageBox.Show("是否接收文件", "消息", MessageBoxButton.YesNo);
                                break;
                            case InfoHeader.ALLOW_RECV:
                                uuidBytes = buf[8..16];
                                string filepath = uuidSendDict[uuidBytes].filepath;
                                FileStream fileStream = File.OpenRead(filepath);
                                uuidSendDict[uuidBytes].stream = fileStream;
                                uuidSendDict[uuidBytes].showPercent = AddElements.AddProgressFromStackPanel(panel);
                                SendFile(uuidBytes);
                                break;
                            case InfoHeader.RESEND_PACK:
                                uuidBytes = buf[8..16];
                                int packageOrder = BitConverter.ToInt32(buf, 4);
                                uuidSendDict[uuidBytes].packnum = packageOrder;
                                long offset = BitConverter.ToInt64(buf, 16);
                                ResendPack(uuidBytes, offset);
                                SendFile(uuidBytes);
                                break;

                            case InfoHeader.CLOSE_SEND:
                                uuidBytes = buf[8..16];
                                MessageBox.Show( "发送完成");
                                ShowPercent delshowPercent = uuidSendDict[uuidBytes].showPercent;

                                Application.Current.Dispatcher.Invoke(() => {
                                    panel.Children.Remove(delshowPercent.bar);
                                    panel.Children.Remove(delshowPercent.percent);
                                });


                                uuidSendDict[uuidBytes].stream.Close();
                                uuidSendDict.Remove(uuidBytes);
                                break;
                            case InfoHeader.OK_RECV:
                                uuidBytes = buf[8..16];
                                uuidSendDict[uuidBytes].packnum++;
                                double percent = (uuidSendDict[uuidBytes].packnum * 100.0 / uuidSendDict[uuidBytes].totalpacknum);
                                AddElements.SetBarValue(uuidSendDict[uuidBytes].showPercent, percent);
                          

                                SendFile(uuidBytes);
                                break;
                            case InfoHeader.REFUSE_RECV:
                                uuidBytes = buf[8..16];
                                uuidSendDict.Remove(uuidBytes);
                                MessageBox.Show("对方已拒绝接收");
                                break;
                            default:
                                throw new Exception("错误信息头");
                        }

                    }
                    catch (Exception e)
                    {
                        changeBtnColor(false);
                        CloseIOStream();
                        uuidSendDict.Clear();
                        ClientSocket.Close();
                        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        break;
                    }

                }

            }
            );

        }
        public void SendFile(byte[] uuidByte)
        {
            
            FileStream fileStream = uuidSendDict[uuidByte].stream;
            //Thread.Sleep(10);
            int len;
            if ((len=fileStream.Read(filebuf,16, Config.FULL_SIZE)) >0)
            {
                SendHandle.AddContinueRecv(filebuf, uuidSendDict[uuidByte].packnum);
                Array.Copy(uuidByte,0,filebuf,8,8);
                SendHandle.WriteDataToBuffer(filebuf, uuidSendDict[uuidByte].packnum, len+16);

                ClientSocket.Send(filebuf,0,len+20,SocketFlags.None);
               
            }
            else
            {
                ClientSocket.Send(SendHandle.SendFinished(uuidByte),SocketFlags.None);
            }
          
        }

        /// <summary>
        /// 重新发送下文件
        /// </summary>
        /// <param name="uuidBytes"></param>
        /// <param name="offset"></param>
        void ResendPack(byte[] uuidBytes,long offset)
        {
            uuidSendDict[uuidBytes].stream.Seek(offset, SeekOrigin.Begin);
        }
        public void Close()
        {

            CloseIOStream(); 
            ClientSocket.Close();

        }

        void CloseIOStream()
        {

            foreach (UUIDSendFileModel uUIDSendFileModel in uuidSendDict.Values)
            {
                uUIDSendFileModel.stream?.Close();
            }
        }

        public void SetBufSize(int index)
        {

            if(index == (int)UnitSize.KBYTES)
            {
                filebuf=new byte[fileBufSize*1024];
            }else
                filebuf=new byte[fileBufSize];

        }
        
    }
}



