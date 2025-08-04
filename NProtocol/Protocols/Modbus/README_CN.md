# 🚀ModbusClient文档

[English](./README.MD)

## 🏝️创建
* Modbus-Rtu
```C#
ModbusClient client = new ModbusClient("COM1", 9600, 8, Parity.None, StopBits.One);
```
* Modbus-RtuOverTcp
```C#
ModbusClient client = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.RtuOverTcp);
```
* Modbus-RtuOverUdp
```C#
ModbusClient client = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.RtuOverUdp);
```
* Modbus-Tcp
```C#
ModbusClient client = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.Tcp);
```
* Modbus-Udp
```C#
ModbusClient client = new ModbusClient("192.168.244.130", 502, ModbusConnectMode.Udp);
```

读写原始报文记录事件，性能敏感应用不要使用，会用性能损失
```C#
client.LogReadWriteRaw += (driverId, rw, data) =>
{
};
```
* driverId：驱动设备Id,一般为连接信息
* rw：是读、写报文
* data：字节数组原始报文
## 🌏连接
```C#
client.Connect();
```

## 🌐断开连接
```C#
client.Close();
```

## 🌋读/写
* 功能码 01 读线圈寄存器
```C#
var result = client.ReadCoils(1, 0, 1);
var value = result.Value;
```
* 功能码 02 读离散输入寄存器
```C#
var result = client.ReadDiscreteInputs(1, 0, 1);
var value = result.Value;
```
* 功能码 03 读保持寄存器
```C#
var result1 = client.ReadHoldingRegister<short>(1, 0, ByteFormat.AB);
var value1 = result1.Value;

var result2 = client.ReadHoldingRegister<ushort>(1, 0, ByteFormat.AB);
var value2 = result2.Value;

var result3 = client.ReadHoldingRegister<int>(1, 0, ByteFormat.ABCD);
var value3 = result3.Value;

var result4 = client.ReadHoldingRegister<uint>(1, 0, ByteFormat.ABCD);
var value4 = result4.Value;

var result5 = client.ReadHoldingRegister<long>(1, 0, ByteFormat.ABCDEFGH);
var value5 = result5.Value;

var result6 = client.ReadHoldingRegister<ulong>(1, 0, ByteFormat.ABCDEFGH);
var value6 = result6.Value;

var result7 = client.ReadHoldingRegister<float>(1, 0, ByteFormat.ABCD);
var value7 = result7.Value;

var result8 = client.ReadHoldingRegister<double>(1, 0, ByteFormat.ABCDEFGH);
var value8 = result8.Value;
```
* 功能码 04 读输入寄存器
```C#
var result1 = client.ReadInputRegister<short>(1, 0, ByteFormat.AB);
var value1 = result1.Value;

var result2 = client.ReadInputRegister<ushort>(1, 0, ByteFormat.AB);
var value2 = result2.Value;

var result3 = client.ReadInputRegister<int>(1, 0, ByteFormat.ABCD);
var value3 = result3.Value;

var result4 = client.ReadInputRegister<uint>(1, 0, ByteFormat.ABCD);
var value4 = result4.Value;

var result5 = client.ReadInputRegister<long>(1, 0, ByteFormat.ABCDEFGH);
var value5 = result5.Value;

var result6 = client.ReadInputRegister<ulong>(1, 0, ByteFormat.ABCDEFGH);
var value6 = result6.Value;

var result7 = client.ReadInputRegister<float>(1, 0, ByteFormat.ABCD);
var value7 = result7.Value;

var result8 = client.ReadInputRegister<double>(1, 0, ByteFormat.ABCDEFGH);
var value8 = result8.Value;
```
* 功能码 05 写单线圈寄存器
```C#
var result = client.WriteSingleCoil(1, 0, true);
```
* 功能码 06 写单个寄存器
```C#
var result = client.WriteSingleRegister(1, 0, 0x31FF);
```
* 功能码 15 写多个线圈寄存器
```C#
var result = client.WriteMultipleCoils(1, 0, new bool[] { true, false });
```
* 功能码 16 写多个寄存器
```C#
var result = client.WriteMultipleRegisters(1, 0, new ushort[] { 0x41F5, 0xF412 });
```
* 功能码 23 读/写多个寄存器
```C#
var result = client.ReadWriteMultipleRegisters(1, 0, 2, 0, new ushort[] { 0x41F5, 0x1234 });
var value = result.Value;
```
## ByteFormat
功能码 03/04 读数据，不同厂家对字节序有所不同，根据实际读到的数据进行字节序转换。包含以下字节序格式
```C#
public enum ByteFormat : byte
{
    /// <summary>
    /// Int16,UInt16
    /// </summary>
    AB,
    /// <summary>
    /// Int32,UInt32,Float
    /// </summary>
    ABCD,
    CDAB,
    BADC,
    DCBA,
    /// <summary>
    /// Int64,UInt64,Double
    /// </summary>
    ABCDEFGH,
    GHEFCDAB,
    BADCFEHG,
    HGFEDCBA,
}
```