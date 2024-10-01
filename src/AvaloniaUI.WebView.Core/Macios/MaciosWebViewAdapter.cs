using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia.Platform;
using AvaloniaUI.WebView.Macios.Interop;
using AvaloniaUI.WebView.NativeMac;

namespace AvaloniaUI.WebView.Macios;

[SupportedOSPlatform("macos")]
[SupportedOSPlatform("ios")]
public class MaciosWebViewAdapter : IWebViewAdapter
{
    private readonly WKWebViewConfiguration _config;
    private readonly AvaloniaWKWebView _webView;

    public MaciosWebViewAdapter()
    {
        _config = new WKWebViewConfiguration { JavaScriptEnabled = true };
        _webView = new AvaloniaWKWebView(_config);
    }

    public IntPtr Handle => _webView.Handle;
    public string? HandleDescriptor => OperatingSystemEx.IsMacOS() ? "NSView" : "UIView";
    public bool IsInitialized => true;
    public event EventHandler? Initialized;

    public bool CanGoBack => true;
    public bool CanGoForward => true;
    public Uri Source
    {
        get
        {
            using var sourceUrl = _webView.GetUrl();
            return Uri.TryCreate(sourceUrl.AbsoluteString, UriKind.RelativeOrAbsolute, out var source) ?
                source : WebViewHelper.EmptyPage;
        }
        set => Navigate(value);
    }
    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;
    public bool GoBack()
    {
        throw new NotImplementedException();
    }

    public bool GoForward()
    {
        throw new NotImplementedException();
    }

    public Task<string?> InvokeScript(string script)
    {
        return Task.FromResult<string?>(null);
    }

    public void Navigate(Uri url)
    {
        using var nsUrl = new NSUrl(url.ToString());
        using var request = new NSURLRequest(nsUrl);
        _ = _webView.LoadRequest(request);
    }

    public void NavigateToString(string text)
    {
        throw new NotImplementedException();
    }

    public bool Refresh()
    {
        throw new NotImplementedException();
    }

    public bool Stop()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void SizeChanged()
    {
    }

    public void SetParent(IPlatformHandle parent)
    {
        // no-op
        // macOS control don't need to be explicitly parented
    }
}
