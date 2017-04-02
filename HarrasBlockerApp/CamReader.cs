using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Accord.Video;
using Accord.Video.DirectShow;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;

namespace HarrasBlockerApp
{
    public class CamReader
    {
        private readonly EmotionServiceClient _emoClient;

        public CamReader()
        {
            _emoClient = new EmotionServiceClient(Properties.Settings.Default.emotionAPIKey);
            _testResults = new List<Emotion>();
            InitializeCamera();
        }

        private VideoCaptureDevice _webcam;

        public VideoCaptureDevice Webcam
        {
            get { return _webcam;}
            set { _webcam = value; }
        }

        private List<Emotion> _testResults;

        public void InitializeCamera()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _webcam = new VideoCaptureDevice(videoDevices[0].MonikerString);
            _webcam.NewFrame += NewFrameHandler;
            
        }

        public async void NewFrameHandler(object sender, NewFrameEventArgs args)
        {
            _webcam.SignalToStop();
            using (Bitmap myFrame = args.Frame)
            {
                myFrame.Save("currentFrame.jpg");
                var results = await UploadAndAnalyze("currentFrame.jpg");
                foreach (var emotion in results)
                {
                    _testResults.Add(emotion);
                }
            }
           
        }

        public async Task<Emotion[]> UploadAndAnalyze(string filePath)
        {
            using (Stream imageFileStream = File.OpenRead(filePath))
            {
                var emotionResult = await _emoClient.RecognizeAsync(imageFileStream);
                return emotionResult;
            }
        }

        public void StoreResults()
        {
            using (StreamWriter file = File.CreateText("TestResults.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, _testResults);
            }
        }
        


    }
}
