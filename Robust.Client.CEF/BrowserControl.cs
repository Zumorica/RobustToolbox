using System.Drawing.Imaging;
using System.IO;
using CefSharp;
using CefSharp.OffScreen;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Robust.Client.CEF
{
    public class BrowserControl : Control
    {
        private readonly ChromiumWebBrowser _webBrowser = new();

        public BrowserControl()
        {
        }

        public void Browse(string url)
        {
            _webBrowser.Load(url);
        }

        protected override void Draw(DrawingHandleScreen handle)
        {
            base.Draw(handle);

            var screenshot = _webBrowser.ScreenshotOrNull();

            if (screenshot == null) return;

            using var memoryStream = new MemoryStream();

            screenshot.Save(memoryStream, ImageFormat.Png);

            var clyde = IoCManager.Resolve<IClyde>();

            using var texture = clyde.LoadTextureFromPNGStream(memoryStream);

            handle.DrawTexture(texture, Vector2.Zero);
        }
    }
}
