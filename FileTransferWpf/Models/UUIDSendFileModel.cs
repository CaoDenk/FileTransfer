using FileTransferWpf.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferWpf.Models
{
    internal class UUIDSendFileModel
    {
        public FileStream stream;
        public string filepath;
        public int packnum ;
        public ShowPercent showPercent;

        public byte[] data;

        public int totalpacknum;

        public UUIDSendFileModel(int size)
        { 
            data = new byte[size];
        }

    }
}
