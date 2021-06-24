using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    internal class WebClient : CefClient
    {
        private CefRenderHandler _renderHandler;

        internal WebClient(CefRenderHandler handler)
        {
            _renderHandler = handler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }
    }
}
