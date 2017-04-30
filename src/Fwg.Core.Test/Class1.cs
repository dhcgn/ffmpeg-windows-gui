using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Fwg.Core.Test
{
    [TestFixture]
    public class PowershellPreparerTest
    {
        private const int ColorTolerance = 20;
        private const int DurationOneSlide = 5000;
        private string outputDir;

        [SetUp]
        public void Setup()
        {
            this.outputDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.outputDir);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(this.outputDir, true);
        }

        [Test]
        [TestCase(255, 0, 0, 0, TestName = "Start of 1. Slide (red)")]
        [TestCase(255, 0, 0, DurationOneSlide/2, TestName = "Middle of 1. Slide (red)")]
        [TestCase(0, 0, 255, DurationOneSlide / 2 + DurationOneSlide, TestName = "Middle of 2. Slide (blue)")]
        [TestCase(0, 255, 0, 2*DurationOneSlide + DurationOneSlide / 2, TestName = "Middle of 3. Slide (green)")]
        public void CaptureImage(byte r, byte g, byte b, int ms)
        {
            var directory = TestContext.CurrentContext.TestDirectory;
            var preparer = new PowershellPreparer($@"{directory}\..\..\FFmpegStatic\ffmpeg.exe");

            var file = Path.Combine(this.outputDir, "picture.png");
            TimeSpan pos = TimeSpan.FromMilliseconds(ms);
            var script = preparer.CaptureImage(TestFiles.Slides, file, pos);

            Console.Out.WriteLine("Script");
            Console.Out.WriteLine(script);

            var process = PowershellExecutor.Execute(script);

            Assert.That(File.Exists(file), "output file exists");

            Color pixelColor;
            using (Bitmap myBitmap = new Bitmap(file))
            {
                pixelColor = myBitmap.GetPixel(10, 10);
            }

            Assert.That(pixelColor.A, Is.EqualTo(255).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.R, Is.EqualTo(r).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.G, Is.EqualTo(g).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.B, Is.EqualTo(b).Within(ColorTolerance).Percent);
        }
    }

    public static class TestFiles
    {
        private static readonly string Dir = TestContext.CurrentContext.TestDirectory;

        public static string Slides => $@"{Dir}\..\..\..\..\TestFiles\Slides1-3.mp4";
    }

    public class Helper
    {
        public enum SlideEnum
        {
            Nr1Red,
            Nr2Blue,
            Nr3Green,
        }

        public static SlideEnum DetermineSlide(TimeSpan position, string fullpath)
        {
            var tmep = new PowershellPreparer(@"..\..\FFmpegStatic\ffmpeg.exe");

            return SlideEnum.Nr1Red;
        }
    }
}