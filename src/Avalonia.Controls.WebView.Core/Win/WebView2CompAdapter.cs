#if NOPE
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia.Platform;
using Microsoft.Web.WebView2.Core;

namespace Avalonia.Controls.Win;

[SupportedOSPlatform("windows10.0.17763.0")]
internal class WebView2CompAdapter : WebView2BaseAdapter
{
    private WebView2CompAdapter(IPlatformHandle handle) : base(handle)
    {
    }

    public override IntPtr Handle => default;

    public override string HandleDescriptor => "Windows.UI.Composition.ContainerVisual";

    protected override Task<CoreWebView2Controller> CreateWebView2Controller(CoreWebView2Environment env, IntPtr handle)
    {
        return env.CreateCoreWebView2ControllerAsync(Handle);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
        base.Dispose(disposing);
    }
}
#endif
