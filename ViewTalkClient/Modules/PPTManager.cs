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
    public class PPTManager
    {
        public PPTManager()
        {

        }

        public List<byte[]> ConvertPPT(string filePath)
        {
            List<byte[]> result = new List<byte[]>();

            try
            {
                Application application = new Application();
                Presentation presentation = application.Presentations.Open(filePath, MsoTriState.msoCTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);

                for (int i = 0; i < presentation.Slides.Count; i++)
                {
                    string imagePath = AppDomain.CurrentDomain.BaseDirectory + @"\ViewTalk_PPT";

                    DirectoryInfo directoryInfo = new DirectoryInfo(imagePath);
                    if (directoryInfo.Exists == false)
                    {
                        directoryInfo.Create();
                    }

                    imagePath += string.Format(@"\ViewTalk_PPT_{0:000}.jpg", i);

                    //presentation.Slides[i + 1].Export(imagePath, "JPG", (int)presentation.Slides[i + 1].Master.Width, (int)presentation.Slides[i + 1].Master.Height);
                    presentation.Slides[i + 1].Export(imagePath, "JPG", 320, 240);

                    result.Add(ConvertImageToByte(imagePath));
                }

                presentation.Close();
                application.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return result;
        }

        public byte[] ConvertImageToByte(string filePath)
        {
            byte[] result = new byte[0];

            try
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));

                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (MemoryStream memoryStreamm = new MemoryStream())
                {
                    jpegBitmapEncoder.Save(memoryStreamm);
                    result = memoryStreamm.ToArray();

                    memoryStreamm.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }
    }
}
