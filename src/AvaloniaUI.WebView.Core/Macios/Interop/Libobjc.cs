using System;
using System.Runtime.InteropServices;

namespace AvaloniaUI.WebView.Macios.Interop;

internal static unsafe partial class Libobjc
{
    internal const string libobjc = "/usr/lib/libobjc.dylib";

#if NET7_0_OR_GREATER
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getClass(string className);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getMetaClass(string className);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr sel_getUid(string selector);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_allocateClassPair(IntPtr superclass, string selector, int extraBytes);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr objc_getProtocol(string selector);
    [LibraryImport(libobjc)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool class_addProtocol(IntPtr basePtr, IntPtr protocol);
    [LibraryImport(libobjc, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool class_addMethod(IntPtr basePtr, IntPtr selector, void* method, string types);
    [LibraryImport(libobjc, EntryPoint = "objc_msgSend")]
    public static partial IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector);
    [LibraryImport(libobjc, EntryPoint = "objc_msgSend")]
    public static partial IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1);
    [LibraryImport(libobjc, EntryPoint = "objc_msgSend")]
    public static partial IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2);
    [LibraryImport(libobjc, EntryPoint = "objc_msgSend")]
    public static partial IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, CGRect param1, IntPtr param2);
    [LibraryImport(libobjc, EntryPoint = "objc_msgSend")]
    public static partial void void_objc_msgSend(IntPtr basePtr, IntPtr selector, [MarshalAs(UnmanagedType.I1)] bool param1);
#else
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getClass(string className);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getMetaClass(string className);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr sel_getUid(string selector);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_allocateClassPair(IntPtr superclass, string selector, int extraBytes);
    [DllImport(libobjc, CharSet = CharSet.Ansi)]
    public static extern IntPtr objc_getProtocol(string selector);
    [DllImport(libobjc)]
    public static extern bool class_addProtocol(IntPtr basePtr, IntPtr protocol);
    [DllImport(libobjc)]
    public static extern bool class_addMethod(IntPtr basePtr, IntPtr selector, void* method, string types);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, IntPtr param1, IntPtr param2);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern IntPtr intptr_objc_msgSend(IntPtr basePtr, IntPtr selector, CGRect param1, IntPtr param2);
    [DllImport(libobjc, EntryPoint = "objc_msgSend")]
    public static extern void void_objc_msgSend(IntPtr basePtr, IntPtr selector, bool param1);
#endif
}
