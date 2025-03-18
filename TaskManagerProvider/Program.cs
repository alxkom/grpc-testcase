namespace TaskManagerProvider
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "TaskManagerProvider.Endpoint";

            CreateHostBuilder(args).Build().Run();

            // Console.WriteLine("Task Manager Service started. Press any key to exit.");
            // Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).
                ConfigureWebHostDefaults(webBuilder =>
                {
                    // webBuilder.ConfigureKestrel(options =>
                    // {
                    //     // Port to use by Kestrel and thus grpc
                    //     options.ListenLocalhost(5001);
                    // });
                    webBuilder.UseStartup<Startup>();
                });
    }
}