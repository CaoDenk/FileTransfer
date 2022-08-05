
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
            Array.Copy(bytes, 0, dataWithHeader, OFFSET, bytes.Length);
            return dataWithHeader;
        }

       /// <summary>
       /// 添加发送文件请求头信息比如uuid
       /// </summary>
       /// <param name="file"></param>
       /// <returns></returns>
      public static  byte[] AddSendFileInfoHead(string fullFilePath, string uuid)
        {

            FileInfo fileInfo = new FileInfo(fullFilePath);

            JsonObject js= new JsonObject();
            js.Add("filename",fileInfo.Name);
            js.Add("filesize", fileInfo.Length);
       

            js.Add("uuid", uuid);

            string jstr=js.ToString();
            byte[] jsoBytes = Encoding.UTF8.GetBytes(jstr);
    
            byte[] data = new byte[Config.TEXT_BUFER_SIZE];
            WriteDataToBuffer(data, InfoHeader.FILE,0);
            WriteDataToBuffer(data,jsoBytes.Length,4);
            Array.Copy(jsoBytes,0,data,OFFSET, jsoBytes.Length);
            return data;
        }
      public static  void WriteDataToBuffer(byte[] data,int value,int offset)
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
        public static byte[] RefuseRecv()
        {
            byte[] bytes = new byte[16];
            //Span<byte> span=new Span<byte>(bytes);
            WriteDataToBuffer(bytes, InfoHeader.REFUSE_RECV, 16);
            return bytes;

        }
        /// <summary>
        /// 添加包序，和继续发送标志
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="packageOrder"></param>
        public static void AddContinueRecv(byte[] buf,int packageOrder)
        {

            WriteDataToBuffer(buf, InfoHeader.CONTINUE_RECV, 0);
            WriteDataToBuffer(buf, packageOrder, 4);

        }
        /// <summary>
        /// 发送结束标志
        /// </summary>
        /// <returns></returns>
        public static byte[] SendFinished(byte[] uuidBytes)
        {

            byte[] bytes = new byte[16];
            WriteDataToBuffer(bytes, InfoHeader.FINISHED, 0);
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
            WriteDataToBuffer(bytes, InfoHeader.OK_RECV, 0);
            //WriteDataToBuffer(data, 0, data.Length);
            Array.Copy(uuidbytes, 0, bytes, 8, 8);
            return bytes;

        }
 
        public static byte[] SendResendPack(byte[] uuid,int packnum,long recvFileSize)
        {
            byte[] bytes=new byte[32];
            WriteDataToBuffer(bytes, InfoHeader.RESEND_PACK, 0);
            WriteDataToBuffer(bytes, packnum, 4);
            Array.Copy(uuid, 0, bytes, 8, 8);
            
            Span<byte> buffer = new Span<byte>(bytes);
            BitConverter.TryWriteBytes(buffer.Slice(OFFSET), recvFileSize);
            return bytes;
        }
        public static byte[] SendCloseSend(byte[] uuid)
        {
            byte[] bytes = new byte[32];
            WriteDataToBuffer(bytes, InfoHeader.CLOSE_SEND, 0);
            Array.Copy(uuid, 0, bytes, 8, 8);
            Span<byte> buffer = new Span<byte>(bytes);
            return bytes;
        }
    }
}
