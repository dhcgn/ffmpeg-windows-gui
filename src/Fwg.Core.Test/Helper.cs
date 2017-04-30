using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Fwg.Core.Test
{
    public class Helper
    {
        public static Color GetTestImageColor(string file)
        {
            Color pixelColor;
            using (Bitmap myBitmap = new Bitmap(file))
            {
                pixelColor = myBitmap.GetPixel(10, 10);
            }
            return pixelColor;
        }


        public static TestFiles.SlideEnum DetermineSlide(TimeSpan position, string file)
        {
            var directory = TestContext.CurrentContext.TestDirectory;
            var preparer = new PowershellPreparer($@"{directory}\..\..\FFmpegStatic\ffmpeg.exe");

            string image = Path.GetTempFileName()+".png";
            var script = preparer.CaptureImage(file, image, position);
            var process = PowershellExecutor.Execute(script);

            var color = GetTestImageColor(image);

            if (color.R > 200 && color.G < 10 && color.B < 10)
                return TestFiles.SlideEnum.Nr1Red;
            if (color.R < 10 && color.G > 200 && color.B < 10)
                return TestFiles.SlideEnum.Nr3Green;
            if (color.R < 10 && color.G < 10 && color.B > 200)
                return TestFiles.SlideEnum.Nr2Blue;

            throw new Exception();
        }
    }
}