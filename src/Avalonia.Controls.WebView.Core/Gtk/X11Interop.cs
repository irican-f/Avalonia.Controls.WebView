using System;
using System.Runtime.InteropServices;

namespace Avalonia.Controls.Gtk;

public static partial class X11Interop
{
    private const string libX11 = "libX11.so.6";

#if NET7_0_OR_GREATER
    [LibraryImport(libX11)]
    public static partial int XMapWindow(IntPtr display, IntPtr window);
    [LibraryImport(libX11)]
    public static partial int XUnmapWindow(IntPtr display, IntPtr window);
    [LibraryImport(libX11)]
    public static partial int XRaiseWindow(IntPtr display, IntPtr window);
    [LibraryImport(libX11)]
    public static partial int XGetWindowAttributes(IntPtr display, IntPtr window, ref XWindowAttributes attributes);
    [LibraryImport(libX11)]
    public static partial int XReparentWindow(IntPtr display, IntPtr window, IntPtr parent, int x, int y);
    [LibraryImport(libX11)]
    public static partial int XFlush(IntPtr display);
    [LibraryImport(libX11)]
    public static partial int XSync(IntPtr display, [MarshalAs(UnmanagedType.Bool)] bool discard);
	[LibraryImport(libX11)]
	public unsafe static partial int XChangeWindowAttributes(IntPtr display, IntPtr window, IntPtr valuemask, XSetWindowAttributes* attributes);
#else
    [DllImport(libX11)]
    public static extern int XMapWindow(IntPtr display, IntPtr window);

    [DllImport(libX11)]
    public static extern int XUnmapWindow(IntPtr display, IntPtr window);

    [DllImport(libX11)]
    public static extern int XRaiseWindow(IntPtr display, IntPtr window);

    [DllImport(libX11)]
    public static extern int XGetWindowAttributes(IntPtr display, IntPtr window, ref XWindowAttributes attributes);

    [DllImport(libX11)]
    public static extern int XReparentWindow(IntPtr display, IntPtr window, IntPtr parent, int x, int y);

    [DllImport(libX11)]
    public static extern int XFlush(IntPtr display);

    [DllImport(libX11)]
    public static extern int XSync(IntPtr display, [MarshalAs(UnmanagedType.Bool)] bool discard);
    
    [DllImport(libX11)]
    public static extern unsafe int XChangeWindowAttributes(IntPtr display, IntPtr window, IntPtr valuemask, XSetWindowAttributes* attributes);
#endif

    [StructLayout(LayoutKind.Sequential)]
    public struct XSetWindowAttributes
    {
        public IntPtr background_pixmap;
        public IntPtr background_pixel;
        public IntPtr border_pixmap;
        public IntPtr border_pixel;
        public Gravity bit_gravity;
        public Gravity win_gravity;
        public int backing_store;
        public IntPtr backing_planes;
        public IntPtr backing_pixel;
        public int save_under;
        public IntPtr event_mask;
        public IntPtr do_not_propagate_mask;
        public int override_redirect;
        public IntPtr colormap;
        public IntPtr cursor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XWindowAttributes
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int border_width;
        public int depth;
        public IntPtr visual;
        public IntPtr root;
        public int c_class;
        public Gravity bit_gravity;
        public Gravity win_gravity;
        public int backing_store;
        public IntPtr backing_planes;
        public IntPtr backing_pixel;
        public int save_under;
        public IntPtr colormap;
        public int map_installed;
        public MapState map_state;
        public IntPtr all_event_masks;
        public IntPtr your_event_mask;
        public IntPtr do_not_propagate_mask;
        public int override_direct;
        public IntPtr screen;
    }

    public enum Gravity
    {
        ForgetGravity = 0,
        NorthWestGravity = 1,
        NorthGravity = 2,
        NorthEastGravity = 3,
        WestGravity = 4,
        CenterGravity = 5,
        EastGravity = 6,
        SouthWestGravity = 7,
        SouthGravity = 8,
        SouthEastGravity = 9,
        StaticGravity = 10
    }

    public enum MapState
    {
        IsUnmapped = 0,
        IsUnviewable = 1,
        IsViewable = 2
    }

    [Flags]
    internal enum XCreateWindowFlags
    {
        CWBackPixmap = (1 << 0),
        CWBackPixel = (1 << 1),
        CWBorderPixmap = (1 << 2),
        CWBorderPixel = (1 << 3),
        CWBitGravity = (1 << 4),
        CWWinGravity = (1 << 5),
        CWBackingStore = (1 << 6),
        CWBackingPlanes = (1 << 7),
        CWBackingPixel = (1 << 8),
        CWOverrideRedirect = (1 << 9),
        CWSaveUnder = (1 << 10),
        CWEventMask = (1 << 11),
        CWDontPropagate = (1 << 12),
        CWColormap = (1 << 13),
        CWCursor = (1 << 14),
    }
}
