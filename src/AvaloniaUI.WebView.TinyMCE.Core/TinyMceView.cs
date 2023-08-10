#if AVALONIA || WPF
using System;
using System.Text;
using AvaloniaUI.WebView.TinyMCE.Core;
#if WPF
using IBrush = System.Windows.Media.Brush;

#elif AVALONIA
using Avalonia.Media;
#endif

namespace AvaloniaUI.WebView.TinyMCE;

public enum TinyMceThemeVariant
{
    Light,
    Dark
}

internal record JsPayload(string type, string body);

public partial class TinyMceView
{
    private const string ToolBarDefaultValue = "bold italic underline bullist numlist fontselect fontsizeselect";
    private const string PluginsDefaultValue = "autoresize fullpage";

    private readonly NativeWebView _nativeWebView;
    private bool _ignoreChanges;

    public event EventHandler? HtmlTextChanged;

    public string? HtmlText
    {
        get => (string?)GetValue(HtmlTextProperty);
        set => SetValue(HtmlTextProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public IBrush? Background
    {
        get => (IBrush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public IBrush? Foreground
    {
        get => (IBrush?)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public string ToolBar
    {
        get => (string)GetValue(ToolBarProperty);
        set => SetValue(ToolBarProperty, value);
    }

    public string Plugins
    {
        get => (string)GetValue(PluginsProperty);
        set => SetValue(PluginsProperty, value);
    }

    protected virtual string LoadTinyMceStyle(TinyMceThemeVariant variant)
    {
        return variant == TinyMceThemeVariant.Dark ? HtmlPageBuilder.CharcoalStyle : HtmlPageBuilder.LightGrayStyle;
    }

    protected virtual string LoadTinyMceContentStyle(TinyMceThemeVariant variant)
    {
        return variant == TinyMceThemeVariant.Dark ?
            HtmlPageBuilder.ContentDarkStyle :
            HtmlPageBuilder.ContentLightStyle;
    }

    private void NativeWebViewOnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        SendCurrentText();
    }

    private void NativeWebViewOnWebMessageReceived(object? sender, WebMessageReceivedEventArgs e)
    {
        if (_ignoreChanges) return;

        _ignoreChanges = true;
        var payload = e.Body;
        SetCurrentValue(HtmlTextProperty, payload);
        HtmlTextChanged?.Invoke(this, EventArgs.Empty);

        _ignoreChanges = false;
    }

    private async void SendCurrentText()
    {
        if (_ignoreChanges) return;

        _ignoreChanges = true;
        var payload = System.Web.HttpUtility.JavaScriptStringEncode(HtmlText ?? "");
        await _nativeWebView.InvokeScript($"sendPayload('{payload}')");
        _ignoreChanges = false;
    }
}
#endif
