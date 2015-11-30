using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Media.Imaging;

using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace ViewTalkClient.Modules
{
    public class PowerPoint
    {
        private Application application;

        public PowerPoint()
        {
            this.application = new Application();
        }

        public List<byte[]> ConvertPPT(string filePath)
        {
            List<byte[]> result = new List<byte[]>();

            try
            { 
                Presentation presentation = application.Presentations.Open(filePath, MsoTriState.msoCTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);

                for (int i = 0; i < presentation.Slides.Count; i++)
                {
                    string imagePath = AppDomain.CurrentDomain.BaseDirectory + @"\PPT";

                    DirectoryInfo directoryInfo = new DirectoryInfo(imagePath);
                    if (directoryInfo.Exists == false)
                    {
                        directoryInfo.Create();
                    }

                    imagePath += string.Format(@"\ViewTalk_PPT_{0:000}.jpg", i);

                    //presentation.Slides[i + 1].Export(imagePath, "JPG", (int)presentation.Slides[i + 1].Master.Width, (int)presentation.Slides[i + 1].Master.Height);
                    presentation.Slides[i + 1].Export(imagePath, "JPG", 320, 240);

                    result.Add(ImageToByte(imagePath));
                }

                presentation.Close();
                application.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public byte[] ImageToByte(string imagePath)
        {
            byte[] result = new byte[0];

            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (MemoryStream memoryStreamms = new MemoryStream())
            {
                jpegBitmapEncoder.Save(memoryStreamms);
                result = memoryStreamms.ToArray();
            }

            return result;
        }
    }
}
