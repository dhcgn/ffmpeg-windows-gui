using System.Collections.Generic;
using NUnit.Framework;
using System.IO;

namespace Fwg.Core.Test
{
    public static class TestFiles
    {
        public enum SlideEnum
        {
            Nr1Red,
            Nr2Blue,
            Nr3Green,
        }

        public static Dictionary<SlideEnum, string> Filenames = new Dictionary<SlideEnum, string>()
        {
            {SlideEnum.Nr1Red, "Slide_1" },
            {SlideEnum.Nr2Blue, "Slide_2" },
            {SlideEnum.Nr3Green, "Slide_3" },
        };
        public static Dictionary<TypeEnum, string> Exentsions = new Dictionary<TypeEnum, string>()
        {
            {TypeEnum.Mp4, ".mp4" },
            {TypeEnum.Wmv, ".wmv" },
        };


        public enum TypeEnum
        {
            Wmv,
            Mp4
        }

        private static readonly string Dir = TestContext.CurrentContext.TestDirectory;

        public static string Slides => $@"{SlidesFolder}Slides1-3.mp4";
        public static string SlidesFolder => $@"{Dir}\..\..\..\..\TestFiles\";

        public static string GetTestVideo(SlideEnum slide, TypeEnum type)
        {
            return Path.Combine(SlidesFolder, Filenames[slide] + Exentsions[type]);
        }
    }
}