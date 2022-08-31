using System.IO;
using System.Net.Sockets;
using System.Text;

namespace FileTransferWpf.Models
{
    internal class Client
    {

        public string Ip { get; set; }
        public int Port { get; set; } = 8080;
        public Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public Client()
        {
            FileStream fileStream = File.OpenRead("tmp/savedip.txt");
            //if(fileStream.CanWrite)
            byte[] buf=new byte[32];
            int len = fileStream.Read(buf);
            Ip=Encoding.Default.GetString(buf, 0, len);
            fileStream.Close();
        }
    }
}
