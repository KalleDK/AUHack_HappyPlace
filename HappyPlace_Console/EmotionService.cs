using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace HappyPlace_Console
{
    public class EmotionService
    {
        private EmotionServiceClient _emotionServiceClient;

        public EmotionService()
        {
            _emotionServiceClient = new EmotionServiceClient(Properties.Settings.Default.apiKey);
        }

        public void RestartService()
        {
            _emotionServiceClient = new EmotionServiceClient(Properties.Settings.Default.apiKey);
        }
        /// <summary>
        /// Async method that uploads a given imagefile to the Project Oxford servers.
        /// Processed emotions are returned and stored in an Emotion array.
        /// </summary>
        /// <param name="filePath">String pointing to the image location on the disc</param>
        /// <seealso cref="Microsoft.ProjectOxford.Emotion"/>
        /// <returns>Array of emotions present in the given image</returns>
        public async Task<Emotion[]> UploadAndProcess(string filePath)
        {
            try
            {
                
                using (Stream imageFileStream = File.OpenRead(filePath))
                {
                    var emotionResult = await _emotionServiceClient.RecognizeAsync(imageFileStream);
                    return emotionResult;
                }
            }
            catch (Microsoft.ProjectOxford.Common.ClientException e)
            {
                //Can't find info on possible exceptions. Just catch, print and release
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
