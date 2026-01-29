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
[Guid("02F6BC74-ED20-4773-AFE6-D49B4A93DB32")]
internal partial interface IContainerVisual : ICompositionVisual
{
    IntPtr GetChildren();
}
