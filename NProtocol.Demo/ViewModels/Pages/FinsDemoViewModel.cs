using CommunityToolkit.Mvvm.ComponentModel;
using NProtocol.Connectors;
using NProtocol.Protocols.Fins;
using System;
using System.Linq;

namespace NProtocol.Demo.ViewModels.Pages
{
    public partial class FinsDemoViewModel : ViewModelBase
    {
        [ObservableProperty] private string _connectStatus = "Connect";
        [ObservableProperty] private string _value = string.Empty;
        public string[] Modes => new string[] { "TCP", "UDP" };
        private FinsClient? client;
        public void ConnectCommand(object[] args)
        {
            try
            {
                if (args[0] is string ip && args[1] is decimal port && args[2] is string mode)
                {
                    if (ConnectStatus == "Disconnect")
                    {
                        client?.Dispose();
                        client = null;
                        ConnectStatus = "Connect";
                    }
                    else if (ConnectStatus == "Connect")
                    {
                        var eth = new EtherNetParameter() { IP = ip, Port = (ushort)port };
                        client = new FinsClient(eth, mode == "TCP" ? FinsConnectMode.FinsTcp : FinsConnectMode.FinsUdp);
                        client.Connect();
                        Info("Connect success");
                        ConnectStatus = "Disconnect";
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        public void ReadCommand(object[] args)
        {
            try
            {
                if (args[0] is string addr && args[1] is decimal cnt)
                {
                    if (client != null)
                    {
                        var finsAddr = new FinsAddress(addr);
                        var result = client.ReadBytes(finsAddr, (ushort)cnt);
                        Value = string.Join(",", result.Value.Select(c => c.ToString("X2")));
                        Info(result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        public void WriteCommand(object[] args)
        {
            try
            {
                if (args[0] is string addr && args[1] is string value)
                {
                    if (client != null)
                    {
                        var vals = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var data = Array.ConvertAll(vals, Convert.ToUInt16);
                        var finsAddr = new FinsAddress(addr);
                        var result = client.Write(finsAddr, data);
                        Info(result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
    }
}
