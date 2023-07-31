using Avalonia.Platform;

namespace AvaloniaWebView.TinyMCE;

internal static class HtmlPageBuilder
{
    public static string Build()
    {
        using var scriptStream = AssetLoader.Open(new Uri($"avares://{nameof(AvaloniaWebView)}.{nameof(TinyMCE)}/tiny_mce.min.js"));
        using var streamReader = new StreamReader(scriptStream);
        var tinyMceScript = streamReader.ReadToEnd();

        var initScript = """
tinyMCE.init({
    mode : "textareas"
});

var textarea = document.querySelector("textarea");
textarea.onchange = function() {
    var obj = {
        'type': 'textChanged',
        'body': textarea.value
    };
    invokeCSharpAction(JSON.stringify(obj));
}
function sendPayload(json) {
    var obj = JSON.parse(json);
}
""";
        
        return $"""
<!DOCTYPE html>
<html lang="en">
    <head>
        <title>TinyMCE</title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8"/>

        <script type="text/javascript">{tinyMceScript}</script>
    </head>
    <body>
        <textarea name="content" cols="50" rows="15">This is some content that will be editable with TinyMCE.</textarea>
        <script type="text/javascript">{initScript}</script>
    </body>
</html>
""";
    }
}
