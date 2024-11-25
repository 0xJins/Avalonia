using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;

namespace CefGlueApp.Views;

public partial class MainView : UserControl
{

    private class BrowserLifeSpanHandler : LifeSpanHandler
    {
        protected override bool OnBeforePopup(
            CefBrowser browser,
            CefFrame frame,
            string targetUrl,
            string targetFrameName,
            CefWindowOpenDisposition targetDisposition,
            bool userGesture,
            CefPopupFeatures popupFeatures,
            CefWindowInfo windowInfo,
            ref CefClient client,
            CefBrowserSettings settings,
            ref CefDictionaryValue extraInfo,
            ref bool noJavascriptAccess)
        {
            var bounds = windowInfo.Bounds;
            Dispatcher.UIThread.Post(() =>
            {
                var window = new Window();
                var popupBrowser = new AvaloniaCefBrowser();
                popupBrowser.Address = targetUrl;
                window.Content = popupBrowser;
                window.Position = new PixelPoint(bounds.X, bounds.Y);
                window.Height = bounds.Height;
                window.Width = bounds.Width;
                window.Title = targetUrl;
                window.Show();
            });
            return true;
        }
    }

    public MainView()
    {
        //dotnet build -a x64 --os linux -o Linux-64

        InitializeComponent();

        bool immediately = Environment.GetCommandLineArgs().Contains("-crash");
        if(immediately)
        {
            LoadBrowser();
        }

        btnLoad.Click += (s,e) => LoadBrowser();
    }

    private void LoadBrowser()
    {
        AvaloniaCefBrowser browser = new AvaloniaCefBrowser();

        browser.Address = "https://www.google.com";
        browser.LoadStart += OnBrowserLoadStart;
        browser.LifeSpanHandler = new BrowserLifeSpanHandler();
        browserWrapper.Child = browser;
    }

    private void OnBrowserLoadStart(object sender, LoadStartEventArgs e)
    {
        if (e.Frame.Browser.IsPopup || !e.Frame.IsMain)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            var addressTextBox = this.FindControl<TextBox>("addressTextBox");

            addressTextBox.Text = e.Frame.Url;
        });
    }
}
