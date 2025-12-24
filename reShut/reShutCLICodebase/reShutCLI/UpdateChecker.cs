using System;
using System.Globalization;
using System.Net.Http;
using System.Resources;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace reShutCLI
{
    internal class UpdateChecker
    {
        private static bool updateCheckPerformed = false;
        private static string updateResultMessage = string.Empty;
        private static string updateResultMessageLine2 = string.Empty;
        private static string updateResultMessageLine3 = string.Empty;


        public static async Task MainCheck()
        {
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);
            // Check if update has already been checked
            if (updateCheckPerformed)
            {
                // Output the stored result
                UIDraw.TextColor = Variables.MenuColor;
                UIDraw.DrawCenteredLine(updateResultMessage);
                UIDraw.DrawCenteredLine(updateResultMessageLine2);
                UIDraw.DrawCenteredLine(updateResultMessageLine3);
                UIDraw.TextColor = ConsoleColor.Gray;
                return;
            }

            const string repositoryUrl = "https://api.github.com/repos/elnino0916/reshut-cli/releases/latest";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("reShutCLI_UpdateSearch");

            var response = await client.GetAsync(repositoryUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<GitHubRelease>(json);

                var latestVersion = release.tag_name;
                var currentVersion = Variables.version;

                if (IsNewerVersionAvailable(currentVersion, latestVersion))
                {
                    UIDraw.TextColor = Variables.MenuColor;

                    UIDraw.DrawBoxedMessage(rm.GetString("UpdateAvailable", culture));

                    Variables.isUpToDate = false;
                    UIDraw.TextColor = ConsoleColor.Gray;

                    // Check if auto-update is enabled in the registry
                    if (RegistryWorker.ReadFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\reShutCLI\config\", "AutoUpdateOnStart") == "yes")
                    {
                        if (!Variables.DevelopmentBuild)
                        {
                            await AutoUpdater.PerformUpdate();
                        }
                    }
                }
                else
                {
                    Variables.isUpToDate = true;
                    UIDraw.TextColor = Variables.MenuColor;
                    updateResultMessage = Variables.Motd();
                    UIDraw.DrawCenteredLine(updateResultMessage);
                    UIDraw.TextColor = ConsoleColor.Gray;
                }
            }
            else
            {
                UIDraw.TextColor = ConsoleColor.Red;
                UIDraw.DrawBoxedMessage($"Failed to check for updates: {response.StatusCode}. Restart the application to try again.");
                UIDraw.TextColor = ConsoleColor.Gray;
            }

            // Set the flags to indicate that update check has been performed
            updateCheckPerformed = true;
        }


        public static bool IsNewerVersionAvailable(string currentVersion, string latestVersion)
        {
            if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(latestVersion))
                return false;

            return new Version(latestVersion) > new Version(currentVersion);
        }

        public class GitHubRelease
        {
#pragma warning disable IDE1006
            public string tag_name { get; set; }
            public Asset[] assets { get; set; }
#pragma warning restore IDE1006

            public class Asset
            {
                public string browser_download_url { get; set; }
            }
        }
    }
}