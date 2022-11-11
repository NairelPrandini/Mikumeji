using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

public static class WinApiFunctions
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, ref Rect lpRect);

    [DllImport("user32.dll")]
    public static extern IntPtr GetTopWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowTextA(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, out Rect pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int GetWindowLongA(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("dwmapi.dll")]
    static extern int DwmGetWindowAttribute(IntPtr hwnd, uint dwAttribute, out uint pvAttribute, uint cbAttribute);

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_TOP = new IntPtr(0);
    static IntPtr CurrenthWnd = GetActiveWindow();
    static IntPtr DesktophWnd = GetDesktopWindow();
    const int GWL_EXSTYLE = -20;
    const int GWL_STYLE = -16;
    const int WS_VISIBLE = 0x10000000;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;
    const uint GW_HWNDNEXT = 2;
    const uint WS_TABSTOP = 0x00010000;
    const uint WS_BORDER = 0x00800000;
    const uint WS_DISABLED = 0x08000000;
    const int BufferSize = 256;
    const uint SPI_GETWORKAREA = 0x0030;

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    public struct Window
    {
        public IntPtr Handle;
        public Rectangle Bounds;
        public int Layer;
        public string Title;

        public Rect WRect;

        public Window(IntPtr H, string T, Rectangle R, int Z, Rect _WRect)
        {
            Handle = H;
            Title = T;
            Bounds = R;
            Layer = Z;
            WRect = _WRect;
        }
    }


    public static void SetScreenToWorkArea()
    {

        Rectangle WorkArea = WinApiFunctions.GetWorkAreaRect();
        SetWindowPos(CurrenthWnd, HWND_TOPMOST, (int)WorkArea.x, (int)WorkArea.y, (int)WorkArea.width, (int)WorkArea.height, 0);
    }

    public static void SetScreenToMonitorArea()
    {
        Rectangle MonitorArea = new Rectangle(0, 0, Raylib.GetMonitorWidth(0), Raylib.GetMonitorHeight(0));
        Rectangle WorkArea = WinApiFunctions.GetWorkAreaRect();

        SetWindowPos(CurrenthWnd, HWND_TOPMOST, (int)(MonitorArea.width - WorkArea.width), (int)(MonitorArea.height - WorkArea.height), (int)WorkArea.width, (int)WorkArea.height, 0);

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);
    const uint LWA_ALPHA = 0x00000002;



    [DllImport("user32.dll")]
    static extern bool UpdateLayeredWindow(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);



    public static void SetWindowParcialClickThrough()
    {
        SetWindowLong(CurrenthWnd, GWL_EXSTYLE, WS_EX_LAYERED | 0x08000000);

        SetLayeredWindowAttributes(CurrenthWnd, 0, 255, LWA_COLORKEY);

    }



    public static List<Window> GetWindows()
    {
        List<Window> AllWindows = new List<Window>();
        IntPtr WindowHandle = GetTopWindow(DesktophWnd);

        int LayerIndex = 1;

        while (WindowHandle != new IntPtr(0))
        {

            StringBuilder Buffer = new StringBuilder(BufferSize);
            GetWindowTextA(WindowHandle, Buffer, BufferSize);

            Rect R = new Rect();
            GetWindowRect(WindowHandle, ref R);

            if (IsValidWindow(WindowHandle))
            {

                var Rect = new Rectangle(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top);
                Window W = new Window(WindowHandle, Buffer.ToString(), Rect, LayerIndex, R);

                AllWindows.Add(W);
                LayerIndex++;
            }

            WindowHandle = GetWindow(WindowHandle, GW_HWNDNEXT);
        }
        return AllWindows;
    }

    public static Rectangle GetWorkAreaRect()
    {
        SystemParametersInfo(SPI_GETWORKAREA, 0, out var R, 0);
        return new Rectangle(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top);
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct WindowInfo
    {
        public uint cbSize;
        public Rect rcWindow;
        public Rect rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WindowInfo(Boolean? filler) : this()
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WindowInfo)));
        }

    }


    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool GetWindowInfo(IntPtr Hwnd, ref WindowInfo pwi);


    public static bool IsValidWindow(IntPtr hWnd)
    {
        const uint WS_EX_TOOLWINDOW = 0x00000080;
        const uint DWMWA_CLOAKED = 14;


        if (hWnd == CurrenthWnd || IsIconic(hWnd)) return false;

        if (!IsWindowVisible(hWnd)) return false;

        WindowInfo winInfo = new WindowInfo(true);
        GetWindowInfo(hWnd, ref winInfo);

        if ((winInfo.dwExStyle & WS_EX_TOOLWINDOW) != 0) return false;

        uint CloakedVal;
        DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out CloakedVal, sizeof(uint));
        return CloakedVal == 0;
    }

}