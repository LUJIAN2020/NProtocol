using NProtocol.Protocols.Modbus;
using System.IO.Ports;

namespace NProtocol.Demo
{
    public class ModbusDemo
    {
        public static void Run()
        {
            //ModbusClient client = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.Tcp) { ReadTimeout = 5000 };
            //client.Connect();
            //ExecuteModbus(client);

            ModbusClient client1 = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.Udp) { ReadTimeout = 5000 };
            client1.Connect();
            ExecuteModbus(client1);
        }
        static void ExecuteModbus(ModbusClient client)
        {
            var result1 = client.WriteSingleCoil(1, 0, true);
            var resilt2 = client.ReadCoils(1, 0, 1);
            Console.WriteLine(resilt2.Value[0]);

            var result3 = client.WriteMultipleCoils(1, 0, new bool[] { true, false });
            var resilt4 = client.ReadCoils(1, 0, 2);
            var resilt41 = client.ReadDiscreteInputs(1, 0, 2);
            Console.WriteLine(string.Join(",", resilt4.Value));

            var result5 = client.WriteSingleRegister(1, 0, 0x31FF);
            var result6 = client.ReadHoldingRegister<ushort>(1, 0);

            Console.WriteLine(string.Join(",", result6.Value));

            var result7 = client.WriteMultipleRegisters(1, 0, new ushort[] { 0x41F5, 0xF412 });
            var result8 = client.ReadHoldingRegisters<ushort>(1, 0, 2);
            var result88 = client.ReadInputRegisters<ushort>(1, 0, 2);
            Console.WriteLine(string.Join(",", result8.Value));

            var result9 = client.ReadWriteMultipleRegisters(1, 0, 2, 0, new ushort[] { 0x41F5, 0x1234 });
            var value = result9.Value;
            Console.WriteLine(string.Join(",", result9.Value));

            var result10 = client.ReadInputRegister<short>(1, 0, ByteFormat.AB);
            var value1= result10.Value;
            var result11 = client.ReadInputRegister<ushort>(1, 0, ByteFormat.AB);
            var value2 = result10.Value;
            var result12 = client.ReadInputRegister<int>(1, 0, ByteFormat.ABCD);
            var value3 = result10.Value;
            var result13 = client.ReadInputRegister<uint>(1, 0, ByteFormat.ABCD);
            var value4 = result10.Value;
            var result14 = client.ReadInputRegister<long>(1, 0, ByteFormat.ABCDEFGH);
            var value5 = result10.Value;
            var result15 = client.ReadInputRegister<ulong>(1, 0, ByteFormat.ABCDEFGH);
            var value6 = result10.Value;
            var result16 = client.ReadInputRegister<float>(1, 0, ByteFormat.ABCD);
            var value7 = result10.Value;
            var result17 = client.ReadInputRegister<double>(1, 0, ByteFormat.ABCDEFGH);
            var value8 = result10.Value;

            var result18 = client.ReadHoldingRegister<short>(1, 0, ByteFormat.AB);
            var value9 = result10.Value;
            var result19 = client.ReadHoldingRegister<ushort>(1, 0, ByteFormat.AB);
            var result20 = client.ReadHoldingRegister<int>(1, 0, ByteFormat.ABCD);
            var result21 = client.ReadHoldingRegister<uint>(1, 0, ByteFormat.ABCD);
            var result22 = client.ReadHoldingRegister<long>(1, 0, ByteFormat.ABCDEFGH);
            var result23 = client.ReadHoldingRegister<ulong>(1, 0, ByteFormat.ABCDEFGH);
            var result24 = client.ReadHoldingRegister<float>(1, 0, ByteFormat.ABCD);
            var result25 = client.ReadHoldingRegister<double>(1, 0, ByteFormat.ABCDEFGH);
        }
    }
}
