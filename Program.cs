using JwtCookiesScheme;
using Serilog;
using System.Security.Principal;
public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
        .CreateLogger();
        var Builder = CreateHostBuilder(args).Build();
        if (Builder.Services == null)
        {
            throw new Exception("Can not build services");
        }
        Task.WaitAll(Seed.SeedingDataAsync(Builder.Services));
        Builder.Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) => 
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilders => { webBuilders.UseStartup<Startup>(); });

}