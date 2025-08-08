using System.ComponentModel;
using NProtocol.Base;
using NProtocol.Extensions;
using NProtocol.Protocols.S7.Enums;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// 建立连接用到的协议包
    /// </summary>
    public partial class S7Client
    {
        /// <summary>
        /// 建立连接
        /// </summary>
        public void EstablishConnection()
        {
            ConnectionRequest();
            ConnectionSetup();
        }

        /// <summary>
        /// 连接请求
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Result ConnectionRequest()
        {
            //发送
            //TPKT 03 00 00 16
            //COTP 11 e0 00 00 00 01 00 c0 01 0a c1 02 01 00 c2 02 03 01

            //接收
            //TPKT 03 00 00 16
            //COTP 11 d0 00 01 00 42 00 c0 01 0a c1 02 01 00 c2 02 03 01
            return EnqueueExecute(() =>
            {
                var sendData = GetConnectionRequestPacket();
                return NoLockExecute(sendData);
            });
        }

        /// <summary>
        /// 连接设置
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Result ConnectionSetup()
        {
            //发送
            //TPKT   03 00 00 19
            //COTP   02 f0 80
            //S7 Comm
            //  Header 32 01 00 00 00 01 00 08 00 00
            //  Parameter  f0 00 00 01 00 01 03 c0

            //接收
            //TPKT   03 00 00 1b
            //COTP   02 f0 80
            //S7 Comm
            //  Header      32 03 00 00 00 01 00 08 00 00 00 00
            //  Parameter   f0 00 00 01 00 01 03 c0
            return EnqueueExecute(() =>
            {
                var sendData = GetS7ConnectionSetupPacket();
                return NoLockExecute(sendData);
            });
        }

        /// <summary>
        /// 获取连接请求包
        /// </summary>
        /// <returns></returns>
        private byte[] GetConnectionRequestPacket()
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpConnectionPacket(CotpPduType.ConnectRequest);
            byte[] len = ((ushort)(tpkt.Length + cotp.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp);
        }

        /// <summary>
        /// 获取S7协议连接设置报文
        /// </summary>
        /// <returns></returns>
        private byte[] GetS7ConnectionSetupPacket()
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
            var s7Comm = CreateS7CommPacket(S7CommPduType.Job, S7CommFuncCode.SetupCommunication);
            byte[] len = ((ushort)(tpkt.Length + cotp.Length + s7Comm.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp, s7Comm);
        }
    }
}
