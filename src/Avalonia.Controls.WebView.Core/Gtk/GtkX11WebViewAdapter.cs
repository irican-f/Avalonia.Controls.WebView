using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using Avalonia.Threading;
using static Avalonia.Controls.Gtk.GtkInterop;
using static Avalonia.Controls.Gtk.AvaloniaGtk;
using static Avalonia.Controls.Gtk.X11Interop;

namespace Avalonia.Controls.Gtk;

internal class GtkX11WebViewAdapter(IPlatformHandle handle) : GtkWebViewAdapter, IPlatformHandle
{
    private IntPtr _x11Window;
    private IntPtr _windowHandle;

    // GTK thread
    protected override void InitializeSafe()
    {
        _windowHandle = gtk_window_new(0 /* GTK_WINDOW_TOPLEVEL */);
        //gtk_window_set_type_hint(_windowHandle, 6 /* GDK_WINDOW_TYPE_HINT_DOCK */);
        base.InitializeSafe(); // creates WebKitWebView object and subscribes to signals

        gtk_container_add(_windowHandle, WebViewHandle);
        gtk_widget_show_all(WebViewHandle);
        gtk_widget_realize(_windowHandle);
        gtk_widget_show_all(_windowHandle);
        _x11Window = gdk_x11_window_get_xid(_windowHandle);
    }

    // Avalonia UI thread
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, "X11NativeControlHost+DumbWindow", "Avalonia.X11")]
    protected override void OnInitialized()
    {
        if (handle.HandleDescriptor != "XID")
            throw new InvalidOperationException("Parent is not supported");

        var display = (IntPtr)handle.GetType()
            .GetField("_display", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(handle)!;

        XWindowAttributes attributes = default;
        _ = XGetWindowAttributes(display, _x11Window, ref attributes);
        attributes.override_direct = /* True */ 1;
        unsafe
        {
            var attr = Marshal.AllocHGlobal(sizeof(XWindowAttributes));
#pragma warning disable CA1421
            Marshal.StructureToPtr<XWindowAttributes>(attributes, attr, false);
            _ = XChangeWindowAttributes(display, _x11Window, (IntPtr)XCreateWindowFlags.CWOverrideRedirect, (XSetWindowAttributes*)attr.ToPointer());
            Marshal.FreeHGlobal(attr);
#pragma warning restore
        }

        var a = XReparentWindow(display, _x11Window, handle.Handle, 0, 0);
        _ = XFlush(display);
        XSync(display, false); // XSync is necessary after XReparent for unknown reasons

        _ = XMapWindow(display, _x11Window);
        _ = XRaiseWindow(display, handle.Handle);

        //host.RegisterInputFromNativeSubwindow(nativeWindow.WindowId);
        //HideWindowFromTaskBar(nativeWindow);
        //xamlRoot.RenderInvalidated += UpdateLayout;
        //xamlRoot.QueueInvalidateRender(); // to force initial layout and clipping
        base.OnInitialized();
    }

    IntPtr IPlatformHandle.Handle => handle.Handle;
    string IPlatformHandle.HandleDescriptor => "XID";
}
