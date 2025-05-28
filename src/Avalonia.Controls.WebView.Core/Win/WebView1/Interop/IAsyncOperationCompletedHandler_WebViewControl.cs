using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Avalonia.Controls.Win.WebView1.Interop;

#if COM_SOURCE_GEN
[GeneratedComInterface(Options = ComInterfaceOptions.ManagedObjectWrapper)]
#else
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
[Guid("d61963d6-806d-50a8-a81c-75d9356ad5d7")]
internal partial interface IAsyncOperationCompletedHandler_WebViewControl
{
    void Invoke(IAsyncOperation_WebViewControl asyncInfo, AsyncStatus asyncStatus);
}
