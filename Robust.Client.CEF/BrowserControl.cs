using System;
using System.Drawing.Imaging;
using System.IO;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Log;
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

        protected override Vector2 MeasureCore(Vector2 availableSize)
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
            info.SetAsWindowless(IntPtr.Zero, false);
            info.Width = 500;
            info.Height = 500;
            info.WindowlessRenderingEnabled = true;
            var settings = new CefBrowserSettings()
            {
                WindowlessFrameRate = 60,
            };

            _browser = CefBrowserHost.CreateBrowserSync(info, _client, settings, "about:version");
        }

        protected override void MouseMove(GUIMouseMoveEventArgs args)
        {
            base.MouseMove(args);

            // TODO CEF Modifiers
            _browser.GetHost().SendMouseMoveEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), false);
        }

        protected override void MouseExited()
        {
            base.MouseExited();

            // TODO CEF Modifiers
            _browser.GetHost().SendMouseMoveEvent(new CefMouseEvent(0, 0, CefEventFlags.None), true);
        }

        protected override void MouseWheel(GUIMouseWheelEventArgs args)
        {
            base.MouseWheel(args);

            // TODO CEF Modifiers
            _browser.GetHost().SendMouseWheelEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), (int)args.Delta.X*4, (int)args.Delta.Y*4);
        }

        protected override void TextEntered(GUITextEventArgs args)
        {
            base.TextEntered(args);

            _browser.GetHost().SendKeyEvent(new CefKeyEvent(){NativeKeyCode = (int) args.CodePoint});
        }

        protected override void KeyBindUp(GUIBoundKeyEventArgs args)
        {
            base.KeyBindUp(args);

            // TODO CEF Clean up this shitty code. Also add middle click.

            if (args.Function == EngineKeyFunctions.UIClick)
            {
                _browser.GetHost().SendMouseClickEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), CefMouseButtonType.Left, true, 1);
            } else if (args.Function == EngineKeyFunctions.UIRightClick)
            {
                _browser.GetHost().SendMouseClickEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), CefMouseButtonType.Middle, true, 1);
            }
        }

        protected override void KeyBindDown(GUIBoundKeyEventArgs args)
        {
            base.KeyBindDown(args);

            if (args.Function == EngineKeyFunctions.UIClick)
            {
                _browser.GetHost().SendMouseClickEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), CefMouseButtonType.Left, false, 1);
            } else if (args.Function == EngineKeyFunctions.UIRightClick)
            {
                _browser.GetHost().SendMouseClickEvent(new CefMouseEvent((int)args.RelativePosition.X, (int)args.RelativePosition.Y, CefEventFlags.None), CefMouseButtonType.Middle, false, 1);
            }
        }

        protected override void Resized()
        {
            base.Resized();

            _browser.GetHost().NotifyMoveOrResizeStarted();
            _browser.GetHost().WasResized();
        }

        public void Browse(string url)
        {
            _browser.GetMainFrame().LoadUrl(url);
        }

        protected override void Draw(DrawingHandleScreen handle)
        {
            base.Draw(handle);

            var bitmap = _renderer.Buffer.CreateBitmap();

            if (bitmap == null)
                return;

            Logger.Debug("DRAW!");

            using var memoryStream = new MemoryStream();

            bitmap.Save(memoryStream, ImageFormat.Png);

            memoryStream.Seek(0, SeekOrigin.Begin);

            var texture = _clyde.LoadTextureFromPNGStream(memoryStream);

            handle.DrawTexture(texture, Vector2.Zero);
        }
    }

    internal class ControlRenderHandler : CefRenderHandler
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

            //var screenCoords = _control.ScreenCoordinates;
            //rect = new CefRectangle((int) screenCoords.X, (int) screenCoords.Y, (int)Math.Max(_control.Size.X, 1), (int)Math.Max(_control.Size.Y, 1));
            rect = new CefRectangle(0, 0, (int)Math.Max(_control.Size.X, 1), (int)Math.Max(_control.Size.Y, 1));
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

            Logger.Info("We paint!");

            foreach (var dirtyRect in dirtyRects)
            {
                Buffer.UpdateBuffer(width, height, buffer, dirtyRect);
            }
        }

        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
            // Unused.
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

        private class AccessibilityHandler : CefAccessibilityHandler
        {
            protected override void OnAccessibilityTreeChange(CefValue value)
            {
            }

            protected override void OnAccessibilityLocationChange(CefValue value)
            {
            }
        }
    }
}
