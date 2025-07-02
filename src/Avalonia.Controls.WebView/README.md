# Avalonia WebView

The Avalonia WebView component provides native web browser functionality for your Avalonia applications. Unlike embedded WebView solutions that require bundling Chromium, this implementation leverages the platform's native web rendering capabilities, resulting in smaller application size and better performance.

## Requirements

This package requires Avalonia Accelerate license:
https://avaloniaui.net/accelerate

## Features

- **Native Web Rendering**: Uses platform-specific web engines (WebView2 on Windows, WebKit on macOS, WebKitGTK on Linux)
- **Lightweight**: No embedded browser engine required - significantly smaller application footprint
- **AOT Compatible**: Full support for Ahead-of-Time compilation and publishing
- **Flexible Platform Configuration**: Configure WebView2 profiles, persistent storage locations, and other platform-specific settings
- **Full Web Integration**: Complete support for cookies, JavaScript execution, bidirectional messaging, and HTTP headers interception
- **Authentication Support**: Built-in web authentication broker for OAuth and web-based authentication flows

## Quick Start

Get started quickly with the WebView component:
https://docs.avaloniaui.net/accelerate/components/webview/quickstart

## Components

### NativeWebView Control

The main WebView control for displaying web content in your application.

**Documentation**: https://docs.avaloniaui.net/accelerate/components/webview/nativewebview

### NativeWebDialog

Native web dialog that provides a way to display web content in a separate window, particularly useful for platforms like Linux where embedded WebView controls might not be available

**Documentation**: https://docs.avaloniaui.net/accelerate/components/webview/nativewebdialog

### WebAuthenticationBroker

WebAuthenticationBroker is a utility class that facilitates OAuth and other web-based authentication flows by providing a secure way to handle web authentication in desktop applications.

**Documentation**: https://docs.avaloniaui.net/accelerate/components/webview/webauthenticationbroker
