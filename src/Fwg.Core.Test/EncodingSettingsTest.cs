using System;
using System.CodeDom;
using NUnit.Framework;

namespace Fwg.Core.Test
{
    [TestFixture]
    public class EncodingSettingsTest
    {
        [Test]
        [TestCase("-c:v libx265", TestName = "Codec")]
        [TestCase("-preset medium", TestName = "Preset")]
        [TestCase("-crf 28", TestName = "RateControl and Value")]
        public void VideoSettings(string argument)
        {
            var encodingSettings = EncodingSettings.CreateVideoSettings(EncodingSettings.CodecEnum.h265, EncodingSettings.RateControlEnum.ConstantRateFactor, EncodingSettings.PresetsEnum.Medium, value: 28);
            
            Assert.That(encodingSettings.GetArgument(), Does.Contain(argument));

            Console.Out.WriteLine("Generated argument");
            Console.Out.WriteLine(encodingSettings.GetArgument());
        }
        [Test]
        [TestCase("-c:v copy", TestName = "Codec")]
        public void VideoSettings_StreamCopy(string argument)
        {
            var encodingSettings = EncodingSettings.CreateVideoSettings(EncodingSettings.CodecEnum.Copy, EncodingSettings.RateControlEnum.None, EncodingSettings.PresetsEnum.Medium, value: 28);
            
            Assert.That(encodingSettings.GetArgument(), Does.Contain(argument));

            Console.Out.WriteLine("Generated argument");
            Console.Out.WriteLine(encodingSettings.GetArgument());
        }

        [Test]
        [TestCase("-c:a aac", TestName = "Codec")]
        [TestCase("-b:a 128k", TestName = "RateControl and Value")]
        public void AudioSettings_Acc(string argument)
        {
            var encodingSettings = EncodingSettings.CreateAudioSettings(EncodingSettings.CodecEnum.Aac, EncodingSettings.RateControlEnum.ConstantBitRate, value: 128);

            Assert.That(encodingSettings.GetArgument(), Does.Contain(argument));

            Console.Out.WriteLine("Generated argument");
            Console.Out.WriteLine(encodingSettings.GetArgument());
        }

        [Test]
        [TestCase("-an", TestName = "Codec")]
        public void AudioSettings_DisableAudio(string argument)
        {
            var encodingSettings = EncodingSettings.CreateAudioSettings(EncodingSettings.CodecEnum.None, EncodingSettings.RateControlEnum.Remove,0);

            Assert.That(encodingSettings.GetArgument(), Does.Contain(argument));

            Console.Out.WriteLine("Generated argument");
            Console.Out.WriteLine(encodingSettings.GetArgument());
        }
    }
}