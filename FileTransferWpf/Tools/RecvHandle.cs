using FileTransferWpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FileTransferWpf.Tools
{
    internal class RecvHandle
    {
       
        const int offset = 16;
         public static int GetDataType(byte[] data)
        {

            return BitConverter.ToInt32(data);
        }


        public static string GetProcessedText(byte[] data,int len)
        {

            return Encoding.UTF8.GetString(data,offset,len-offset);

        }

        public static RecvFile GetRecvFileInfo(byte[] data)
        {

            int jsInfoSize = BitConverter.ToInt32(data, 4);
            ReadOnlySpan<byte> jsInfoHeaderByte= new ReadOnlySpan<byte>(data);
            JsonNode jsn = JsonObject.Parse(jsInfoHeaderByte.Slice(offset,jsInfoSize));
           
            RecvFile recvFile = new RecvFile();
            recvFile.filename = jsn.GetValue<string>("filename");
            recvFile.filesize = jsn.GetValue<long>("filesize");
            recvFile.uuidbytes = Encoding.UTF8.GetBytes(jsn.GetValue<string>("uuid"));
            //recvFile.packagenum = jsn.GetValue<int>("packagenum");
            //recvFile.packagenum
           //string res=  jsonObject.GetValue<string>();
            
            return recvFile;
        }


    }
}
