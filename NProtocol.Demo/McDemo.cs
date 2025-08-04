using NProtocol.Communication.Enums;
using NProtocol.Protocols.Mc;

namespace NProtocol.Demo
{
    public class McDemo
    {
        public static void Run()
        {
            Mc3E4EClient client1 = new Mc3E4EClient("192.168.244.130", 6000, ConnectMode.Tcp) { McFrame = McFrame.MC3E };
            Mc3E4EClient client2 = new Mc3E4EClient("192.168.244.130", 6000, ConnectMode.Udp) { McFrame = McFrame.MC3E };
            Mc3E4EClient client3 = new Mc3E4EClient("192.168.244.130", 6000, ConnectMode.Tcp) { McFrame = McFrame.MC4E };
            Mc3E4EClient client4 = new Mc3E4EClient("192.168.244.130", 6000, ConnectMode.Udp) { McFrame = McFrame.MC4E };
            client1.LogReadWriteRaw += (driverId, rw, data) =>
            {
            };
            client1.Connect();

            var result1 = client1.WriteValues("M0", new ushort[] { 0x17FD, 0xFA14 });
            var result2 = client1.ReadValues<ushort>("M0", 2);
            var value1 = result2.Value;

            var result3 = client1.WriteValues("D100", new ushort[] { 0x17FD, 0xFA14 });
            var result4 = client1.ReadValues<ushort>("D100", 2);
            var value2 = result4.Value;

            var result5 = client1.WriteValues("R100", new ushort[] { 0x17FD, 0xFA14 });
            var result6 = client1.ReadValues<ushort>("R100", 2);
            var value3 = result6.Value;
        }
    }
}
