using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fwg.Core
{
    public class PowershellPreparer
    {
        private readonly string pathToFfmpeg;

        public PowershellPreparer(string pathToFfmpeg)
        {
            if(!File.Exists(pathToFfmpeg))
                throw new FileNotFoundException("Couldn't finde ffmpeg.exe", pathToFfmpeg);

            this.pathToFfmpeg = pathToFfmpeg;
        }

        public string CaptureImage(string inputMovie, string outputImage, TimeSpan position)
        {
            return $@".\""{this.pathToFfmpeg}\"" `
-i \""{inputMovie}\"" `
-ss {position.ToString("g",CultureInfo.InvariantCulture)} `
-vframes 1 `
\""{outputImage}\""
";
        }
    }

    public class PowershellExecutor
    {
        public static Process Execute(string script)
        {
            var execute = Process.Start("powershell.exe", $"–NoProfile -NonInteractive -Command \"&{{{script}}}\"");
            execute.WaitForExit();
            return execute;
        }
    }
}
