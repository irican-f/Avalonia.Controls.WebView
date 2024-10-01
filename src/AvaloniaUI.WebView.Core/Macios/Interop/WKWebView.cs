using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AvaloniaUI.WebView.Macios.Interop;

internal class AvaloniaWKWebView : NSObject
{
    private static readonly IntPtr s_webViewClass;
    private static readonly IntPtr s_initWithFrame = Libobjc.sel_getUid("initWithFrame:configuration:");
    private static readonly IntPtr s_loadRequest = Libobjc.sel_getUid("loadRequest:");
    private static readonly IntPtr s_url = Libobjc.sel_getUid("URL");

    static unsafe AvaloniaWKWebView()
    {
        s_webViewClass = WebKit.objc_getClass("WKWebView");
        return;
        var webViewClass = AllocateClassPair("AvaloniaWKWebView");

        var webViewProtocol = WebKit.objc_getProtocol("WKWebView");
        var result = Libobjc.class_addProtocol(webViewClass, webViewProtocol);
        Debug.Assert(result);
        //
        // var performKeyEquivalentSel = Libobjc.sel_getUid("performKeyEquivalent:");
        // result = Libobjc.class_addMethod(webViewClass, performKeyEquivalentSel,
        //     (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, bool>)&PerformKeyEquivalent,
        //     "B@:@");
        // Debug.Assert(result);

        // var acceptsFirstResponderSel = Libobjc.sel_getUid("acceptsFirstResponder:");
        // result = Libobjc.class_addMethod(webViewClass, acceptsFirstResponderSel,
        //     (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, bool>)&AcceptsFirstResponder,
        //     "B@:");
        // Debug.Assert(result);
        //
        // var becomeFirstResponderSel = Libobjc.sel_getUid("becomeFirstResponder:");
        // result = Libobjc.class_addMethod(webViewClass, becomeFirstResponderSel,
        //     (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, bool>)&BecomeFirstResponder,
        //     "B@:");
        // Debug.Assert(result);
        //
        // var resignFirstResponderSel = Libobjc.sel_getUid("resignFirstResponder:");
        // result = Libobjc.class_addMethod(webViewClass, resignFirstResponderSel,
        //     (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, bool>)&ResignFirstResponder,
        //     "B@:");
        // Debug.Assert(result);

        s_webViewClass = webViewClass;
    }

    public AvaloniaWKWebView(WKWebViewConfiguration configuration) : base(s_webViewClass)
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, s_initWithFrame, new CGRect(), configuration.Handle);
    }

    public NSUrl GetUrl()
    {
        return new NSUrl(Libobjc.intptr_objc_msgSend(Handle, s_url));
    }

    public IntPtr LoadRequest(NSURLRequest request)
    {
        return Libobjc.intptr_objc_msgSend(Handle, s_loadRequest, request.Handle);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static bool PerformKeyEquivalent(IntPtr self, IntPtr sel, IntPtr nsEvent)
    {
        return false;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static bool AcceptsFirstResponder(IntPtr self, IntPtr sel) => true;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static bool BecomeFirstResponder(IntPtr self, IntPtr sel) => true;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static bool ResignFirstResponder(IntPtr self, IntPtr sel) => true;
}
