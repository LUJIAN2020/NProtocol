using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using NProtocol.Demo.Models;
using System;
using System.Text;

namespace NProtocol.Demo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Register();
        }
        private readonly StringBuilder sb = new();
        private void Register()
        {
            WeakReferenceMessenger.Default.Register<LogMessage>(this, (o, m) =>
            {
                if (m.Exception != null)
                {
                    if (m.Message != null)
                    {
                        sb.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | ERROR | {m.Message}\r\n{m.Exception.Message}\r\n");
                    }
                    else
                    {
                        sb.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | ERROR | {m.Exception.Message}\r\n");
                    }
                }
                else
                {
                    if (m.Message != null)
                    {
                        sb.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | INFO | {m.Message}\r\n");
                    }
                }
                logTextBox.Text = sb.ToString();
            });
        }
    }
}