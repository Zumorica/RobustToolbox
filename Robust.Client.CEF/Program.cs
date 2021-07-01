using System;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return CefRuntime.ExecuteProcess(new CefMainArgs(args), null, IntPtr.Zero);
        }
    }

    /*
    internal class SubprocessApp : CefApp
    {
        private readonly BrowserProcessHandler _browserProcessHandler = new();
        private readonly RenderProcessHandler _renderProcessHandler = new();

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return _browserProcessHandler;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            commandLine.AppendSwitch("--disable-gpu");
            commandLine.AppendSwitch("--disable-gpu-compositing");
            commandLine.AppendSwitch("--in-process-gpu");

            System.Console.WriteLine(commandLine.ToString());
        }
    }*/
}
