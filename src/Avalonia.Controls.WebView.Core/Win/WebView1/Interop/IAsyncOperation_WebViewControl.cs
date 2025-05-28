using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Avalonia.Controls.Win.WebView1.Interop;

#if COM_SOURCE_GEN
[GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
#else
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
[Guid("ac3d28ac-8362-51c6-b2cc-16f3672758f1")]
internal partial interface IAsyncOperation_WebViewControl : IInspectable
{
#if !COM_SOURCE_GEN
    void _VtblGap1_3();
#endif

    void put_Completed(IAsyncOperationCompletedHandler_WebViewControl handler);

    IAsyncOperationCompletedHandler_WebViewControl get_Completed();

    IWebViewControl GetResults();
}
