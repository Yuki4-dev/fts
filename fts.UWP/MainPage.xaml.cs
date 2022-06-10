using fts.Shared;
using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace fts.UWP
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            App.RequestReceived += App_RequestReceived;
        }

        private async void App_RequestReceived(Windows.ApplicationModel.AppService.AppServiceConnection sender, Windows.ApplicationModel.AppService.AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message[Constant.RequestMessage];
            var valueset = new ValueSet
            {
                { Constant.ResponseMessage, "OK" }
            };
            _ = args.Request.SendResponseAsync(valueset);

            await Dispatcher.RunIdleAsync((e) => RequestTextBox.Text += $"{DateTime.Now} : {message} \r\n");
        }
    }
}
