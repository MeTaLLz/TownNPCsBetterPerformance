namespace TownNPCsFreeze
{
    public static class FileLogger
    {
        private const string Prefix = ModConstants.Prefix;

        public static void Warn(string className, string message)
        {
            ModInstance.Mod.Logger.Warn($"{Prefix} [{className}] {message}");
        }

        public static void Error(string className, string message)
        {
            ModInstance.Mod.Logger.Error($"{Prefix} [{className}] {message}");
        }
    }
}