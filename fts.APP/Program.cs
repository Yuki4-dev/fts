using fts.Shared;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;

namespace fts.APP
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await new HttpServer().WaitClosed();
        }
    }
}