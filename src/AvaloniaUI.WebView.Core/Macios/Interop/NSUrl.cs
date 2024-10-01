using System;

namespace AvaloniaUI.WebView.Macios.Interop;

internal class NSUrl : NSObject
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSUrl");
    private static readonly IntPtr s_initWithUrl = Libobjc.sel_getUid("initWithString:");
    private static readonly IntPtr s_absoluteString = Libobjc.sel_getUid("absoluteString");

    public NSUrl(IntPtr handle)
    {
        Handle = handle;
    }

    public NSUrl(string uriStr) : base(s_class)
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, s_initWithUrl, NSString.Create(uriStr));
    }

    public string? AbsoluteString
    {
        get
        {
            var nsString = Libobjc.intptr_objc_msgSend(Handle, s_absoluteString);
            return NSString.GetString(nsString);
        }
    }
}
