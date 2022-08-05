using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferWpf.Models
{
    internal class UUIDRecvFileModel
    {
        public FileStream stream;
        public long filesize;
        public int packnum;
        //public List<int> verifyPack=new List<int>();
        public DateTime start;
        public long hasRecvSize=0;
        public int packOrder=1;
        //public DateTime end;
    }
}
