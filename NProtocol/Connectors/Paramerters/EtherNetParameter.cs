namespace NProtocol.Communication.Connectors
{
    public class EtherNetParameter : IParameter
    {
        public string IP { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 502;
        public ParameterType Type => ParameterType.EnterNet;
        public static EtherNetParameter Create(string ip, ushort port)
        {
            return new EtherNetParameter
            {
                IP = ip,
                Port = port
            };
        }
    }
}