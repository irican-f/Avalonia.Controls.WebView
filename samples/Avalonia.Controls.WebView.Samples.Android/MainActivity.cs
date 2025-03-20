using Android.App;
using Android.Content;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace Avalonia.Controls.WebView.Samples.Android;

[Activity(
    Label = "Avalonia.Controls.WebView.Samples",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    LaunchMode = LaunchMode.SingleTask,
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
[IntentFilter(actions: ["android.intent.action.VIEW"], Categories = ["android.intent.category.DEFAULT", "android.intent.category.BROWSABLE"], DataScheme = "com.AvaloniaUI.WebView.Samples", DataPath = "/oauth2redirect")]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder);
    }

    public override void OnNewIntent(Intent intent, ComponentCaller caller)
    {
        base.OnNewIntent(intent, caller);
        if (intent?.Data is not null)
        {
            SetResult(Result.Ok, intent);
            Finish();
        }
    }
}
