using Avalonia.Controls;
using System;

namespace NProtocol.Demo.Models
{
    public class PageItem
    {
        public PageItem(string title, Type viewType, Type viewModelType)
        {
            Title = title;
            ViewType = viewType;
            ViewModelType = viewModelType;
            var view = Activator.CreateInstance(viewType);
            var vm = Activator.CreateInstance(viewModelType);
            if (view is UserControl uc)
            {
                uc.DataContext = vm;
                View = uc;
            }
        }
        public string Title { get; }
        public Type ViewType { get; }
        public Type ViewModelType { get; }
        public object? View { get; }
    }
}
