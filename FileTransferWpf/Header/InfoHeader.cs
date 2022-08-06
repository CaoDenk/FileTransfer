using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferWpf.Header
{
    internal class InfoHeader
    {
        public const int TEXT=1024;
        public const int FILE=1;
        public const int ALLOW_RECV=10;
        public const int REFUSE_RECV=11;
        public const int CONTINUE_RECV=12;
        public const int FINISHED=13;
        public const int OK_RECV=14;
        public const int CANCEL_SEND = 15;
        public const int CANCEL_RECV=16;
        public const int RESEND_PACK=17;
        public const int CLOSE_SEND=18;
    }
}
