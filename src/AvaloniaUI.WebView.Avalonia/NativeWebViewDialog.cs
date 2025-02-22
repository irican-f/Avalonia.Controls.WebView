using System;
using System.Threading.Tasks;
using IPlatformHandle = Avalonia.Platform.IPlatformHandle;
using AvaloniaUI.WebView.Gtk;
#if WPF
using System.Windows;
using AvaloniaUI.Xpf.WpfAbstractions;
#elif AVALONIA
using Avalonia.Controls;
#endif

namespace AvaloniaUI.WebView;

public class NativeWebViewDialog : IWebView, INativeWebViewDialog
{
    private readonly INativeWebViewDialog _impl;

    public NativeWebViewDialog()
    {
        _impl = OperatingSystemEx.IsLinux() ? new GtkNativeWebViewDialog() : new WindowNativeWebViewDialog();
        _impl.WebView.NavigationStarted += (_, args) => NavigationStarted?.Invoke(this, args);
        _impl.WebView.NavigationStarted += (_, args) => NavigationStarted?.Invoke(this, args);
        _impl.WebView.WebMessageReceived += (_, args) => WebMessageReceived?.Invoke(this, args);
    }

    public bool CanGoBack => _impl.WebView.CanGoBack;
    public bool CanGoForward => _impl.WebView.CanGoForward;
    public Uri Source { get => _impl.WebView.Source; set => _impl.WebView.Source = value; }

    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;

    public bool GoBack() => _impl.WebView.GoBack();
    public bool GoForward() => _impl.WebView.GoForward();
    public Task<string?> InvokeScript(string script) => _impl.WebView.InvokeScript(script);
    public void Navigate(Uri url) => _impl.WebView.Navigate(url);
    public void NavigateToString(string text) => _impl.WebView.NavigateToString(text);
    public bool Refresh() => _impl.WebView.Refresh();
    public bool Stop() => _impl.WebView.Stop();

    public void Dispose() => _impl.Dispose();

    public string? Title { get => _impl.Title; set => _impl.Title = value; }
    public void Show() => _impl.Show();

#if WPF
    public void Show(Window owner)
#elif AVALONIA
    public void Show(TopLevel owner)
#endif
    {
#if WPF
        var avTopLevel = XpfWpfAbstraction.GetAvaloniaTopLevelForWindow(owner);
#elif AVALONIA
        var avTopLevel = owner;
#endif

        if (owner is Window ownerWindow && TryGetWindow() is { } window)
        {
#if WPF
            window.Owner = ownerWindow;
            window.Show();
#elif AVALONIA
            window.Show(ownerWindow);
#endif
        }
        else if (avTopLevel?.TryGetPlatformHandle() is { } platformHandle)
        {
            _impl.Show(platformHandle);
        }
        else
        {
            _impl.Show();
        }
    }

    public void Close() => _impl.Close();

    public IPlatformHandle? TryGetPlatformHandle() => _impl.TryGetPlatformHandle();
    public Window? TryGetWindow() => _impl as WindowNativeWebViewDialog;

    IWebView INativeWebViewDialog.WebView => _impl.WebView;
    void INativeWebViewDialog.Show(IPlatformHandle owner) => _impl.Show(owner);
}
