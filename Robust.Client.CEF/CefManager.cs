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
                ExternalMessagePump = false,
                NoSandbox = true,
                BrowserSubprocessPath = "/home/zumo/Projects/space-station-14/bin/Content.Client/Robust.Client.CEF",
                LocalesDirPath = "/home/zumo/Projects/space-station-14/bin/Content.Client/locales/",
                ResourcesDirPath = "/home/zumo/Projects/space-station-14/bin/Content.Client/",
            };

            Logger.Info($"CEF Version: {CefRuntime.ChromeVersion}");

            CefRuntime.Initialize(new CefMainArgs(Array.Empty<string>()), settings, this, IntPtr.Zero);

            _initialized = true;
        }

        public void Update()
        {
            DebugTools.Assert(_initialized);

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
            commandLine.AppendSwitch("--no-zygote");
            commandLine.AppendSwitch("--single-process");
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
