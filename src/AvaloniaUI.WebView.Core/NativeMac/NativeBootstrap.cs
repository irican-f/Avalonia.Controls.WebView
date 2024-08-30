using System;
using System.Runtime.InteropServices;
using Avalonia;
using MicroCom.Runtime;

namespace AvaloniaUI.WebView.NativeMac;

internal static class NativeBootstrap
{
    private const string FactoryMethodName = "CreateWebViewNativeFactory";

    public static unsafe IWebViewFactory CreateWebViewNativeFactory()
    {
#if NET6_0_OR_GREATER
        var options = AvaloniaLocator.Current.GetService<WebViewOptions>();
        if (options?.WebViewNativePath is { } nativePath)
        {
            var lib = NativeLibrary.Load(nativePath);
            if (lib == default)
            {
                throw new InvalidOperationException("WebViewNativePath wasn't found or can't be loaded.");
            }

            var procPtr = NativeLibrary.GetExport(lib, FactoryMethodName);
            if (procPtr == default)
            {
                throw new InvalidOperationException(FactoryMethodName + " is missing in the native library.");
            }

            var proc = (delegate* unmanaged[Cdecl]<IntPtr>)procPtr;
            return MicroComRuntime.CreateProxyFor<IWebViewFactory>(proc(), true);
        }
#endif

        return MicroComRuntime.CreateProxyFor<IWebViewFactory>(CreateWebViewNativeFactoryNative(), true);
    }

    [DllImport("libWebView", EntryPoint = FactoryMethodName)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern IntPtr CreateWebViewNativeFactoryNative();
}
