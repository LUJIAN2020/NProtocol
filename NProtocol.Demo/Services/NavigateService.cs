using NProtocol.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NProtocol.Demo.Services
{
    public class NavigateService
    {
        public static NavigateService Default { get; set; } = new NavigateService();
        private NavigateService() { }
        private readonly Dictionary<string, PageItem> pages = new();
        public PageItem[] PageItems => pages.Select(c => c.Value).ToArray();
        public void RegisterPage(string title, Type viewType, Type viewModelType)
        {
            var pageItem = new PageItem(title, viewType, viewModelType);
            if (pages.ContainsKey(title))
            {
                pages[title] = pageItem;
            }
            else
            {
                pages.Add(title, pageItem);
            }
        }
        public object? NavigateToView(string name)
        {
            if (pages.TryGetValue(name, out var page))
            {
                return page.View;
            }
            return default;
        }
    }
}
