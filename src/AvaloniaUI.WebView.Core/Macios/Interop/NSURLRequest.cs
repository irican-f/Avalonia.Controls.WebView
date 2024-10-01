using System;

namespace AvaloniaUI.WebView.Macios.Interop;

internal class NSURLRequest : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSURLRequest");
    private static readonly IntPtr s_initWithUrl = Libobjc.sel_getUid("initWithURL:");

    public NSURLRequest(NSUrl nsUrl) : base(s_class)
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, s_initWithUrl, nsUrl.Handle);
    }
}
