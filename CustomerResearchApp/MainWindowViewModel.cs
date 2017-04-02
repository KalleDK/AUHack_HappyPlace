using CustomerResearchApp.ViewModels;

namespace CustomerResearchApp
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly WebCamViewModel _webCamViewModel = new WebCamViewModel();
        private BindableBase _currentViewModel;

        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                SetProperty(ref _currentViewModel, value);
            }
        }
        public MainWindowViewModel()
        {
            CurrentViewModel = _webCamViewModel;
        }
    }
}
