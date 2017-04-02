using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Accord.Video;
using Accord.Video.DirectShow;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Threading;

namespace HappyPlace_Console
{
    class Program
    {
        private static VideoCaptureDevice videoSource;
        static EmotionService emotionService = new EmotionService();
        private static List<Task<Emotion[]>> _emotionTasks = new List<Task<Emotion[]>>();
        static void Main(string[] args)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += NewFrameHandler;
            videoSource.Start();

            Console.WriteLine("Press A to start.");
            ConsoleKeyInfo pressedKey = Console.ReadKey();
            while (pressedKey.Key != ConsoleKey.Q)
            {
                Console.WriteLine("A = videoSource.Start ----- S = videoSource.SignalToStop");
                pressedKey = Console.ReadKey();
                switch (pressedKey.Key)
                {
                    case ConsoleKey.A:
                        videoSource.Start();
                        break;

                    case ConsoleKey.S:
                        videoSource.SignalToStop();
                        break;
                }
            }
        }

        static async void NewFrameHandler(object sender, NewFrameEventArgs args)
        {
            videoSource.SignalToStop();
            try
            {
                Bitmap bitmap = args.Frame;
                bitmap.Save(@"test.jpg");
                var task = emotionService.UploadAndProcess(@"test.jpg");
                _emotionTasks.Add(task);
                Console.WriteLine("Current nr. of running tasks: " + _emotionTasks.Count);
                var results = await task;
                foreach (var emotion in results)
                {
                    var ranking = emotion.Scores.ToRankedList();
                    Console.WriteLine(ranking.ElementAtOrDefault(0));
                    Console.WriteLine(ranking.ElementAtOrDefault(1));
                }
                await Task.Delay(3000);
                videoSource.Start();
            }
            catch (Microsoft.ProjectOxford.Common.ClientException e)
            {
                Console.WriteLine("Shit Crashed bro..");
                foreach (var emotionTask in _emotionTasks)
                {
                    if(emotionTask.Status != TaskStatus.RanToCompletion)
                        emotionTask.Dispose();
                    Console.WriteLine(emotionTask.Status);
                }
                _emotionTasks.Clear();
            }
        }
    }
}
