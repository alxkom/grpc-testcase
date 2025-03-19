namespace TaskManagerProvider
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "TaskManagerProvider.Endpoint";

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).
                ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}