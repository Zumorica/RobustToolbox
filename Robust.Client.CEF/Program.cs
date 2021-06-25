using System;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            CefRuntime.Initialize(new CefMainArgs(args), new CefSettings(), new SubprocessApp(), IntPtr.Zero);

            return CefRuntime.ExecuteProcess(new CefMainArgs(args), new SubprocessApp(), IntPtr.Zero);
        }
    }

    internal class SubprocessApp : CefApp
    {

    }
}
