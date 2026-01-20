using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel.DataTransfer;

namespace Translate.Services
{
    /// <summary>
    /// Service for detecting text selection via mouse hook and clipboard monitoring.
    /// </summary>
    public class TextSelectionService : IDisposable
    {
        private IntPtr _mouseHookHandle = IntPtr.Zero;
        private NativeMethods.LowLevelMouseProc? _mouseProc;
        private System.Drawing.Point _mouseDownPos;
        private bool _isMouseDown;
        private string _previousClipboardText = string.Empty;
        private DateTime _lastClickTime = DateTime.MinValue;
        private System.Drawing.Point _lastClickPos;
        private readonly DispatcherQueue? _dispatcherQueue;

        private const int DoubleClickTime = 500;
        private const int DoubleClickDistance = 4;

        /// <summary>
        /// Event fired when text is selected via mouse drag or double-click.
        /// </summary>
        public event Action<string>? TextSelected;

        public TextSelectionService(DispatcherQueue? dispatcherQueue = null)
        {
            _dispatcherQueue = dispatcherQueue;
        }

        /// <summary>
        /// Start the global mouse hook for text selection detection.
        /// </summary>
        public void Start()
        {
            if (_mouseHookHandle != IntPtr.Zero) return;
            _mouseProc = MouseHookCallback;
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            using var module = process.MainModule;
            _mouseHookHandle = NativeMethods.SetWindowsHookEx(
                NativeMethods.WH_MOUSE_LL, _mouseProc,
                NativeMethods.GetModuleHandle(module?.ModuleName), 0);
        }

        /// <summary>
        /// Stop the global mouse hook.
        /// </summary>
        public void Stop()
        {
            if (_mouseHookHandle != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_mouseHookHandle);
                _mouseHookHandle = IntPtr.Zero;
            }
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                if (wParam == NativeMethods.WM_LBUTTONDOWN)
                {
                    _mouseDownPos = new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y);
                    _isMouseDown = true;
                    var now = DateTime.Now;
                    var timeDiff = (now - _lastClickTime).TotalMilliseconds;
                    var distX = Math.Abs(hookStruct.pt.x - _lastClickPos.X);
                    var distY = Math.Abs(hookStruct.pt.y - _lastClickPos.Y);
                    if (timeDiff < DoubleClickTime && distX < DoubleClickDistance && distY < DoubleClickDistance)
                    {
                        _ = HandleTextSelectionAsync();
                        _lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        _lastClickTime = now;
                        _lastClickPos = new System.Drawing.Point(hookStruct.pt.x, hookStruct.pt.y);
                    }
                }
                else if (wParam == NativeMethods.WM_LBUTTONUP && _isMouseDown)
                {
                    _isMouseDown = false;
                    var deltaX = Math.Abs(hookStruct.pt.x - _mouseDownPos.X);
                    var deltaY = Math.Abs(hookStruct.pt.y - _mouseDownPos.Y);
                    if (deltaX > 10 || deltaY > 5)
                    {
                        _ = HandleTextSelectionAsync();
                    }
                }
            }
            return NativeMethods.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        private async Task HandleTextSelectionAsync()
        {
            await Task.Delay(100);
            try
            {
                var currentContent = Clipboard.GetContent();
                if (currentContent.Contains(StandardDataFormats.Text))
                    _previousClipboardText = await currentContent.GetTextAsync();
            }
            catch { }

            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_C, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_C, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);

            await Task.Delay(100);
            try
            {
                var dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text))
                {
                    var text = (await dataPackageView.GetTextAsync()).Trim();
                    if (!string.IsNullOrWhiteSpace(text) && text != _previousClipboardText)
                    {
                        if (_dispatcherQueue != null)
                            _dispatcherQueue.TryEnqueue(() => TextSelected?.Invoke(text));
                        else
                            TextSelected?.Invoke(text);
                    }
                }
            }
            catch { }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        private static class NativeMethods
        {
            public const int WH_MOUSE_LL = 14;
            public const int WM_LBUTTONDOWN = 0x0201;
            public const int WM_LBUTTONUP = 0x0202;
            public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("user32.dll")]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string? lpModuleName);
            [DllImport("user32.dll")]
            public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT { public int x; public int y; }
            [StructLayout(LayoutKind.Sequential)]
            public struct MSLLHOOKSTRUCT
            {
                public POINT pt;
                public uint mouseData;
                public uint flags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            public const byte VK_CONTROL = 0x11;
            public const byte VK_C = 0x43;
            public const uint KEYEVENTF_KEYUP = 0x0002;
        }
    }
}
