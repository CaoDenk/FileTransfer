using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferWpf.ViewModels
{
    internal class MainWindowViewModel: INotifyPropertyChanged
    { 
        //string localIpv4;
        public string LocalIpv4 { set; get; }
        
        
        public MainWindowViewModel()
        {


            IPAddress[] ipAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList;//获得当前IP地址
                                                                                 //string ip = ipAddr.ToString();;

            foreach (IPAddress ipAddress in ipAddr)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    // Console.WriteLine(ipAddress.ToString());
                    LocalIpv4 = ipAddress.ToString();
                    break;
                }

            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
