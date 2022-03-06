using DuxiuShovel.Zipper;
using Serilog;

namespace DuxiuShovel.Main

{
    public class Shovel
    {
        public readonly DuxiuUnzipper Zipper = new DuxiuUnzipper();
        public Shovel()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\duxiu_tools.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}