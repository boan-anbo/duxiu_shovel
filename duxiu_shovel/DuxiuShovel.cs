using DuxiuShovel.Zipper;
using Serilog;
using Serilog.Core;

namespace DuxiuShovel.Main

{
    public class Shovel
    {
        public readonly DuxiuUnzipper Zipper;

        public Shovel()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "duxiu_shovel.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true
                )
                .CreateLogger();

            try
            {
                Zipper = new DuxiuUnzipper();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            Log.Logger.Information("Duxiu Shovel started");
            Log.Logger.Information($"{Zipper.allAvailablePasswords.Length} passwords loaded");
        }
    }
}