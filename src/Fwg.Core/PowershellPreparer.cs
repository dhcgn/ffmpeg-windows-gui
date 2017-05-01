using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Fwg.Core
{
    public class PowershellPreparer
    {
        private readonly string pathToFfmpeg;

        public PowershellPreparer(string pathToFfmpeg)
        {
            if (!File.Exists(pathToFfmpeg))
                throw new FileNotFoundException("Couldn't finde ffmpeg.exe", pathToFfmpeg);

            this.pathToFfmpeg = pathToFfmpeg;
        }

        public string CaptureImage(string inputMovie, string outputImage, TimeSpan position)
        {
            return $@"
.""{this.pathToFfmpeg}"" `
    -i ""{inputMovie}"" `
    -ss {position.ToString("g", CultureInfo.InvariantCulture)} `
    -vframes 1 `
    ""{outputImage}""
";
        }

        /// <summary>
        /// Concat (stream copy) if clips has exactly the same codec and codec parameters
        /// </summary>
        /// <returns></returns>
        public string ConcateClips(string ouput, params string[] inputs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# this is a comment");
            foreach (var input in inputs)
            {
                sb.AppendLine($"file '{input}'");
            }

            var path = Path.GetTempFileName();
            File.WriteAllText(path, sb.ToString());

            return $@"
.""{this.pathToFfmpeg}"" `
    -f concat `
    -safe 0 `
    -i ""{path}"" `
    -c copy `
    ""{ouput}""
";
        }


        /// <summary>
        /// Encode to H.265
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ouput"></param>
        /// <returns></returns>
        public string Encode(string input, string ouput, EncodingSettings encodingSettingsVideo, EncodingSettings encodingSettingsAudio)
        {
            return null;
        }
    }
}