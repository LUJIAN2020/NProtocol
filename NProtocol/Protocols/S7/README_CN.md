# 🚀S7Client文档

[English](./README.MD)

## 🌞S7Client介绍
S7Client 是一款PLC驱动程序，仅适用于西门子PLC和以太网连接。这意味着您的PLC必须具有 Profinet CPU 或 profinet 外部卡（CPxxx 卡）。 S7Client 完全是用 C# 编写的，因此可以轻松调试它，而无需通过本机 dll 。

## 🏖️支持的 PLC
S7-200、S7-300、S7-400、S7-1200、S7-1500 。

## 🏝️创建
```C#
S7Client s7 = new S7Client("192.168.244.141", 102, CpuType.S71500, 0, 1);
```
* ip：指定CPU或外部以太网卡的IP地址
* port：指定CPU端口号，默认：102
* cpuType：指定您要连接的 CPU 类型
* rack：PLC 的机架号，默认：0
* slot：CPU 的插槽号，默认：1

读写原始报文记录事件，性能敏感应用不要使用，会用性能损失
```C#
s7.LogReadWriteRaw += (driverId, rw, data) =>
{
};
```
* driverId：驱动设备Id,一般为连接信息
* rw：是读、写报文
* data：字节数组原始报文

## 🌏连接
```C#
s7.Connect();
s7.EstablishConnection();
```

## 🌐断开连接
```C#
s7.Close();
```

## 🌋读/写
* Bool
```C#
var result1 = s7.ReadBoolean("DB1.DBX0");
var value1 = result1.Value;
var result2 = s7.ReadBooleans("DB1.DBX0", 2);
var value2 = result2.Value;

var result3 = s7.WriteBoolean("DB1.DBX0", true);
```
* Byte
```C#
var result1 = s7.ReadByte("DB1.DBB1");
var value1 = result1.Value;
var result2 = s7.ReadBytes("DB1.DBB1",2);
var value2 = result2.Value;

var result3 = s7.WriteBytes("DB1.DBB1", 0xFE);
var result4 = s7.WriteBytes("DB1.DBB1", new byte[] { 0xFE, 0xEF });
```
* Int(int16)
```C#
var result1 = s7.ReadInt("DB1.DBB16");
var value1 = result1.Value;
var result2 = s7.ReadInts("DB1.DBB16", 2);
var value2 = result2.Value;

var result3 = s7.WriteInts("DB1.DBB16", 0x1288, 0x1F12);
var result4 = s7.WriteInts("DB1.DBB16", new short[] { 0x1288, 0x1F12 });
```
* Word(uint16)
```C#
var result1 = s7.ReadWord("DB1.DBW4");
var value1 = result1.Value;
var result2 = s7.ReadWords("DB1.DBW4", 2);
var value2 = result2.Value;

var result3 = s7.WriteWords("DB1.DBW4", 0x1311);
var result4 = s7.WriteWords("DB1.DBW4", new ushort[] { 0x1311, 0x11EF });
```
* DInt(int32)
```C#
var result1 = s7.ReadDInt("DB1.DBB20");
var value1 = result1.Value;
var result2 = s7.ReadDInts("DB1.DBB20", 2);
var value2 = result2.Value;

var result3 = s7.WriteDInts("DB1.DBB20", 0x1FFF1288);
var result4 = s7.WriteDInts("DB1.DBB20", new int[] { 0x1FFF1288, 0x2FFF1F12 });
```
* DWord(uint32)
```C#
var result1 = s7.ReadDWord("DB1.DBD8");
var value1 = result1.Value;
var result2 = s7.ReadDWords("DB1.DBD8", 2);
var value2 = result2.Value;

var result3 = s7.WriteDWords("DB1.DBD8", 0xFE11FF88);
var result4 = s7.WriteDWords("DB1.DBD8", new uint[] { 0xFE11FF88, 0x823811EF });
```
* Real(float)
```C#
var result1 = s7.ReadReal("DB1.DBB28");
var value1 = result1.Value;
var result2 = s7.ReadReals("DB1.DBB28", 2);
var value2 = result2.Value;

var result3 = s7.WriteReals("DB1.DBB28", 3.141596f);
var result4 = s7.WriteReals("DB1.DBB28", new float[] { 3.141596f, -40.541f });
```
* LReal(double)
```C#
var result1 = s7.ReadLReal(S7MemoryAreaType.DataBlock, 1, 36);
var value = result1.Value;
var result2 = s7.ReadLReals(S7MemoryAreaType.DataBlock, 1, 36, 2);
var value = result2.Value;

var result3 = s7.WriteLReals(S7MemoryAreaType.DataBlock, 1, 36, 3.141596d);
var result4 = s7.WriteLReals(S7MemoryAreaType.DataBlock, 1, 36, new double[] { 3.141596d, -1545.144552d });
```
* Char(char ASCII)
```C#
var result1 = s7.ReadS7CharFromDataBlock(1, 52);
var value = result1.Value;
var result2 = s7.WriteS7CharFromDataBlock(1, 52, 'B');
```
* WChar(string BigEndianUnicode)
```C#
var result1 = s7.ReadS7WCharFromDataBlock(1, 54);
var value = result1.Value;
var result2 = s7.WriteS7WCharFromDataBlock(1, 54, "好");
```
* String(string ASCII)
```C#
var result1 = s7.ReadS7StringFromDataBlock(1, 56, 6);
var value = result1.Value;
var result2 = s7.WriteS7StringToDataBlock(1, 56, "abc123");
```
* WString(string BigEndianUnicode)
```C#
var result1 = s7.ReadS7WStringFromDataBlock(1, 312, 4);
var value = result1.Value;
var result2 = s7.WriteS7WStringToDataBlock(1, 312, "我很好呀");
```
* 结构体
```C#
var result1 = s7.ReadStruct<MyStruct>(1, 824);
var value = result1.Value;
var result2 = s7.WriteStruct(mystruct, 1, 824);

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
```

## 🚩注意：关于 smart200 的地址

|数据类型 | smart200地址 | 实际读写地址
|---|---|---
|Bool | V1.0 | DB1.DBX1.0
|Byte | VB1 | DB1.DBB1
|Int <br> Word  | VW2 | DB1.DBW2
|DInt <br> DWord <br> Real | VD4 | DB1.DBD4

## 🥁其他操作

* CPU时间操作
```
s7.SetCpuDateTime(DateTime.Now);

var result = s7.GetCpuDateTime();
DateTime time = result.Value;
```

* 获取CPU状态
```C#
var result = s7.ReadCpuStatus();
CpuStatus status = result.Value;
```

* 获取CPU模块信息
```C#
var result = s7.GetCpuModelInfo();
var status = result.Value;
```

* 获取CPU组件信息
```C#
var result = s7.GetCpuComponentInfo();
var status = result.Value;
```

* 获取CPU通讯能力信息
```C#
var result = s7.GetCpuCommunicationCapabilityInfo();
var status = result.Value;
```

