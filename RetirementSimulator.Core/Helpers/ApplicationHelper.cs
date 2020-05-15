namespace RetirementSimulator.Core.Helpers
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class ApplicationHelper
    {
        static ApplicationHelper()
        {
            CompanyName = "twentySix";
            Title = "RetirementSimulator";

            var exeAssemblyNumber = Assembly.GetEntryAssembly()?.GetName().Version;
            AssemblyVersionNumber = exeAssemblyNumber?.ToString(4);
        }

        public static string AssemblyVersionNumber { get; }

        public static string Title { get; }

        public static string CompanyName { get; }

        public static string GetAppDataFolder()
        {
            var appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName, Title);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return appFolder;
        }
    }
}