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
        private readonly BrowserProcessHandler _browserProcess = new();
        private readonly RenderProcessHandler _renderProcessHandler = new();
        private bool _initialized = false;

        public CefManager() : base()
        {

        }

        public void Initialize()
        {
            DebugTools.Assert(!_initialized);

            var settings = new CefSettings()
            {
                WindowlessRenderingEnabled = true,
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
            return _browserProcess;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            Logger.Debug($"{processType} -- {commandLine}");
        }
    }

    public class BrowserProcessHandler : CefBrowserProcessHandler
    {

    }

    public class RenderProcessHandler : CefRenderProcessHandler
    {
    }
}
