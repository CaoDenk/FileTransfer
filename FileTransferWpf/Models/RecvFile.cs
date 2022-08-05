using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferWpf.Models
{
    internal class RecvFile
    {
        public string filename;
        public long filesize;
        public int packagenum;
        public byte[] uuidbytes;
    }
}
