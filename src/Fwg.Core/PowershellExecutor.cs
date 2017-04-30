using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fwg.Core
{
    public class PowershellExecutor
    {
        public static Process Execute(string script)
        {
            var base64Script = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));

            var arguments = $"–NoProfile -NonInteractive -EncodedCommand {base64Script}";

            if (false)
            {
                arguments = "-NoExit " + arguments;
            }

            var execute = Process.Start("powershell.exe", arguments);
            execute.WaitForExit();
            return execute;
        }
    }
}
