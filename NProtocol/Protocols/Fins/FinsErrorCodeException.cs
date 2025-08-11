using System.Collections.Generic;
using NProtocol.Exceptions;

namespace NProtocol.Protocols.Fins
{
    public class FinsErrorCodeException : ReceivedException
    {
        public FinsErrorCodeException(
            ushort code,
            byte[] sendData,
            byte[] receiveData,
            string driverId
        )
            : base(GetErrorMessage(code), sendData, receiveData, driverId) { }

        public static string GetErrorMessage(ushort code)
        {
            if (FrameErrorCodeValues.TryGetValue(code, out var msg))
            {
                return $"ErrorCode:{code},ErrorMessage:{msg}";
            }
            return "Unknow error";
        }

        private static IReadOnlyDictionary<ushort, string> FrameErrorCodeValues =>
            new Dictionary<ushort, string>()
            {
                // Completed successfully
                { 0x0000, "Completed successfully" },
                { 0x0001, "Service has been canceled" },
                // Local node errors
                { 0x0101, "Local node is not on the network" },
                { 0x0102, "Token timeout" },
                { 0x0103, "Retry failed" },
                { 0x0104, "Too many frames sent" },
                { 0x0105, "Node address range error" },
                { 0x0106, "Duplicate node address" },
                // Target node errors
                { 0x0201, "Target node is not on the network" },
                { 0x0202, "Unit missing" },
                { 0x0203, "Third node missing" },
                { 0x0204, "Target node is busy" },
                { 0x0205, "Response timeout" },
                // Controller errors
                { 0x0301, "Communication controller error" },
                { 0x0302, "CPU unit error" },
                { 0x0303, "Controller error" },
                { 0x0304, "Unit number error" },
                // Service not supported
                { 0x0401, "Undefined command" },
                { 0x0402, "Model/version not supported" },
                // Routing table errors
                { 0x0501, "Target address setting error" },
                { 0x0502, "No routing table" },
                { 0x0503, "Routing table error" },
                { 0x0504, "Too many relays" },
                // Command format errors
                { 0x1001, "Command too long" },
                { 0x1002, "Command too short" },
                { 0x1003, "Elements/data mismatch" },
                { 0x1004, "Command format error" },
                { 0x1005, "Header error" },
                // Parameter errors
                { 0x1101, "Region classification missing" },
                { 0x1102, "Access size error" },
                { 0x1103, "Address range error" },
                { 0x1104, "Out of address range" },
                { 0x1106, "Program missing" },
                { 0x1109, "Relation error" },
                { 0x110A, "Data access duplicated" },
                { 0x110B, "Response too long" },
                { 0x110C, "Parameter error" },
                // Cannot read
                { 0x2002, "Protected" },
                { 0x2003, "Table missing" },
                { 0x2004, "Data missing" },
                { 0x2005, "Program missing" },
                { 0x2006, "File missing" },
                { 0x2007, "Data mismatch" },
                // Cannot write
                { 0x2101, "Read-only" },
                { 0x2102, "Protected, cannot write data link table" },
                { 0x2103, "Cannot register" },
                { 0x2105, "Program missing" },
                { 0x2106, "File missing" },
                { 0x2107, "File name already exists" },
                { 0x2108, "Cannot change" },
                // Cannot execute in current mode
                { 0x2201, "Cannot perform during execution" },
                { 0x2202, "Cannot be implemented at runtime" },
                { 0x2203, "PLC mode error" },
                { 0x2204, "PLC mode error" },
                { 0x2205, "PLC mode error" },
                { 0x2206, "PLC mode error" },
                { 0x2207, "Specified node is not a polling node" },
                { 0x2208, "Cannot execute step" },
                // No such device
                { 0x2301, "File device missing" },
                { 0x2302, "Memory missing" },
                { 0x2303, "Clock missing" },
                // Cannot start/stop
                { 0x2401, "Table missing" },
                // Unit errors
                { 0x2502, "Memory error" },
                { 0x2503, "I/O setting error" },
                { 0x2504, "Too many I/O points" },
                { 0x2505, "CPU bus error" },
                { 0x2506, "I/O duplication" },
                { 0x2507, "I/O bus error" },
                { 0x2509, "SYMACBUS/2 error" },
                { 0x250A, "CPU bus unit error" },
                { 0x250D, "SYSMAC BUS number duplication" },
                { 0x250F, "Memory error" },
                { 0x2510, "SYSMAC BUS terminator" },
                // Command errors
                { 0x2601, "No protection" },
                { 0x2602, "Incorrect password" },
                { 0x2604, "Protected" },
                { 0x2605, "Service already executing" },
                { 0x2606, "Service has stopped" },
                { 0x2607, "No execution permission" },
                { 0x2608, "Settings incomplete" },
                { 0x2609, "Required items not set" },
                { 0x260A, "Defined number" },
                { 0x260B, "Error not cleared" },
                // Access permission error
                { 0x3001, "No access permission" },
                // Termination
                { 0x4001, "Service has been aborted" },
            };
    }
}
