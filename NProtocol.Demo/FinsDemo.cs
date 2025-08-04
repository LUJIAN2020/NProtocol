using NProtocol.Protocols.Fins;

namespace NProtocol.Demo
{
    public class FinsDemo
    {
        public static void Run()
        {
            //FinsClient client = new FinsClient("192.168.244.130", 9600, FinsConnectMode.FinsTcp) { ReadTimeout = 5000 };
            //client.Connect();
            //client.SendHandshakeCommand();
            //Execute(client);


            FinsClient client = new FinsClient("192.168.244.130", 9600, FinsConnectMode.FinsUdp) { ReadTimeout = 5000 };
            client.Connect();
            Execute(client);
        }
        static void Execute(FinsClient client)
        {
            var result1 = client.Write("CIO10.1", new bool[] { true, false, true, true, true, true, false, true, false, true });
            var result2 = client.Read<bool>("CIO10.1", 50);
            var value2 = result2.Value;

            var result3 = client.Write("D10", new ushort[] { 0x1478, 0x71fa, });
            var result4 = client.Read<ushort>("D10", 2);
            var value4 = result2.Value;

            var result5 = client.Write("W10", new ushort[] { 0x1478, 0x71fa, });
            var result6 = client.Read<ushort>("W10", 2);
            var value6 = result2.Value;

            var result7 = client.Write("H10", new ushort[] { 0x1478, 0x71fa, });
            var result8 = client.Read<ushort>("H10", 2);
            var value8 = result2.Value;

            var result9 = client.Write("A10", new ushort[] { 0x1478, 0x71fa, });
            var result10 = client.Read<ushort>("A10", 2);
            var value10 = result2.Value;

            var result11 = client.Write("E10", new ushort[] { 0x1478, 0x71fa, });
            var result12 = client.Read<ushort>("E10", 2);
            var value12 = result2.Value;

            var result13 = client.Write("E08:00104", new ushort[] { 0x1478, 0x71fa, });
            var result14 = client.Read<ushort>("E08:00104", 2);
            var value14 = result2.Value;

            var result15 = client.Write("E08:00104.06", new bool[] { true, false, true, true, true, true, false, true, false, true });
            var result16 = client.Read<bool>("E08:00104.06", 50);
            var value16 = result2.Value;
        }
    }
}
