using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Avalonia.Controls.Win.WebView1.Interop;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Controls.Win.WebView1;

[SupportedOSPlatform("windows6.1")]
internal sealed class WebView1Adapter : IWebViewAdapter
{
    private static IWebViewControlProcess? s_lazyProcess;
    private static int s_webViewsCount;

    private IWebViewControl? _webViewControl;
    private IWebViewControlSite? _webViewControlSite;
    private Action? _subscriptions;

    public static bool IsAvailable
    {
        get
        {
            try
            {
                return (s_lazyProcess ??= CreateProcess()) is not null;
            }
            catch
            {
                return false;
            }
        }
    }

    public WebView1Adapter(IPlatformHandle handle)
    {
        Handle = handle.Handle;
        Initialize();

        Interlocked.Increment(ref s_webViewsCount);
    }

    public IntPtr Handle { get; }
    public string? HandleDescriptor => "HWDN";

    private async void Initialize()
    {
        var process = s_lazyProcess ??= CreateProcess();

        var operation = process.CreateWebViewControl((long)Handle,
            new winrtRect { Height = 100, Width = 100 });
        var handler = new WebViewControlHandler();
        operation.put_Completed(handler);

        var control = await handler.Task;

        if (control.get_Settings() is { } settings)
        {
            settings.put_IsJavaScriptEnabled(true);
            settings.put_IsScriptNotifyAllowed(true);
        }

        _webViewControl = control;
        // ReSharper disable once SuspiciousTypeConversion.Global
        // IWebViewControlSite can be queried from the IWebViewControl
        _webViewControlSite = (IWebViewControlSite)control;
        
        _webViewControlSite.put_IsVisible(true);
        SizeChanged(default);

        _subscriptions = AddHandlers(_webViewControl);

        IsInitialized = true;
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    public bool IsInitialized { get; private set; }

    public event EventHandler? Initialized;
    public event EventHandler<WebViewNewWindowRequestedEventArgs>? NewWindowRequested;
    public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;
    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;

    public bool CanGoBack => _webViewControl?.get_CanGoBack() ?? false;

    public bool CanGoForward => _webViewControl?.get_CanGoForward() ?? false;

    public Uri Source
    {
        get
        {
            var absoluteUri = _webViewControl?.get_Source()?.get_AbsoluteUri();
            return absoluteUri.HasValue
                   && Uri.TryCreate(HStringInterop.FromIntPtr(absoluteUri.Value), UriKind.Absolute, out var uri)
                ? uri : WebViewHelper.EmptyPage;
        }
        set
        {
            Navigate(value);
        }
    }

    public bool GoBack()
    {
        if (!CanGoBack) return false;
        _webViewControl?.GoBack();
        return true;
    }

    public bool GoForward()
    {
        if (!CanGoForward) return false;
        _webViewControl?.GoForward();
        return true;
    }

    public Task<string?> InvokeScript(string script)
    {
        //return _webViewControl?.InvokeScriptAsync("eval", new[] { script }).AsTask() ?? Task.FromResult<string?>(null);
        return Task.FromResult<string?>(script);
    }

    public void Navigate(Uri url)
    {
        var factory = NativeWinRTMethods.CreateActivationFactory<IUriRuntimeClassFactory>("Windows.Foundation.Uri");
        using var hstring = new HStringInterop(url.AbsoluteUri);
        var uri = factory?.CreateUri(hstring.Handle);
        if (uri is not null)
            _webViewControl?.put_Source(uri);
    }

    public void NavigateToString(string text)
    {
        using var hstring = new HStringInterop(text);
        _webViewControl?.NavigateToString(hstring.Handle);
    }

    public bool Refresh()
    {
        _webViewControl?.Refresh();
        return true;
    }

    public bool Stop()
    {
        _webViewControl?.Stop();
        return true;
    }

    public void SizeChanged(PixelSize containerSize)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (PInvoke.GetWindowRect(new HWND(Handle), out var rect)
                && _webViewControlSite is not null)
            {
                _webViewControlSite.put_Bounds(new winrtRect
                {
                    Width = rect.Width,
                    Height = rect.Height
                });
            }
        });
    }

    public void SetParent(IPlatformHandle parent)
    {
    }

    private static IWebViewControlProcess CreateProcess()
    {
        var options = NativeWinRTMethods.CreateInstance<IWebViewControlProcessOptions>("Windows.Web.UI.Interop.WebViewControlProcessOptions")
            ?? throw new InvalidOperationException("Unable to create WebViewControlProcessOptions.");
        options.put_PrivateNetworkClientServerCapability(WebViewControlProcessCapabilityState.Enabled);
        var factory = NativeWinRTMethods.CreateActivationFactory<IWebViewControlProcessFactory>("Windows.Web.UI.Interop.WebViewControlProcess")
            ?? throw new InvalidOperationException("Unable to create WebViewControlProcess.");
        return factory.CreateWithOptions(options);
    }

    private Action AddHandlers(IWebViewControl webView)
    {
        // webView.NavigationStarting += WebViewOnNavigationStarting;
        // void WebViewOnNavigationStarting(object? sender, WebViewControlNavigationStartingEventArgs e)
        // {
        //     var args = new WebViewNavigationStartingEventArgs { Request = e.Uri };
        //     NavigationStarted?.Invoke(this, args);
        //     if (args.Cancel)
        //     {
        //         e.Cancel = true;
        //     }
        // }
        //
        // webView.NavigationCompleted += WebViewOnNavigationCompleted;
        // async void WebViewOnNavigationCompleted(object? sender, WebViewControlNavigationCompletedEventArgs e)
        // {
        //     await InvokeScript("function invokeCSharpAction(data){window.external.notify(data);}");
        //     
        //     NavigationCompleted?.Invoke(this, new WebViewNavigationCompletedEventArgs
        //     {
        //         Request = ((WebViewControl)sender!).Source,
        //         IsSuccess = e.IsSuccess
        //     });
        // }
        //
        // webView.ScriptNotify += WebViewOnScriptNotify;
        // void WebViewOnScriptNotify(IWebViewControl sender, WebViewControlScriptNotifyEventArgs args)
        // {
        //     WebMessageReceived?.Invoke(this, new WebMessageReceivedEventArgs { Body = args.Value });
        // }

        return () =>
        {
            // webView.NavigationStarting -= WebViewOnNavigationStarting;
            // webView.NavigationCompleted -= WebViewOnNavigationCompleted;
            // webView.ScriptNotify -= WebViewOnScriptNotify;
        };
    }

    public void Dispose()
    {
        var webViewsCount = Interlocked.Decrement(ref s_webViewsCount);

        _subscriptions?.Invoke();

        if (_webViewControlSite is not null)
        {
            _webViewControlSite.Close();
            _webViewControlSite = null;
        }

        _webViewControl = null;

        if (s_lazyProcess is { } process && webViewsCount == 0)
        {
            s_lazyProcess = null;
        }
    }
}
