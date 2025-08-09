# 🚀S7Client Document

[中文](./README_CN.md)

## 🌞S7Client Introduce
The S7Client is a PLC driver specifically designed for Siemens PLCs with Ethernet connectivity. This requires your PLC to have either a Profinet CPU or Profinet external card (CPxxx card). Being fully written in C#, the S7Client allows easy debugging without requiring native DLLs.
## 🏖️Supported PLC models
S7-200、S7-300、S7-400、S7-1200、S7-1500 。

## 🏝️Create
```C#
S7Client s7 = new S7Client("192.168.244.141", 102, CpuType.S71500, 0, 1);
```
* ip：Specify the IP address of the CPU or external Ethernet card
* port：Specifies the CPU port number. default: 102
* cpuType：Specify the type of CPU you want to connect to
* rack：The rack number of the PLC is 0 by default
* slot：CPU slot number, default: 1

Read and write raw message records events. Performance-sensitive applications should not use them, as they will cause performance loss
```C#
s7.LogReadWriteRaw += (driverId, rw, data) =>
{
};
```
* driverId：Driver ID, usually connection information
* rw：It reads and writes messages
* data：Byte raw message

## 🌏Connect
```C#
s7.Connect();
s7.EstablishConnection();
```

## 🌐Disconnect
```C#
s7.Close();
```

## 🌋Write/Read
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
* Struct
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

## 🚩Note: About the address of smart200

|Data Type | smart200 Address | Actual read/write addresses
|---|---|---
|Bool | V1.0 | DB1.DBX1.0
|Byte | VB1 | DB1.DBB1
|Int <br> Word  | VW2 | DB1.DBW2
|DInt <br> DWord <br> Real | VD4 | DB1.DBD4

## 🥁Other operations

* CPU DateTime
```
s7.SetCpuDateTime(DateTime.Now);

var result = s7.GetCpuDateTime();
DateTime time = result.Value;
```

* Read CPU Status
```C#
var result = s7.ReadCpuStatus();
CpuStatus status = result.Value;
```

* Get CPU model info
```C#
var result = s7.GetCpuModelInfo();
var status = result.Value;
```

*Get CPU component info
```C#
var result = s7.GetCpuComponentInfo();
var status = result.Value;
```

* Get CPU communication capability info
```C#
var result = s7.GetCpuCommunicationCapabilityInfo();
var status = result.Value;
```