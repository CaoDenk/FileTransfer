using FileTransferWpf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FileTransferWpf.Tools
{
    internal class RecvHandle
    {
       
        const int OFFSET = 16;
         public static int GetDataType(byte[] data)
        {

            return BitConverter.ToInt32(data);
        }


        public static string GetProcessedText(byte[] data)
        {
            int len=BitConverter.ToInt32(data,4);

            return Encoding.UTF8.GetString(data, OFFSET, len);

        }

        public static RecvFile GetRecvFileInfo(byte[] data)
        {

            int jsInfoSize = BitConverter.ToInt32(data, 4);
            ReadOnlySpan<byte> jsInfoHeaderByte= new ReadOnlySpan<byte>(data);
            JsonNode jsn  = JsonNode.Parse(jsInfoHeaderByte.Slice(OFFSET, jsInfoSize));
           
            //if(jsn==null)
            //{
            //    throw new Exception();
            //}

            RecvFile recvFile = new RecvFile();
            recvFile.filename = jsn.GetValue<string>("filename");
            recvFile.filesize = jsn.GetValue<long>("filesize");
            recvFile.uuidbytes = Encoding.UTF8.GetBytes(jsn.GetValue<string>("uuid"));
            
            return recvFile;
        }


    }
}
