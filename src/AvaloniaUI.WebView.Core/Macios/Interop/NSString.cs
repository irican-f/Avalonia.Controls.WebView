using System;
using System.Text;

namespace AvaloniaUI.WebView.Macios.Interop;

internal static class NSString
{
    private static readonly IntPtr s_class = Libobjc.objc_getClass("NSString");
    private static readonly IntPtr s_stringWithCharacters = Libobjc.sel_getUid("stringWithCharacters:length:");
    private static readonly IntPtr s_getUTF8String = Libobjc.sel_getUid("UTF8String");

    public static unsafe IntPtr Create(string? value)
    {
        if (value == null) { return IntPtr.Zero; }

        fixed (char* ptr = value)
        {
            return Libobjc.intptr_objc_msgSend(
                s_class,
                s_stringWithCharacters,
                (IntPtr)ptr,
                new IntPtr((uint)value.Length));
        }
    }

    public static string? GetString(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            return null;
        }

        var utf8 = Libobjc.intptr_objc_msgSend(handle, s_getUTF8String);
        return Utf8PointerToString(utf8);
    }

    public static unsafe string? Utf8PointerToString(IntPtr utf8)
    {
        if (utf8 == IntPtr.Zero) { return null; }

        int count = 0;
        byte* ptr = (byte*)utf8;
        while (*ptr != 0)
        {
            count++;
            ptr++;
        }

        return Encoding.UTF8.GetString((byte*)utf8, count);
    }
}
