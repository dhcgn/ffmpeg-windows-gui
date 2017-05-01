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
        private string tempOutputDir;
        private PowershellPreparer preparer;

        [SetUp]
        public void Setup()
        {
            var directory = TestContext.CurrentContext.TestDirectory;
            this.preparer = new PowershellPreparer($@"{directory}\..\..\FFmpegStatic\ffmpeg.exe");

            this.tempOutputDir = Path.Combine(directory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.tempOutputDir);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(this.tempOutputDir, true);
        }

        [Test]
        [TestCase(255, 0, 0, 0, TestName = "Start of 1. Slide (red)")]
        [TestCase(255, 0, 0, DurationOneSlide / 2, TestName = "Middle of 1. Slide (red)")]
        [TestCase(0, 0, 255, DurationOneSlide / 2 + DurationOneSlide, TestName = "Middle of 2. Slide (blue)")]
        [TestCase(0, 255, 0, 2 * DurationOneSlide + DurationOneSlide / 2, TestName = "Middle of 3. Slide (green)")]
        public void CaptureImage(byte r, byte g, byte b, int ms)
        {
            var file = Path.Combine(this.tempOutputDir, "picture.png");

            TimeSpan pos = TimeSpan.FromMilliseconds(ms);
            var script = this.preparer.CaptureImage(TestFiles.Slides, file, pos);

            Console.Out.WriteLine("Script");
            Console.Out.WriteLine(script);

            var process = PowershellExecutor.Execute(script);

            Assert.That(File.Exists(file), "output file exists");

            Color pixelColor = Helper.GetTestImageColor(file);

            Assert.That(pixelColor.A, Is.EqualTo(255).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.R, Is.EqualTo(r).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.G, Is.EqualTo(g).Within(ColorTolerance).Percent);
            Assert.That(pixelColor.B, Is.EqualTo(b).Within(ColorTolerance).Percent);
        }


        [Test]
        [TestCase(TestFiles.TypeEnum.Wmv, TestName = "format wmv")]
        [TestCase(TestFiles.TypeEnum.Mp4, TestName = "format mp4")]
        public void JoinSameCodec(TestFiles.TypeEnum type)
        {
            var output = Path.Combine(this.tempOutputDir, "concated_video" + TestFiles.Exentsions[type]);

            var slides = new[]
            {
                TestFiles.SlideEnum.Nr1Red,
                TestFiles.SlideEnum.Nr2Blue,
                TestFiles.SlideEnum.Nr3Green,
            };

            var files = slides.Select(slide => TestFiles.GetTestVideo(slide, type));

            foreach (var file in files)
            {
                Assert.That(File.Exists(file), "input test must exists");
            }

            var script = this.preparer.ConcateClips(output, files.ToArray());

            Console.Out.WriteLine("Script");
            Console.Out.WriteLine(script);

            var process = PowershellExecutor.Execute(script);
            Assert.That(File.Exists(output), "output file exists");

            Assert.That(Helper.DetermineSlide(TimeSpan.FromSeconds(2.5), output), Is.EqualTo(TestFiles.SlideEnum.Nr1Red));
            Assert.That(Helper.DetermineSlide(TimeSpan.FromSeconds(5 + 2.5), output), Is.EqualTo(TestFiles.SlideEnum.Nr2Blue));
            Assert.That(Helper.DetermineSlide(TimeSpan.FromSeconds(10 + 2.5), output), Is.EqualTo(TestFiles.SlideEnum.Nr3Green));
        }
    }
}