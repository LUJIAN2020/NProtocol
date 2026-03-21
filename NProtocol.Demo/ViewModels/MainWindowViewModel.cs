using CommunityToolkit.Mvvm.ComponentModel;
using NProtocol.Demo.Models;
using NProtocol.Demo.Services;

namespace NProtocol.Demo.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            PageItems = NavigateService.Default.PageItems;
            selectedPageItem = PageItems[0];
            var view = NavigateService.Default.NavigateToView(PageItems[0].Title);
            if (view != null)
            {
                CurrentView = view;
            }
        }
        public PageItem[] PageItems { get; }
        [ObservableProperty] private object? _currentView;
        private PageItem selectedPageItem;
        public PageItem SelectedPageItem
        {
            get { return selectedPageItem; }
            set
            {
                if (SetProperty(ref selectedPageItem, value))
                {
                    var view = NavigateService.Default.NavigateToView(value.Title);
                    if (view != null)
                    {
                        CurrentView = view;
                    }
                }
            }
        }
    }
}
