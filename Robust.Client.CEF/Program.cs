using System;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            CefRuntime.Load();

            var mainArgs = new CefMainArgs(args);
            var app = new SubprocessApp();

            var code = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);

            if (code != 0)
            {
                System.Console.WriteLine($"CEF Subprocess exited with exit code {code}! Arguments: {string.Join(' ', args)}");
            }

            return code;
        }

        private class SubprocessApp : CefApp
        {
            protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
            {
                base.OnBeforeCommandLineProcessing(processType, commandLine);

                commandLine.AppendSwitch("--no-zygote");
                commandLine.AppendSwitch("--disable-gpu");
                commandLine.AppendSwitch("--disable-gpu-compositing");
                commandLine.AppendSwitch("--in-process-gpu");

                System.Console.WriteLine($"SUBPROCESS COMMAND LINE: {commandLine.ToString()}");
            }
        }
    }
}
