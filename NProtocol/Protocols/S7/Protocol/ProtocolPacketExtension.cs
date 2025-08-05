using NProtocol.Base;
using NProtocol.Extensions;
using NProtocol.Protocols.S7.CpuInfo;
using NProtocol.Protocols.S7.Enums;
using NProtocol.Protocols.S7.Enums.UserData;
using System;
using System.Text;

namespace NProtocol.Protocols.S7
{
    /// <summary>
    /// S7协议扩展功能性方法
    /// </summary>
    public partial class S7Client
    {
        /// <summary>
        /// 创建扩展报文包
        /// </summary>
        /// <param name="userDataFunction">用户数据功能码</param>
        /// <param name="subFunction">子功能码</param>
        /// <param name="payload">负载数据</param>
        /// <returns></returns>
        private byte[] CreateExtensionPacket(UserDataFunction userDataFunction, byte subFunction, byte[] payload)
        {
            var tpkt = CreateTpktPacket();
            var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
            var header = CreateS7CommHeaderPacket(S7CommPduType.UserData);
            var parameter = CreateS7CommParameterPacket(S7CommFuncCode.CpuServices, count: 1, userDataFunction: userDataFunction, subFunction: subFunction);
            byte[] dataLens = ((ushort)payload.Length).ToBytes();
            header[8] = dataLens[1];
            header[9] = dataLens[0];
            byte[] len = ((ushort)(tpkt.Length + cotp.Length + header.Length + parameter.Length + payload.Length)).ToBytes();
            tpkt[2] = len[1];
            tpkt[3] = len[0];
            return tpkt.Combine(cotp, header, parameter, payload);
        }

        /// <summary>
        /// 读取CPU时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Result<DateTime> GetCpuDateTime()
        {
            return EnqueueExecute(() =>
            {
                byte[] data = [0x0A, 0, 0, 0];
                var sendData = CreateExtensionPacket(UserDataFunction.TimeFunctions, (byte)TimeSubFunction.ReadClock, data);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int recLen = receivedData[31] * 256 + receivedData[32];
                recLen = GetReceivePaloadDataLength(receivedData[30], recLen);
                var readBytes = new byte[recLen];
                Buffer.BlockCopy(receivedData, 33, readBytes, 0, readBytes.Length);
                int year = Convert.ToInt32((readBytes[1] * 256 + readBytes[2]).ToString("x2"));
                int month = Convert.ToInt32(readBytes[3].ToString("x2"));
                int day = Convert.ToInt32(readBytes[4].ToString("x2"));
                int hour = Convert.ToInt32(readBytes[5].ToString("x2"));
                int minute = Convert.ToInt32(readBytes[6].ToString("x2"));
                int second = Convert.ToInt32(readBytes[7].ToString("x2"));
                int millisecond = Convert.ToInt32((readBytes[8] * 256 + (readBytes[9] & 0xF0)).ToString("x2"));
                string timeString = $"{year:00}-{month:00}-{day:00} {hour:00}:{minute:00}:{second:00}.{millisecond:000}";
                DateTime.TryParse(timeString, out DateTime dateTime);
                return result.ToResult(dateTime);
            });
        }

        /// <summary>
        /// 设置CPU时钟（可能有问题，不要使用）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public Result SetCpuDateTime(DateTime dateTime)
        {
            return EnqueueExecute(() =>
            {
                byte year1 = Convert.ToByte(dateTime.Year.ToString().Substring(0, 2), 16);
                byte year2 = Convert.ToByte(dateTime.Year.ToString().Substring(2, 2), 16);
                byte month = Convert.ToByte(dateTime.Month.ToString(), 16);
                byte day = Convert.ToByte(dateTime.Day.ToString(), 16);
                byte hour = Convert.ToByte(dateTime.Hour.ToString(), 16); ;
                byte minute = Convert.ToByte(dateTime.Minute.ToString(), 16);
                byte second = Convert.ToByte(dateTime.Second.ToString(), 16);
                int mw = (dateTime.Millisecond << 4) + ((byte)dateTime.DayOfWeek & 0x0F);
                byte mw1 = (byte)(mw >> 8);
                byte mw2 = (byte)mw;
                byte[] data = new byte[14] {
                0xFF, 0x09, 0, 0x0A,
                0,
                year1,
                year2,
                month,
                day,
                hour,
                minute,
                second,
                mw1,
                mw2};
                var sendData = CreateExtensionPacket(UserDataFunction.TimeFunctions, (byte)TimeSubFunction.SetClock, data);
                return NoLockExecute(sendData);
            });
        }

        /// <summary>
        /// 获取CPU状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<CpuStatus> GetCpuStatusInfo()
        {
            return EnqueueExecute(() =>
            {
                byte[] data = [
                0xFF,//Success
                0x09,//octet string 
                0x00,
                0x04,//length=4
                0x04,
                0x24,//szl-id 
                0x00,
                0x00//szl-index
                  ];
                var sendData = CreateExtensionPacket(UserDataFunction.CPUFunctions, (byte)CpuSubFunction.ReadSZL, data);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int payloadLength = receivedData[31] * 256 + receivedData[32];
                var payload = receivedData.Slice(33);
                ushort szlId = payload.Slice(0, 2).ToUInt16(true);
                ushort szlIndex = payload.Slice(2, 2).ToUInt16(true);
                ushort szlPartialListLength = payload.Slice(4, 2).ToUInt16(true);
                ushort szlPartialListCount = payload.Slice(6, 2).ToUInt16(true);
                ushort ereig = payload.Slice(8, 2).ToUInt16(true);
                byte ae = payload[10];
                var status = (CpuStatus)payload[11];
                var reserved = payload.Slice(12, 4);
                byte anlinfo1 = payload[16];
                byte anlinfo2 = payload[17];
                byte anlinfo3 = payload[18];
                byte anlinfo4 = payload[19];//no startup type
                var dateTimeBuffer = payload.Slice(20);
                int year = Convert.ToInt32(dateTimeBuffer[0].ToString("x2"));
                int month = Convert.ToInt32(dateTimeBuffer[1].ToString("x2"));
                int day = Convert.ToInt32(dateTimeBuffer[2].ToString("x2"));
                int hour = Convert.ToInt32(dateTimeBuffer[3].ToString("x2"));
                int minute = Convert.ToInt32(dateTimeBuffer[4].ToString("x2"));
                int second = Convert.ToInt32(dateTimeBuffer[5].ToString("x2"));
                int millisecond = Convert.ToInt32((dateTimeBuffer[6] * 256 + (dateTimeBuffer[7] & 0xF0)).ToString("x2"));
                string timeString = $"{year:00}-{month:00}-{day:00} {hour:00}:{minute:00}:{second:00}.{millisecond:000}";
                DateTime.TryParse(timeString, out DateTime dateTime);
                return result.ToResult(status);
            });
        }

        /// <summary>
        /// 设置CPU停止
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result SetCpuStop()
        {
            return EnqueueExecute(() =>
            {
                var tpkt = CreateTpktPacket();
                var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
                var s7Comm = CreateS7CommPacket(S7CommPduType.Job, S7CommFuncCode.PlcStop);
                var packet = tpkt.Combine(cotp, s7Comm);
                byte[] cmdLength = ((ushort)packet.Length).ToBytes();
                packet[2] = cmdLength[0];
                packet[3] = cmdLength[1];
                return NoLockExecute(packet);
            });
        }

        /// <summary>
        /// 热重启
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result SetCpuHotRestart()
        {
            return EnqueueExecute(() =>
            {
                var tpkt = CreateTpktPacket();
                var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
                var s7Comm = CreateS7CommPacket(S7CommPduType.Job, S7CommFuncCode.PlcControl);
                var piServicePayload = new byte[] {
                0x00,0x00,0x00,0x00,0x00,0x00,0xFD ,//unknown
                0x00,0x00,//parameter block length
                0x09,//string length
                0x50,0x5F,0X50,0X52,0X4F,0X47,0X52,0X41,0X4D//P_PROGRAM
                  };
                byte[] piServicePayloadLength = ((ushort)(piServicePayload.Length + 1)).ToBytes();
                s7Comm[6] = piServicePayloadLength[0];
                s7Comm[7] = piServicePayloadLength[1];
                var packet = tpkt.Combine(cotp, s7Comm, piServicePayload);
                byte[] cmdLength = ((ushort)packet.Length).ToBytes();
                packet[2] = cmdLength[0];
                packet[3] = cmdLength[1];
                return NoLockExecute(packet);
            });
        }

        /// <summary>
        /// 冷重启
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result SetCpuColdRestart()
        {
            return EnqueueExecute(() =>
            {
                var tpkt = CreateTpktPacket();
                var cotp = CreateCotpFuctionPacket(CotpPduType.Data);
                var s7Comm = CreateS7CommPacket(S7CommPduType.Job, S7CommFuncCode.PlcControl);
                var piServicePayload = new byte[] {
                0x00,0x00,0x00,0x00,0x00,0x00,0xFD ,//unknown
                0x00,0x02,//parameter block length
                0x43,0x20,//parameter block "C "  
                0x09,//string length
                0x50,0x5F,0X50,0X52,0X4F,0X47,0X52,0X41,0X4D//P_PROGRAM
                 };
                byte[] piServicePayloadLength = ((ushort)(piServicePayload.Length + 1)).ToBytes();
                s7Comm[6] = piServicePayloadLength[0];
                s7Comm[7] = piServicePayloadLength[1];
                var packet = tpkt.Combine(cotp, s7Comm, piServicePayload);
                byte[] cmdLength = ((ushort)packet.Length).ToBytes();
                packet[2] = cmdLength[0];
                packet[3] = cmdLength[1];
                return NoLockExecute(packet);
            });
        }

        /// <summary>
        /// 获取CPU系统信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<CpuModelInfo> GetCpuModelInfo()
        {
            return EnqueueExecute(() =>
            {
                byte[] data = new byte[8] {
                0xFF,//Success
                0x09,//octet string 
                0x00,
                0x04,//length=4
                0x00,
                0x11,//szl-id
                0x00,
                0x00//szl-index 
                 };
                var sendData = CreateExtensionPacket(UserDataFunction.CPUFunctions, (byte)CpuSubFunction.ReadSZL, data);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int payloadLength = receivedData[31] * 256 + receivedData[32];
                var payload = receivedData.Slice(33);
                ushort szlId = payload.Slice(0, 2).ToUInt16(true);
                ushort szlIndex = payload.Slice(2, 2).ToUInt16(true);
                ushort szlPartialListLength = payload.Slice(4, 2).ToUInt16(true);
                ushort szlPartialListCount = payload.Slice(6, 2).ToUInt16(true);
                var info = new CpuModelInfo(payload.Slice(8), Encoding.ASCII);
                return result.ToResult(info);
            });
        }

        /// <summary>
        /// 获取CPU组件信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<CpuCompnenetInfo> GetCpuComponentInfo()
        {
            return EnqueueExecute(() =>
            {
                byte[] data = new byte[8] {
                0xFF,//Success
                0x09,//octet string 
                0x00,
                0x04,//length=4
                0x00,
                0x1C,//szl-id 
                0x00,
                0x00//szl-index 
                };
                var sendData = CreateExtensionPacket(UserDataFunction.CPUFunctions, (byte)CpuSubFunction.ReadSZL, data);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int payloadLength = receivedData[31] * 256 + receivedData[32];
                var payload = receivedData.Slice(33);
                ushort szlId = payload.Slice(0, 2).ToUInt16(true);
                ushort szlIndex = payload.Slice(2, 2).ToUInt16(true);
                ushort szlPartialListLength = payload.Slice(4, 2).ToUInt16(true);
                ushort szlPartialListCount = payload.Slice(6, 2).ToUInt16(true);
                var info = new CpuCompnenetInfo(payload.Slice(8), Encoding.ASCII);
                return result.ToResult(info);
            });
        }

        /// <summary>
        /// 获取CPU通讯能力信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<CpuCommunicationCapabilityInfo> GetCpuCommunicationCapabilityInfo()
        {
            return EnqueueExecute(() =>
            {
                byte[] data = new byte[8] {
                0xFF,//Success
                0x09,//octet string 
                0x00,
                0x04,//length=4
                0x01,
                0x31,//szl-id 
                0x00,
                0x01//szl-index 
                };
                var sendData = CreateExtensionPacket(UserDataFunction.CPUFunctions, (byte)CpuSubFunction.ReadSZL, data);
                var result = NoLockExecute(sendData);
                var receivedData = result.ReceivedData;
                int payloadLength = receivedData[31] * 256 + receivedData[32];
                var payload = receivedData.Slice(33);
                ushort szlId = payload.Slice(0, 2).ToUInt16(true);
                ushort szlIndex = payload.Slice(2, 2).ToUInt16(true);
                ushort szlPartialListLength = payload.Slice(4, 2).ToUInt16(true);
                ushort szlPartialListCount = payload.Slice(6, 2).ToUInt16(true);
                var info = new CpuCommunicationCapabilityInfo(payload.Slice(8));
                return result.ToResult(info);
            });
        }
    }
}
