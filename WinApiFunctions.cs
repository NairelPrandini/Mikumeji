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
    private static extern bool GetWindowInfo(IntPtr hwnd, ref tagWINDOWINFO pwi);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, ref Rect lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr GetTopWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowTextA(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct tagWINDOWINFO
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
    }

    public struct DesktopWindow
    {
        public IntPtr Handle;
        public Rectangle Bounds;
        public int Layer;
        public string Title;

        public DesktopWindow(IntPtr H, string T, Rectangle R, int Z)
        {
            Handle = H;
            Title = T;
            Bounds = R;
            Layer = Z;
        }
    }

    const uint GW_HWNDNEXT = 2;
    const int BufferSize = 50;

    public static List<DesktopWindow> GetWindows()
    {
        List<DesktopWindow> AllWindows = new List<DesktopWindow>();
        IntPtr WindowHandle = GetTopWindow(GetDesktopWindow());

        int LayerIndex = 1;
        while (WindowHandle != new IntPtr(0))
        {
            StringBuilder Buffer = new StringBuilder(BufferSize);
            Rect R = new Rect();
            GetWindowTextA(WindowHandle, Buffer, BufferSize);
            if (IsWindowVisible(WindowHandle) && Buffer.Length != 0)
            {

                if (WindowHandle != GetActiveWindow())
                {
                    if (GetWindowRect(WindowHandle, ref R))
                    {
                        var Rect = new Rectangle(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top);
                        DesktopWindow W = new DesktopWindow(WindowHandle, Buffer.ToString(), Rect, LayerIndex);
                        AllWindows.Add(W);
                        LayerIndex++;
                    }
                }
            }


            WindowHandle = GetWindow(WindowHandle, GW_HWNDNEXT);
        }
        return AllWindows;
    }

    public static uint WindowPtrToPID(IntPtr Handle)
    {
        return GetWindowThreadProcessId(Handle, out uint Processid);
    }



    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, out Rect pvParam, uint fWinIni);
    const uint SPI_GETWORKAREA = 0x0030;
    public static Rectangle GetWorkAreaRect()
    {
        SystemParametersInfo(SPI_GETWORKAREA, 0, out var R, 0);
        return new Rectangle(R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_TOP = new IntPtr(0);
    public static IntPtr hWnd = GetActiveWindow();


    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);



    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    const int GWL_EXSTYLE = -20;
    const int GWL_STYLE = -16;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;

    public static void SetScreenToWorkArea()
    {
        Rectangle WorkArea = WinApiFunctions.GetWorkAreaRect();
        SetWindowPos(hWnd, HWND_TOPMOST, (int)WorkArea.x, (int)WorkArea.y, (int)WorkArea.width, (int)WorkArea.height, 0);
    }

    public static void SetWindowParcialClickThrough()
    {
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
    }


}