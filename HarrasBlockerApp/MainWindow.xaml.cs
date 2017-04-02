using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Video;
using Accord.Video.DirectShow;
using HarrasBlockerApp.Annotations;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace HarrasBlockerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _triggered;
        private Dictionary<string, float> _emotionWeights;
        private EmotionServiceClient _emotionClient;
        private VideoCaptureDevice _webcam;
        private Timer _imageTimer;
        private int refreshTime = 5000; //Time between data gathering in ms
        private IEnumerable<KeyValuePair<string, float>> _emotionRankings;
        private Dictionary<string, float> _emotionCatalogue;
        private List<float> _emotionResults = new List<float>();
        private AngerManagementWindow _amWindow;

        private float _shittyMoodThreshold = 0.10f;

        public float ShittyMoodTreshold
        {
            get { return _shittyMoodThreshold; }
            set
            {
                _shittyMoodThreshold = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ShittyMoodThreshold"));
            }
        }

        public MainWindow()
        {
            _triggered = false;
            InitializeCamera();
            _webcam.SetCameraProperty(CameraControlProperty.Iris, -100, CameraControlFlags.Auto);
            _amWindow = new AngerManagementWindow();
            _amWindow.Show();
            _amWindow.SettingsChanged += SettingsChangedHandler;
            _emotionWeights = new Dictionary<string, float>();
            _emotionClient = new EmotionServiceClient(Properties.Settings.Default.emotionAPIKey);
            _emotionCatalogue = new Dictionary<string, float>();
            _emotionRankings = new List<KeyValuePair<string, float>>();
            SetEmotionalBaggage();
            _webcam.NewFrame += NewFrameHandler;
            _imageTimer = new Timer(refreshTime);
            _imageTimer.Elapsed += GetEmotionData;
            _imageTimer.AutoReset = true;
            _imageTimer.Enabled = true;
            InitializeComponent();
            mainWindow.Hide();
        }

        private string _emotionalState;
        public string EmotionalState
        {
            get { return _emotionalState; }
            set
            {
                if (_emotionalState != value)
                {
                    _emotionalState = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("EmotionalState"));
                }
            }
        }

        private void CalculateEmotionalState()
        {
            float badMoodValue = 0.0f;
            float happinessValue = 0.0f;
            foreach (var emotion in _emotionRankings)
            {
                badMoodValue += emotion.Value * _emotionWeights[emotion.Key];
                if (emotion.Key == "Happiness")
                    happinessValue = emotion.Value;
            }

            if (badMoodValue > ShittyMoodTreshold)
            {
                _triggered = true;
                _amWindow.Mood = AngerManagementWindow.moodStates.Furious;
                EmotionalState = "Y u heff to be mad";
                BringWindowForward();
                if(_amWindow.StoreAngryImages)
                    StoreAngryImage();
            }
            else
            {
                if (_triggered)
                {
                    if (happinessValue > 0.85f)
                    {
                        _triggered = false;
                        EmotionalState = "Happy Man";
                        _amWindow.Mood = AngerManagementWindow.moodStates.Happy;
                        HideWindow();
                    }
                }
            }
        }

        private void HideWindow()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                mainWindow.Hide();
                mainWindow.Topmost = false;
            }));
        }

        private void BringWindowForward()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (!mainWindow.IsVisible)
                    mainWindow.Show();

                mainWindow.Width = 500;
                mainWindow.Height = 500;
                //mainWindow.Width = SystemParameters.VirtualScreenWidth;
                //mainWindow.Height = SystemParameters.VirtualScreenHeight;
                mainWindow.Left = (SystemParameters.VirtualScreenWidth / 2) - 250;
                mainWindow.Top = (SystemParameters.VirtualScreenHeight / 2) - 250;

                if (mainWindow.WindowState == WindowState.Minimized)
                    mainWindow.WindowState = WindowState.Maximized;

                mainWindow.Activate();
                mainWindow.Topmost = true;
                mainWindow.Focus();
                
            }));
        }

        private void GetEmotionData(object sender, ElapsedEventArgs args)
        {
            _webcam.Start();
            
        }

        private async void NewFrameHandler(object sender, NewFrameEventArgs args)
        { 
            
            _webcam.SignalToStop();

            Bitmap myFrame = args.Frame;
            myFrame.Save("../../Images/currentFrame.jpg");
            using (Stream imageFileStream = File.OpenRead("../../Images/currentFrame.jpg"))
            {
                var results = await _emotionClient.RecognizeAsync(imageFileStream);
                foreach (var emotion in results)
                {
                    _emotionRankings = emotion.Scores.ToRankedList();
                }
                CalculateEmotionalState();
            }
        }

        private void InitializeCamera()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _webcam = new VideoCaptureDevice(videoDevices[0].MonikerString);
        }

        private void SetEmotionalBaggage()
        {
            _emotionWeights.Add("Anger", 20);
            _emotionWeights.Add("Contempt", 10f);
            _emotionWeights.Add("Disgust", 20f);
            _emotionWeights.Add("Fear", 3f);
            _emotionWeights.Add("Happiness", -1);
            _emotionWeights.Add("Neutral", 0);
            _emotionWeights.Add("Sadness", 3f);
            _emotionWeights.Add("Surprise", 0.1f);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SettingsChangedHandler(object sender)
        {
            Dictionary<string, float> emoDictionary = sender as Dictionary<string, float>;
            foreach (var f in emoDictionary)
            {
                _emotionWeights[f.Key] = emoDictionary[f.Key];
            }
        }

        private void StoreAngryImage()
        {
            char[] separators = {'_', '.'};
            string fileLocation = "../../AngryPeople";
            string[] fileNames = Directory.GetFileSystemEntries(fileLocation, "*.jpg");
            int lastIndex = int.Parse(System.IO.Path.GetFileName(fileNames[fileNames.Length - 1]).Split(separators)[1]);
            lastIndex++;
            string newFileName;
            if (lastIndex < 10)
                newFileName = "angry_" + "0" + lastIndex.ToString() + ".jpg";
            else
                newFileName = "angry_" + lastIndex.ToString() + ".jpg";

            newFileName = System.IO.Path.Combine(fileLocation, newFileName);

            File.Copy("../../Images/currentFrame.jpg", newFileName);
        }
    }
}
