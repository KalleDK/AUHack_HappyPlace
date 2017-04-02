using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HarrasBlockerApp.Annotations;

namespace HarrasBlockerApp
{
    /// <summary>
    /// Interaction logic for AngerManagementWindow.xaml
    /// </summary>
    public partial class AngerManagementWindow : Window, INotifyPropertyChanged
    {
        private bool _storeAngryImages;

        public bool StoreAngryImages
        {
            get { return _storeAngryImages; }
            set
            {
                _storeAngryImages = value;
                PropertyChanged(this, new PropertyChangedEventArgs("StoreAngryImages"));
            }
        }

        private bool _killPC;

        public bool KillPC
        {
            get { return _killPC; }
            set
            {
                _killPC = value;
                PropertyChanged(this, new PropertyChangedEventArgs("KillPC"));
            }
        }

        public enum moodStates
        {
            Happy,
            Furious
        }

        private BitmapImage _bmp;

        public BitmapImage BMP
        {
            get { return _bmp; }
            set
            {
                _bmp = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BMP"));
            }
        }
        private moodStates _mood = moodStates.Happy;

        public moodStates Mood
        {
            get { return _mood; }
            set
            {
                _mood = value;
                if (_mood == moodStates.Happy)
                {
                    Dispatcher.Invoke(() =>
                    {
                        moodTextBlock.Foreground = new SolidColorBrush(Colors.White);
                        moodTextBlock.Background = new SolidColorBrush(Colors.DarkGreen);
                    });
                }
                else
                {
                    if (KillPC)
                    {
                        var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        Process.Start(psi);
                    }
                    Dispatcher.Invoke(() =>
                    {
                        moodTextBlock.Foreground = new SolidColorBrush(Colors.White);
                        moodTextBlock.Background = new SolidColorBrush(Colors.DarkRed);
                    });
                }
                    
                PropertyChanged(this, new PropertyChangedEventArgs("Mood"));
            }
            
        }

        private Dictionary<string, float> _emotionWeights;

        public Dictionary<string, float> EmotionWeights
        {
            get { return _emotionWeights; }
            set
            {
                _emotionWeights = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EmotionWeights"));
            }
        }
        public AngerManagementWindow()
        {
            InitializeComponent();
            _emotionWeights = new Dictionary<string, float>();
            SetEmotionalBaggage();
        }

        private void SetEmotionalBaggage()
        {
            _emotionWeights.Add("Anger", 15);
            _emotionWeights.Add("Contempt", 10f);
            _emotionWeights.Add("Disgust", 10f);
            _emotionWeights.Add("Fear", 3f);
            _emotionWeights.Add("Happiness", -1);
            _emotionWeights.Add("Neutral", 0);
            _emotionWeights.Add("Sadness", 3f);
            _emotionWeights.Add("Surprise", 0.1f);
            angerSlider.Value = 15;
            contemptSlider.Value = 10;
            disgustSlider.Value = 10;
            fearSlider.Value = 3;
            happinessSlider.Value = -1;
            neutralSlider.Value = 0;
            sadnessSlider.Value = 3;
            surpriseSlider.Value = 0.1;

        }

        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void disgustSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Disgust"] = (float)e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        public delegate void SettingsChangedEventHandler(object sender);

        public event SettingsChangedEventHandler SettingsChanged = delegate {};

        private void contemptSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Contempt"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void angerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Anger"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void neutralSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Neutral"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void sadnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Sadness"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void fearSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Fear"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void happinessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Fear"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }

        private void surpriseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _emotionWeights["Surprise"] = (float) e.NewValue;
            SettingsChanged(_emotionWeights);
        }
    }
}
