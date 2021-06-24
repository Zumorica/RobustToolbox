using System;
using System.IO;
using System.Reflection;
using CefSharp;
using CefSharp.Handler;
using CefSharp.OffScreen;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Robust.Client.CEF
{
    [UsedImplicitly]
    public class CefManager : IApp
    {
        private bool _initialized = false;

        public void Initialize()
        {
            DebugTools.Assert(!_initialized);

            var settings = new CefSettings()
            {
                WindowlessRenderingEnabled = true,
            };

            Cef.Initialize(settings, true);

            _initialized = true;
        }

        public void Shutdown()
        {
            Cef.Shutdown();
        }

        void IApp.OnRegisterCustomSchemes(ISchemeRegistrar registrar)
        {
        }

        IBrowserProcessHandler IApp.BrowserProcessHandler { get; } = new BrowserProcessHandler();

        private static Assembly? Resolver(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith("CefSharp.Core.Runtime"))
                return null;

            string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!,
                Environment.Is64BitProcess ? "x64" : "x86",
                assemblyName);

            return File.Exists(archSpecificPath)
                ? Assembly.LoadFile(archSpecificPath)
                : null;
        }
    }
}
