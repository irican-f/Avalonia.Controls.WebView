using System;

namespace AvaloniaUI.WebView.Macios.Interop;

internal class WKWebViewConfiguration : NSObject
{
    private static readonly IntPtr s_class = WebKit.objc_getClass("WKWebViewConfiguration");
    private static readonly IntPtr s_defaultWebpagePreferences = Libobjc.sel_getUid("defaultWebpagePreferences");
    private static readonly IntPtr s_setAllowsContentJavaScript = Libobjc.sel_getUid("setAllowsContentJavaScript:");

    public WKWebViewConfiguration() : base(s_class)
    {
        Init();
    }

    public bool JavaScriptEnabled
    {
        set
        {
            var defaultPreferences = Libobjc.intptr_objc_msgSend(Handle, s_defaultWebpagePreferences);
            Libobjc.void_objc_msgSend(defaultPreferences, s_setAllowsContentJavaScript, value);
        }
    }
}
