using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Platform;
using static AvaloniaUI.WebView.Gtk.GtkInterop;
using static AvaloniaUI.WebView.Gtk.AvaloniaGtk;

namespace AvaloniaUI.WebView.Gtk;

public sealed class GtkNativeWebViewDialog : INativeWebViewDialog
{
    private readonly GtkWebViewAdapter _nativeWebView;
    private IntPtr _windowHandle;
    private bool _disposed;

    public GtkNativeWebViewDialog()
    {
        _windowHandle = RunOnGlibThread(static () =>
        {
            var window = gtk_window_new(0 /* GTK_WINDOW_TOPLEVEL */);
            gtk_window_set_default_size(window, 800, 600);
            return window;
        });

        _nativeWebView = new GtkWebViewAdapter();
        _nativeWebView.NavigationCompleted += (_, a) => NavigationCompleted?.Invoke(this, a);
        _nativeWebView.NavigationStarted += (_, a) => NavigationStarted?.Invoke(this, a);
        _nativeWebView.WebMessageReceived += (_, a) => WebMessageReceived?.Invoke(this, a);

        _ = RunOnGlibThread(() =>
        {
            var scrolled = gtk_scrolled_window_new(IntPtr.Zero, IntPtr.Zero);
            gtk_container_add(scrolled, _nativeWebView.Handle);
            gtk_container_add(_windowHandle, scrolled);
            return 0;
        });
    }

    public string? Title
    {
        get => RunOnGlibThread(() =>
        {
            var titlePtr = gtk_window_get_title(_windowHandle);
            if (titlePtr == IntPtr.Zero)
            {
                return null;
            }

#if NET5_0_OR_GREATER
            return Marshal.PtrToStringUTF8(titlePtr);
#else
            // Custom UTF8 conversion
            var length = 0;
            while (Marshal.ReadByte(titlePtr, length) != 0)
            {
                length++;
            }

            var buffer = new byte[length];
            Marshal.Copy(titlePtr, buffer, 0, length);
            return System.Text.Encoding.UTF8.GetString(buffer);
#endif
        });
        set => RunOnGlibThread(() => gtk_window_set_title(_windowHandle, value ?? string.Empty));
    }

    public void Show() => RunOnGlibThread(() =>
    {
        gtk_widget_show_all(_windowHandle);
        gtk_window_present(_windowHandle);
    });

    public void Show(IPlatformHandle owner)
    {
        if (owner.HandleDescriptor != "XID")
        {
            Show();
            return;
        }

        RunOnGlibThread(() =>
        {
            var xid = owner.Handle;
            var parent = gdk_x11_window_foreign_new_for_display(gdk_display_get_default(), xid);
            gtk_widget_realize(_windowHandle);
            var window = gtk_widget_get_window(_windowHandle);
            if (parent != IntPtr.Zero)
            {
                gdk_window_set_transient_for(window, parent);
            }
            gtk_widget_show_all(_windowHandle);
            gtk_window_present(_windowHandle);
        });
    }

    public void Close()
    {
        if (_windowHandle != IntPtr.Zero)
        {
            RunOnGlibThread(() => gtk_widget_destroy(_windowHandle));
            _windowHandle = IntPtr.Zero;
        }

        _nativeWebView.Dispose();
    }

    public bool CanGoBack => _nativeWebView.CanGoBack;
    public bool CanGoForward => _nativeWebView.CanGoForward;

    public Uri Source
    {
        get => _nativeWebView.Source;
        set => _nativeWebView.Source = value;
    }

    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;
    public bool GoBack() => _nativeWebView.GoBack();
    public bool GoForward() => _nativeWebView.GoForward();
    public Task<string?> InvokeScript(string script) => _nativeWebView.InvokeScript(script);
    public void Navigate(Uri url) => _nativeWebView.Navigate(url);
    public void NavigateToString(string text) => _nativeWebView.NavigateToString(text);
    public bool Refresh() => _nativeWebView.Refresh();
    public bool Stop() => _nativeWebView.Stop();

    private void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Close();
            _nativeWebView.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~GtkNativeWebViewDialog()
    {
        Dispose(false);
    }
}
