using System;

namespace AvaloniaUI.WebView.Macios.Interop;

internal class NSObject : IDisposable
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSObject");
    private static IntPtr AllocSel { get; } = Libobjc.sel_getUid("alloc");
    private static IntPtr InitSel { get; } = Libobjc.sel_getUid("init");

    protected NSObject()
    {
    }

    protected NSObject(IntPtr classHandle)
    {
        Handle = Libobjc.intptr_objc_msgSend(classHandle, AllocSel);
    }

    public IntPtr Handle { get; protected set; }

    public static IntPtr AllocateClassPair(string className)
        => Libobjc.objc_allocateClassPair(s_class, className, 0);

    protected void Init()
    {
        Handle = Libobjc.intptr_objc_msgSend(Handle, InitSel);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
