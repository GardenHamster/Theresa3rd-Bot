namespace TheresaBot.MiraiHttpApi
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
                webBuilder.UseWebRoot(Path.Combine(AppContext.BaseDirectory, "wwwroot")).UseStartup<Startup>();
            });
        }

    }
}
