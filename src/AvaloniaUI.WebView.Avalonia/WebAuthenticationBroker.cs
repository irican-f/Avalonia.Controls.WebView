using System;
using System.Threading.Tasks;
using AvaloniaUI.WebView.Gtk;
#if WPF
using System.Windows;
using AvaloniaUI.Xpf.WpfAbstractions;
#elif AVALONIA
using Avalonia.Controls;
#endif

namespace AvaloniaUI.WebView;

public static class WebAuthenticationBroker
{
    public static Task<WebAuthenticationResult> AuthenticateAsync
#if WPF
        (Window topLevel, WebAuthenticatorOptions options)
#elif AVALONIA
        (TopLevel topLevel, WebAuthenticatorOptions options)
#endif
    {
        var supportsNativeWebDialog =
            OperatingSystemEx.IsWindows() || OperatingSystemEx.IsLinux() || OperatingSystemEx.IsMacOS();

        if (!(supportsNativeWebDialog & options.PreferNativeWebViewDialog)
            && (OperatingSystemEx.IsIOSVersionAtLeast(13, 0) || OperatingSystemEx.IsMacOSVersionAtLeast(10, 15)))
        {
#if WPF
            var avTopLevel = XpfWpfAbstraction.GetAvaloniaTopLevelForWindow(topLevel);
#elif AVALONIA
            var avTopLevel = topLevel;
#endif
            return MaciosWebAuthenticationBroker.AuthenticateAsync(avTopLevel, options);
        }

        if (supportsNativeWebDialog)
        {
            return AuthenticateDialogAsync(topLevel as Window, options);
        }

        throw new PlatformNotSupportedException();
    }

    private static async Task<WebAuthenticationResult> AuthenticateDialogAsync(Window? owner, WebAuthenticatorOptions options)
    {
        using var dialog = CreateNativeDialog();
        var tcs = new TaskCompletionSource<WebAuthenticationResult>();

        dialog.NavigationStarted += OnNavigationStarted;

        try
        {
            dialog.Title = "Authentication";
            dialog.Source = options.RequestUri;
            if (dialog is WindowNativeWebViewDialog windowDialog && owner is not null)
            {
#if WPF
                windowDialog.Owner = owner;
                windowDialog.Show();
#elif AVALONIA
                windowDialog.Show(owner);
#endif
            }
            else if (owner?.TryGetPlatformHandle() is { } platformHandle)
            {
                dialog.Show(platformHandle);
            }
            else
            {
                dialog.Show();
            }

            return await tcs.Task;
        }
        finally
        {
            dialog.NavigationStarted -= OnNavigationStarted;
            dialog.Close();
        }

        void OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e)
        {
            if (e.Request is not null && IsCallbackUri(e.Request, options.CallbackUri))
            {
                e.Cancel = true;
                tcs.TrySetResult(new WebAuthenticationResult(e.Request));
            }
        }
    }

    private static bool IsCallbackUri(Uri navigatingUri, Uri callbackUri)
    {
        return navigatingUri.Scheme == callbackUri.Scheme
               && navigatingUri.Host == callbackUri.Host
               && navigatingUri.AbsolutePath == callbackUri.AbsolutePath;
    }

    private static INativeWebViewDialog CreateNativeDialog()
    {
        if (OperatingSystemEx.IsLinux())
            return new GtkNativeWebViewDialog();
        return new WindowNativeWebViewDialog();
    }
}
