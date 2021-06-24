using System;
using JetBrains.Annotations;
using Robust.Shared.Utility;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    [UsedImplicitly]
    public class CefManager : CefApp
    {
        private bool _initialized = false;

        public void Initialize()
        {
            DebugTools.Assert(!_initialized);

            var settings = new CefSettings()
            {
                WindowlessRenderingEnabled = true,
            };

            CefRuntime.Initialize(new CefMainArgs(new string[0]), settings, this, IntPtr.Zero);

            _initialized = true;
        }

        public void Shutdown()
        {
            DebugTools.Assert(_initialized);

            CefRuntime.Shutdown();
        }
    }
}
