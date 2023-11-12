using TheresaBot.Main.Helper;

namespace TheresaBot.GoCqHttp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                var webRootPath = AppHelper.GetWebRootPath();
                if (!string.IsNullOrEmpty(webRootPath)) webBuilder.UseWebRoot(webRootPath);
                webBuilder.UseStartup<Startup>();
            });
        }

    }
}
