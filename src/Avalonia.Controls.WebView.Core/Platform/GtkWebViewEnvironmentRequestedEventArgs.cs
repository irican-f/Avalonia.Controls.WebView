using Avalonia.Controls;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace Avalonia.Platform;

public sealed class GtkWebViewEnvironmentRequestedEventArgs : WebViewEnvironmentRequestedEventArgs
{
    /// <summary>
    /// Experimental support for GTK WebView that can be hosted in the same Avalonia window, without overlapping other controls.
    /// </summary>
    public bool ExperimentalOffscreen { get; set; }

    /// <summary>
    /// An ephemeral webview handles all websites data as non-persistent, and nothing will be written to the client storage.
    /// Note that if you create an ephemeral webview all other parameters to configure data directories will be ignored.
    /// </summary>
    public bool EphemeralDataManager { get; set; }

    /// <summary>
    /// The base directory for Website data. This is used as a base directory for any Website data when no specific data directory has been provided.
    /// </summary>
    public string? BaseDataDirectory { get; set; }

    /// <summary>
    /// The base directory for Website cache. This is used as a base directory for any Website cache when no specific cache directory has been provided.
    /// </summary>
    public string? BaseCacheDirectory { get; set; }

    /// <summary>
    /// Use a single process to perform content rendering. The process is shared among all the WebKitWebView instances created by the application.
    /// True by default.
    /// </summary>
    public bool SharedProcessModel { get; set; } = true;

    /// <summary>
    /// Disable the cache completely, which substantially reduces memory usage. Useful for applications that only access a single local file, with no navigation to other pages. No remote resources will be cached.
    /// </summary>
    /// <remarks>
    /// Equivalent of WEBKIT_CACHE_MODEL_DOCUMENT_VIEWER. When disabled, WEBKIT_CACHE_MODEL_DOCUMENT_BROWSER is used.
    /// </remarks>
    public bool DisableCache { get; set; } = false;
}
