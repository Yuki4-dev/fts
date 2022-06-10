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