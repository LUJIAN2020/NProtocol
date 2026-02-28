using NProtocol.Base;

namespace NProtocol.Extensions
{
    public static class ResultExtension
    {
        public static Result<T> ToResult<T>(this Result result, T value)
        {
            return new Result<T>(value)
            {
                SendData = result.SendData,
                ReceivedData = result.ReceivedData,
            };
        }
    }
}
