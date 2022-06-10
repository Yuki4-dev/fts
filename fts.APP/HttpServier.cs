using fts.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace fts.APP
{
    internal class HttpServer
    {
        private readonly AppServiceConnection _connection;

        private readonly HttpListener _listener = new();

        private readonly TaskCompletionSource _source = new();

        public HttpServer()
        {
            _connection = new AppServiceConnection
            {
                AppServiceName = Constant.AppServiceName,
                PackageFamilyName = Package.Current.Id.FamilyName
            };
            _connection.ServiceClosed += Connection_ServiceClosed;

            StartAsync();
        }

        private async void StartAsync()
        {
            var status = await _connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                Close();
                return;
            }

            Debug.WriteLine($"Connected.");

            try
            {
                _listener.Prefixes.Add("http://localhost:8080/");
                _listener.Start();
            }
            catch (Exception ex)
            {
                Close();
                return;
            }

            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var context = await _listener.GetContextAsync();
                        var request = context.Request;
                        string requestBody = string.Empty;
                        if (request.HasEntityBody)
                        {
                            using var requestStream = request.InputStream;
                            using var reader = new StreamReader(requestStream, request.ContentEncoding);
                            requestBody = reader.ReadToEnd();
                        }

                        var valueset = new ValueSet
                        {
                            { Constant.RequestMessage, requestBody }
                        };
                        var massage = await _connection.SendMessageAsync(valueset);

                        using var response = context.Response;
                        response.StatusCode = 200;
                        using var responseStream = response.OutputStream;
                        using var writer = new StreamWriter(responseStream, response?.ContentEncoding ?? Encoding.UTF8);
                        writer.Write(massage.Message[Constant.ResponseMessage]);
                    }
                    catch (Exception ex)
                    {
                        Close();
                        return;
                    }
                }
            });
        }

        public void Close()
        {
            Debug.WriteLine($"Service Closed.");
            _listener.Close();
            _connection.Dispose();
            _source.TrySetResult();
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Close();
        }

        public Task WaitClosed()
        {
            return _source.Task;
        }
    }
}
