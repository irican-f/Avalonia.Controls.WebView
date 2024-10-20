using System;

namespace AvaloniaUI.WebView;

internal class JavaScriptException(string message, Exception? innerException = null)
    : Exception(message, innerException);
