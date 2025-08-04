# 🚀ModbusClient Document

[中文](./README_CN.md)

## 🏝️Create
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

Read and write raw message records events. Performance-sensitive applications should not use them, as they will cause performance loss
```C#
client.LogReadWriteRaw += (driverId, rw, data) =>
{
};
```
* driverId：Driver ID, usually connection information
* rw：It reads and writes messages
* data：Byte raw message
## 🌏Connect
```C#
client.Connect();
```

## 🌐Disconnect
```C#
client.Close();
```

## 🌋Write/Read
* Function Code 01 Read the coil register
```C#
var result = client.ReadCoils(1, 0, 1);
var value = result.Value;
```
* Function Code 02 Read the discrete input register
```C#
var result = client.ReadDiscreteInputs(1, 0, 1);
var value = result.Value;
```
* Function Code 03 Read the holding register
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
* Function Code 04 Read input registers
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
* Function Code 05 Write a single coil register
```C#
var result = client.WriteSingleCoil(1, 0, true);
```
* Function Code 06 Write a single register
```C#
var result = client.WriteSingleRegister(1, 0, 0x31FF);
```
* Function Code 15 Write multiple coil registers
```C#
var result = client.WriteMultipleCoils(1, 0, new bool[] { true, false });
```
* Function Code 16 Write to multiple registers
```C#
var result = client.WriteMultipleRegisters(1, 0, new ushort[] { 0x41F5, 0xF412 });
```
* Function Code 23 Read/write multiple registers
```C#
var result = client.ReadWriteMultipleRegisters(1, 0, 2, 0, new ushort[] { 0x41F5, 0x1234 });
var value = result.Value;
```
## ByteFormat
Function Code 03/04 Read data. Different manufacturers have different byte order, and the byte order is converted according to the actual read data. The following byte order formats are included
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