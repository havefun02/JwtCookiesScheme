using JwtCookiesScheme;

public class Program
{
    public static void Main(string[] args)
    {
        var Builder = CreateHostBuilder(args).Build();
        Builder.Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) => 
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilders => { webBuilders.UseStartup<Startup>(); });
}