using System;
using Avalonia;
using Avalonia.Controls;
using AvaloniaWebView.TinyMCE;

namespace AvaloniaWebView.Sample;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void NativeWebView_OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        Console.WriteLine(e.Request);

        await ((NativeWebView)sender!).InvokeScript(""" invokeCSharpAction("{'key': 10}") """);
    }

    private void NativeWebView_OnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e)
    {
        Console.WriteLine(e.Request);
    }

    private void NativeWebView_OnWebMessageReceived(object? sender, WebMessageReceivedEventArgs e)
    {
        Console.WriteLine(e.Body);
    }

    private void AvaloniaObject_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TinyMceView.HtmlTextProperty)
        {
            Console.WriteLine(e.NewValue);
        }
    }
}
