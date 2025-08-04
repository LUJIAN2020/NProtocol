# 🚀FinsClient文档

[English](./README.MD)

## 🏝️创建
* FINS-TCP
```C#
FinsClient client = new FinsClient("192.168.244.130", 9600, FinsConnectMode.FinsTcp);
```
* FINS-UDP
```C#
FinsClient client = new FinsClient("192.168.244.130", 9600, FinsConnectMode.FinsUdp);
```
推荐使用**FINS-UDP**，无连接模式，现场使用更好管理

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
```C#
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
```