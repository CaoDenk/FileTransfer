
using FileTransferWpf.GlobalConfig;
using FileTransferWpf.Header;
using FileTransferWpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FileTransferWpf.Tools
{
    internal class SendHandle
    {
       const int OFFSET = 16;

    /// <summary>
    /// 第一个是key是uuid，value 是文件名
    /// </summary>
       public  Dictionary<byte[], UUIDSendFileModel> _headers = new Dictionary<byte[], UUIDSendFileModel>(new ByteCmp());
       public static byte[] AddTextInfoHeader(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] dataWithHeader= new byte[bytes.Length+OFFSET];

            WriteDataToBuffer(dataWithHeader, InfoHeader.TEXT, 0);
            //第四字节写上文本大小
            WriteDataToBuffer(dataWithHeader, bytes.Length, 4);
            Array.Copy(bytes, 0, dataWithHeader, OFFSET, bytes.Length);
            return dataWithHeader;
        }
        /// <summary>
        /// 添加信息头
        /// </summary>
        /// <param name="js"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static byte[] AddSendFileInfoHead(JsonObject js, string uuid)
        {



            string jstr = js.ToString();
            byte[] jsoBytes = Encoding.UTF8.GetBytes(jstr);

            byte[] data = new byte[Config.TEXT_BUFER_SIZE];
            WriteDataToBuffer(data, InfoHeader.FILE, 0);
            WriteDataToBuffer(data, jsoBytes.Length, 4);
            Array.Copy(jsoBytes, 0, data, OFFSET, jsoBytes.Length);
            return data;
        }
        public static  void WriteDataToBuffer(byte[] data,int value,int offset=0)
        {
          
            Span<byte> span = new Span<byte>(data);
            BitConverter.TryWriteBytes(span.Slice(offset), value);
        }

        /// <summary>
        /// 发送同意接收文件请求
        /// </summary>
        /// <returns></returns>
        public static byte[] AllowRecv(byte[] uuidbytes)
        {

            byte[] bytes =new byte[16];
            Array.Copy(uuidbytes, 0, bytes, 8,8);
            //Span<byte> span=new Span<byte>(bytes);
            WriteDataToBuffer(bytes, InfoHeader.ALLOW_RECV, 0);
            return bytes;
        }
        public static byte[] RefuseRecv(byte[] uuibytes)
        {
            byte[] bytes = new byte[16];

            //Span<byte> span=new Span<byte>(bytes);
            Array.Copy(uuibytes,0, bytes, 8,8);
            WriteDataToBuffer(bytes, InfoHeader.REFUSE_RECV);
            return bytes;

        }
        /// <summary>
        /// 添加包序，和继续发送标志
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="packageOrder"></param>
        public static void AddContinueRecv(byte[] buf,int packageOrder)
        {

            WriteDataToBuffer(buf, InfoHeader.CONTINUE_RECV);
            WriteDataToBuffer(buf, packageOrder, 4);

        }
        /// <summary>
        /// 发送结束标志
        /// </summary>
        /// <returns></returns>
        public static byte[] SendFinished(byte[] uuidBytes)
        {

            byte[] bytes = new byte[16];
            WriteDataToBuffer(bytes, InfoHeader.FINISHED);
            Array.Copy(uuidBytes,0, bytes, 8,8);
            return bytes;
        }
        /// <summary>
        /// 发送接受成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SendRecvOk(byte[] uuidbytes)
        {
            byte[] bytes = new byte[16];
            WriteDataToBuffer(bytes, InfoHeader.OK_RECV);
            //WriteDataToBuffer(data, 0, data.Length);
            Array.Copy(uuidbytes, 0, bytes, 8, 8);
            return bytes;

        }
 
        public static byte[] SendResendPack(byte[] uuid,int packnum,long recvFileSize)
        {
            byte[] bytes=new byte[32];
            WriteDataToBuffer(bytes, InfoHeader.RESEND_PACK);
            WriteDataToBuffer(bytes, packnum, 4);
            Array.Copy(uuid, 0, bytes, 8, 8);
            
            Span<byte> buffer = new Span<byte>(bytes);
            BitConverter.TryWriteBytes(buffer.Slice(OFFSET), recvFileSize);
            return bytes;
        }
        public static byte[] SendCloseSend(byte[] uuid)
        {
            byte[] bytes = new byte[32];
            WriteDataToBuffer(bytes, InfoHeader.CLOSE_SEND);
            Array.Copy(uuid, 0, bytes, 8, 8);
            Span<byte> buffer = new Span<byte>(bytes);
            return bytes;
        }
    }
}
