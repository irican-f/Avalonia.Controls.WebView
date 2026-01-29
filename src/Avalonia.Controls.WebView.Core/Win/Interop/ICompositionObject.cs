using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Avalonia.Controls.Win.Interop;

#if COM_SOURCE_GEN
[GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
#else
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
[Guid("BCB4AD45-7609-4550-934F-16002A68FDED")]
internal partial interface ICompositionObject : IInspectable
{
    ICompositor Compositor();
    IDispatcherQueue Dispatcher();
    IntPtr Properties();
    IntPtr StartAnimation(IntPtr propertyNameHString);
    void StopAnimation(IntPtr propertyNameHString);
}
