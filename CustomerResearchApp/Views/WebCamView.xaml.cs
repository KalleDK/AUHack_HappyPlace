using System;
using System.Reflection;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CustomerResearchApp.Models;

namespace CustomerResearchApp.Views
{
    /// <summary>
    /// Interaction logic for WebCamView.xaml
    /// </summary>
    public partial class WebCamView : UserControl
    {
        private int currentImageIndex = 0;
        private CamReader _camReader;
        public WebCamView()
        {
            InitializeComponent();
            Timer newTimer = new Timer(2000);
            newTimer.Elapsed += TimerEvent;
            newTimer.AutoReset = true;
            newTimer.Enabled = true;
            _camReader = new CamReader();
        }

        private void TimerEvent(object sender, ElapsedEventArgs args)
        {
            string folder = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Images";
            string imageFolder = System.IO.Path.Combine(folder, "\\Images");
            string[] imagePaths = System.IO.Directory.GetFiles(folder, "*.jpg");
            this.Dispatcher.Invoke(new Action(() =>
                {
                    if (currentImageIndex < imagePaths.Length)
                    {
                        Uri imageUri = new Uri(imagePaths[currentImageIndex]);
                        BitmapImage bmp = new BitmapImage(imageUri);
                        image.Source = bmp;
                        
                        currentImageIndex++;
                        GrabEmotions();
                    }
                    else
                    {
                        Timer timer = sender as Timer;
                        timer.Enabled = false;
                        _camReader.StoreResults();
                    }
                }
            ));
            
        }

        public void GrabEmotions()
        {
            _camReader.Webcam.Start();
        }
    }
}
