using CustomerResearchApp.Models;

namespace CustomerResearchApp.ViewModels
{
    public class WebCamViewModel : BindableBase
    {
        private CamReader _camReader;
        public CamReader CamReader
        {
            get { return _camReader; }
            set { SetProperty(ref _camReader, value); }
        }

        public WebCamViewModel()
        {
            _camReader = new CamReader();
        }

        private string _topEmotion;
        public string TopEmotion
        {
            get { return _topEmotion; }
            set
            {
                SetProperty(ref _topEmotion, value);
            }
        }
    }
}
