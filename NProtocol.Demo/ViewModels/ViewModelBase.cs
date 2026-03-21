using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using NProtocol.Demo.Models;
using System;

namespace NProtocol.Demo.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        public void Info(string message)
        {
            WeakReferenceMessenger.Default.Send(new LogMessage() { Message = message });
        }
        public void Error(Exception exception, string? message = default)
        {
            WeakReferenceMessenger.Default.Send(new LogMessage() { Message = message, Exception = exception });
        }
    }
}
