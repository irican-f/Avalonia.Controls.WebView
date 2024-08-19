//This file will contain actual IID structures
#define COM_GUIDS_MATERIALIZE
#include "common.h"
#include "AvnString.h"


@interface AvaloniaWKWebView : WKWebView

@end

@interface WebViewHandlers : NSObject<WKNavigationDelegate, WKScriptMessageHandler>
-(id)initWithHandlers: (INativeWebViewHandlers*) arg;
-(void)releaseHandlers;
-(void)onScriptResult:(int)index withResult:(id)result withError:(NSError*)error;
-(BOOL)becomeFirstResponder;
-(BOOL)resignFirstResponder;
@end

NSMutableArray* _handlersArray = [[NSMutableArray alloc] init];

class WebViewNative : public ComSingleObject<INativeWebView, &IID_INativeWebView>
{
private:
    AvaloniaWKWebView* _webView;
    WebViewHandlers* _handlersWrapper;

public:
    FORWARD_IUNKNOWN()

    WebViewNative(WebViewHandlers* handlers)
    {
        WKWebViewConfiguration* config = [[WKWebViewConfiguration alloc] init];
        if (@available(macOS 11, *)) {
            [[config defaultWebpagePreferences] setAllowsContentJavaScript: true];
        }
        else {
            [[config preferences] setJavaScriptEnabled: true];
        }
        [config.userContentController addScriptMessageHandler:handlers name:@"postWebViewMessage"];
        [config.preferences setValue:@YES forKey:@"developerExtrasEnabled"]; // only for debug

        CGRect frame = {};
        _webView = [[AvaloniaWKWebView alloc] initWithFrame:frame configuration:config];
        _webView.navigationDelegate = handlers;
        _handlersWrapper = handlers;
    }

    virtual HRESULT ReleaseUnmanaged() override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            [_handlersWrapper releaseHandlers];
            [_handlersArray removeObject: _handlersWrapper];
            
            _handlersWrapper = nullptr;
            if (_webView.superview != nullptr)
            {
                [_webView removeFromSuperview];
            }

            _webView.navigationDelegate = nullptr;
            _webView = nullptr;
            return S_OK;
        }
    }
    
    virtual void* AsNsView () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return (__bridge void*)_webView;
        }
    };

    virtual bool GetCanGoBack () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return [_webView canGoBack];
        }
    }

    virtual bool GoBack () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return [_webView goBack] != nullptr;
        }
    }

    virtual bool GetCanGoForward () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return [_webView canGoForward];
        }
    }

    virtual bool GoForward () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return [_webView goForward] != nullptr;
        }
    }

    virtual HRESULT GetSource (IAvnString** ppv) override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            auto url = _webView.URL.absoluteString;
            *ppv = CreateAvnString(url);
            return S_OK;
        }
    }

    virtual HRESULT Navigate (IAvnString* url) override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            if (url == nullptr) return E_POINTER;
            NSURL* nsUrl = [NSURL URLWithString: GetNSStringWithoutRelease(url)];
            NSURLRequest* request = [NSURLRequest requestWithURL: nsUrl];
            [_webView loadRequest:request];
            return S_OK;
        }
    }

    virtual HRESULT NavigateToString (IAvnString* text, IAvnString* baseUrl) override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            if (text == nullptr) return E_POINTER;
            NSURL* baseNsUrl = nullptr;
            if (baseUrl != nullptr)
            {
                baseNsUrl = [NSURL URLWithString: GetNSStringWithoutRelease(baseUrl)];
            }
            
            auto navigation = [_webView loadHTMLString: GetNSStringWithoutRelease(text) baseURL: baseNsUrl];
            return S_OK;
        }
    }

    virtual bool Refresh () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            return [_webView reload] != nullptr;
        }
    }

    virtual bool Stop () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            [_webView stopLoading];
            return true;
        }
    }

    virtual HRESULT InvokeScript (IAvnString* script, int index) override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            auto scriptStr = GetNSStringWithoutRelease(script);
            
            [_webView evaluateJavaScript:scriptStr completionHandler:^(id _Nullable value, NSError * _Nullable error) {
                [_handlersWrapper onScriptResult: index withResult:value withError:error];
            }];
            return S_OK;
        }
    }
    
    virtual bool Focus () override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            auto window = [_webView window];
            if (window)
            {
                return [window makeFirstResponder: _webView];
            }
            return false;
        }
    }
};

@implementation AvaloniaWKWebView
- (BOOL)acceptsFirstResponder {
    return true;
}
- (BOOL)becomeFirstResponder {
    auto handlers = (WebViewHandlers*)[self navigationDelegate];
    if (handlers) {
        return [handlers becomeFirstResponder];
    }
    return [super becomeFirstResponder];
}
- (BOOL)resignFirstResponder {
    auto handlers = (WebViewHandlers*)[self navigationDelegate];
    if (handlers) {
        return [handlers resignFirstResponder];
    }
    return [super resignFirstResponder];
}
@end


@implementation WebViewHandlers {
    ComPtr<INativeWebViewHandlers> handler;
}
- (id)initWithHandlers: (INativeWebViewHandlers*) arg
{
    handler = arg;
    return self;
}
- (void)releaseHandlers
{
    handler = nil;
}
- (BOOL)becomeFirstResponder {
    @autoreleasepool
    {
        bool cancel = false;
        handler->BecomeFirstResponder(&cancel);
        return !cancel;
    }
}
- (BOOL)resignFirstResponder {
    @autoreleasepool
    {
        bool cancel = false;
        handler->ResignFirstResponder(&cancel);
        return !cancel;
    }
}
- (void)webView:(WKWebView *)webView didFinishNavigation:(WKNavigation *)navigation
{
    if (handler == nullptr) return;
    auto url = webView.URL.absoluteString;

    @autoreleasepool
    {
        auto str = CreateAvnString(url);
        handler->OnNavigationCompleted(str, true);
    }
}
- (void)webView:(WKWebView *)webView
    decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction
    decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler
{
    if (handler == nullptr) return;
    auto url = webView.URL.absoluteString;
    bool cancel = false;

    @autoreleasepool
    {
        auto str = CreateAvnString(url);
        handler->OnNavigationStarted(str, &cancel);
    }

    if (cancel)
    {
        decisionHandler(WKNavigationActionPolicyCancel);
    }
    else
    {
        decisionHandler(WKNavigationActionPolicyAllow);
    }
}
-(void)onScriptResult:(int)index withResult:(id)result withError:(NSError*)error
{
    if (handler == nullptr) return;

    @autoreleasepool
    {
        if (error != nullptr)
        {
            handler->OnScriptResult(index, true, CreateAvnString(error.localizedDescription));
        }
        else
        {
            handler->OnScriptResult(index, false, CreateAvnString((NSString *)result));
        }
    }
}
- (void)userContentController:(WKUserContentController *)userContentController didReceiveScriptMessage:(WKScriptMessage *)message
{
    if ([message.name isEqualToString:@"postWebViewMessage"])
    {
        @autoreleasepool
        {
            auto str = CreateAvnString((NSString *)message.body);
            handler->OnWebMessageReceived(str);
        }
    }
}
@end

class WebViewNativeFactory : public ComSingleObject<IWebViewFactory, &IID_IWebViewFactory>
{
public:
    FORWARD_IUNKNOWN()
    
    virtual INativeWebView* CreateWebView (
        INativeWebViewHandlers* handlers) override
    {
        START_COM_CALL;
        
        @autoreleasepool
        {
            if(handlers == nullptr)
                return nullptr;
            
            if (_handlersArray == nullptr)
                _handlersArray = [[NSMutableArray alloc] init];
            
            auto handlersWrapper = [[WebViewHandlers alloc] initWithHandlers: handlers];
            [_handlersArray addObject: handlersWrapper];
            return new WebViewNative(handlersWrapper);
        }
    }

    virtual HRESULT InvalidateAllManagedReferences () override
    {
        for (WebViewHandlers * item in _handlersArray)
        {
            [item releaseHandlers];
        }
        [_handlersArray removeAllObjects];
        
        return S_OK;
    }
};

extern "C" IWebViewFactory* CreateWebViewNativeFactory()
{
    return new WebViewNativeFactory();
};
