using System;
using JetBrains.Annotations;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    [UsedImplicitly]
    public class CefManager : CefApp
    {
        private readonly BrowserProcessHandler _browserProcessHandler;
        private readonly RenderProcessHandler _renderProcessHandler;
        private bool _initialized = false;

        public CefManager() : base()
        {
            _renderProcessHandler = new RenderProcessHandler();
            _browserProcessHandler = new BrowserProcessHandler();
        }

        public void Initialize()
        {
            DebugTools.Assert(!_initialized);

            var settings = new CefSettings()
            {
                WindowlessRenderingEnabled = true,
                NoSandbox = true,
                BrowserSubprocessPath = "/home/zumo/Projects/space-station-14/bin/Content.Client/Robust.Client.CEF",
            };

            Logger.Info(CefRuntime.ChromeVersion);

            CefRuntime.Initialize(new CefMainArgs(new string[]{}), settings, this, IntPtr.Zero);

            _initialized = true;
        }

        public void Update()
        {
            CefRuntime.DoMessageLoopWork();
        }

        public void Shutdown()
        {
            DebugTools.Assert(_initialized);

            Dispose(true);

            CefRuntime.Shutdown();
        }

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

            Logger.Debug($"{commandLine}");
        }

        private class BrowserProcessHandler : CefBrowserProcessHandler
        {
        }

        private class RenderProcessHandler : CefRenderProcessHandler
        {
        }
    }
}
