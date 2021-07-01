using System;
using System.Drawing.Imaging;
using System.IO;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Xilium.CefGlue;

namespace Robust.Client.CEF
{
    public class BrowserControl : Control
    {
        [Dependency] private readonly IClyde _clyde = default!;

        private RobustWebClient _client;
        private CefBrowser _browser;
        private ControlRenderHandler _renderer;

        protected override Vector2 MeasureOverride(Vector2 availableSize)
        {
            var buffer = _renderer.Buffer;
            return new Vector2(buffer.Width, buffer.Height);
        }

        public BrowserControl()
        {
            IoCManager.InjectDependencies(this);

            _renderer = new ControlRenderHandler(this);
            _client = new RobustWebClient(_renderer);

            var info = CefWindowInfo.Create();
            info.Width = 500;
            info.Height = 500;
            info.WindowlessRenderingEnabled = true;
            var settings = new CefBrowserSettings()
            {
                WindowlessFrameRate = 60,
            };

            _browser = CefBrowserHost.CreateBrowserSync(info, _client, settings, "about:blank");
        }

        public void Browse(string url)
        {
            //_browser.GetMainFrame().LoadUrl(url);
        }

        protected override void Draw(DrawingHandleScreen handle)
        {
            base.Draw(handle);

            var bitmap = _renderer.Buffer.CreateBitmap();

            if (bitmap == null)
                return;

            using var memoryStream = new MemoryStream();

            bitmap.Save(memoryStream, ImageFormat.Png);

            memoryStream.Seek(0, SeekOrigin.Begin);

            using var texture = _clyde.LoadTextureFromPNGStream(memoryStream);

            handle.DrawTexture(texture, Vector2.Zero);
        }
    }

    internal unsafe class ControlRenderHandler : CefRenderHandler
    {
        public BitmapBuffer Buffer { get; }
        private Control _control;

        internal ControlRenderHandler(Control control)
        {
            Buffer = new BitmapBuffer(this);
            _control = control;
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            if (_control.Disposed)
                return null!;

            // TODO CEF?
            return new AccessibilityHandler();
        }

        protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
        {
            if (_control.Disposed)
            {
                rect = new CefRectangle();
                return;
            }

            var screenCoords = _control.ScreenCoordinates;
            rect = new CefRectangle((int) screenCoords.X, (int) screenCoords.Y, (int)Math.Max(_control.Size.X, 1), (int)Math.Max(_control.Size.Y, 1));
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            if (_control.Disposed)
                return false;

            // TODO CEF
            screenInfo.DeviceScaleFactor = 1.0f;

            return true;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
            if (_control.Disposed)
                return;
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (_control.Disposed)
                return;

            foreach (var dirtyRect in dirtyRects)
            {
                Buffer.UpdateBuffer(width, height, buffer, dirtyRect);
            }
        }

        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
            if (_control.Disposed)
                return;
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            if (_control.Disposed)
                return;
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
            if (_control.Disposed)
                return;
        }
    }

    internal class AccessibilityHandler : CefAccessibilityHandler
    {
        protected override void OnAccessibilityTreeChange(CefValue value)
        {
        }

        protected override void OnAccessibilityLocationChange(CefValue value)
        {
        }
    }
}
