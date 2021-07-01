using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    internal class RobustWebClient : CefClient
    {
        private readonly CefRenderHandler _renderHandler;

        internal RobustWebClient(CefRenderHandler handler)
        {
            _renderHandler = handler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }
    }
}
