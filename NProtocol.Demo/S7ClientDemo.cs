using NProtocol.Communication.Base;
using NProtocol.Protocols.S7;
using NProtocol.Protocols.S7.Enums;
using NProtocol.Protocols.S7.StructType;
using NProtocol.Protocols.S7.StructType.S7DateTime;

namespace NProtocol.Demo
{
    internal class S7ClientDemo
    {
        public static void Run()
        {
            S7Client s7 = new S7Client("192.168.244.141", 102, CpuType.S71500, 0, 1) { ReadTimeout = 5000 };
            s7.Connect();
            s7.EstablishConnection();
            BooleanWR(s7);
            ByteWR(s7);
            WordWR(s7);
            IntWR(s7);
            RealWR(s7);
            StringWR(s7);
            StructWR(s7);
            s7.Close();
        }
        static void BooleanWR(S7Client s7)
        {
            var result1 = s7.WriteBoolean("DB1.DBX0", true);

            var result2 = s7.ReadBooleans("DB1.DBX0", 2);
            var value = result2.Value[0];
            Console.WriteLine(string.Join("-", result2.Value));
            var result3 = s7.ReadBoolean("DB1.DBX0");
            Console.WriteLine(result3.Value);
        }
        static void ByteWR(S7Client s7)
        {
            var result1 = s7.WriteBytes("DB1.DBB1", new byte[] { 0xFE, 0xEF });
            var result11 = s7.WriteBytes("DB1.DBB1", 0xFE);
            var result2 = s7.ReadBytes("DB1.DBB1", 2);
            Console.WriteLine(string.Join("-", result2.Value.Select(c => c.ToString("X2"))));
            var result3 = s7.ReadByte("DB1.DBB1");
            Console.WriteLine(result3.Value);
        }
        static void WordWR(S7Client s7)
        {
            var result1 = s7.WriteWords("DB1.DBW4", new ushort[] { 0x1311, 0x11EF });
            var result2 = s7.ReadWords("DB1.DBW4", 2);
            Console.WriteLine(string.Join("-", result2.Value.Select(c => c.ToString("X2"))));
            var result3 = s7.ReadWord("DB1.DBW4");

            var result4 = s7.WriteDWords("DB1.DBD8", new uint[] { 0xFE11FF88, 0x823811EF });
            var result5 = s7.ReadDWords("DB1.DBD8", 2);
            Console.WriteLine(string.Join("-", result5.Value.Select(c => c.ToString("X2"))));
            var result6 = s7.ReadDWord("DB1.DBD8");
        }
        static void IntWR(S7Client s7)
        {
            var result1 = s7.WriteInts("DB1.DBB16", new short[] { 0x1288, 0x1F12 });
            var result2 = s7.ReadInts("DB1.DBB16", 2);
            Console.WriteLine(string.Join("-", result2.Value.Select(c => c.ToString("X2"))));
            var result3 = s7.ReadInt("DB1.DBB16");

            var result4 = s7.WriteDInts("DB1.DBB20", new int[] { 0x1FFF1288, 0x2FFF1F12 });
            var result5 = s7.ReadDInts("DB1.DBB20", 2);
            Console.WriteLine(string.Join("-", result5.Value.Select(c => c.ToString("X2"))));
            var result6 = s7.ReadDInt("DB1.DBB20");
        }
        static void RealWR(S7Client s7)
        {
            var result1 = s7.WriteReals("DB1.DBB28", new float[] { 3.141596f, -40.541f });
            var result2 = s7.ReadReals("DB1.DBB28", 2);
            Console.WriteLine(string.Join("-", result2.Value));
            var result3 = s7.ReadReal("DB1.DBB28");


            var result4 = s7.WriteLReals(S7MemoryAreaType.DataBlock, 1, 36, new double[] { 3.141596d, -1545.144552d });
            var result5 = s7.ReadLReals(S7MemoryAreaType.DataBlock, 1, 36, 2);
            Console.WriteLine(string.Join("-", result5.Value));
            var result6 = s7.ReadLReal(S7MemoryAreaType.DataBlock, 1, 36);
        }
        static void StringWR(S7Client s7)
        {
            var result1 = s7.WriteS7CharFromDataBlock(1, 52, 'B');
            var result2 = s7.ReadS7CharFromDataBlock(1, 52);

            var result3 = s7.WriteS7WCharFromDataBlock(1, 54, "好");
            var result4 = s7.ReadS7WCharFromDataBlock(1, 54);


            var result5 = s7.WriteS7StringToDataBlock(1, 56, "abc123");
            var result6 = s7.ReadS7StringFromDataBlock(1, 56, 6);

            var result7 = s7.WriteS7WStringToDataBlock(1, 312, "我很好呀");
            var result8 = s7.ReadS7WStringFromDataBlock(1, 312, 4);
        }
        static void StructWR(S7Client s7)
        {
            var mystruct = new MyStruct();
            var result1 = s7.WriteStruct(mystruct, 1, 824);
            var result2 = s7.ReadStruct<MyStruct>(1, 824);
        }
    }


    public struct MyStruct
    {
        public MyStruct()
        {
        }
        public bool Value1 = true;
        public byte Value2 = 214;
        public ushort Value3 = 4751;
        public uint Value4 = 4515347;
        public short Value5 = -1245;
        public int Value6 = -15624563;
        public float Value7 = 3.141596f;
        public double Value8 = 1245.1455d;

        [S7Time(S7TimeType.Time)]
        public TimeSpan Value9 = TimeSpan.FromSeconds(10);

        [S7Time(S7TimeType.LTime)]
        public TimeSpan Value10 = TimeSpan.FromSeconds(10);

        [S7Time(S7TimeType.TimeOfDay)]
        public TimeSpan Value11 = TimeSpan.FromSeconds(10);

        [S7Time(S7TimeType.LTimeOfDay)]
        public TimeSpan Value12 = TimeSpan.FromSeconds(10);

        [S7DateTime(S7DateTimeType.Date)]
        public DateTime Value13 = DateTime.Now;

        [S7DateTime(S7DateTimeType.DateAndTime)]
        public DateTime Value14 = DateTime.Now;

        [S7DateTime(S7DateTimeType.DTL)]
        public DateTime Value15 = DateTime.Now;

        [S7String(S7StringType.S7String, 5)]
        public string Value16 = "12351";

        [S7String(S7StringType.S7WString, 2)]
        public string Value17 = "好呀";
    }
}
