using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace reShutCLI
{
    internal class AutoUpdater
    {
        private const string RepositoryUrl = "https://api.github.com/repos/elnino0916/reshut-cli/releases/latest";
        private const string UserAgent = "reShutCLI_AutoUpdater";

        public static async Task PerformUpdate()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

                var response = await client.GetAsync(RepositoryUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var release = JsonSerializer.Deserialize<UpdateChecker.GitHubRelease>(json);

                    var latestVersion = release.tag_name;
                    var currentVersion = Variables.version;

                    if (UpdateChecker.IsNewerVersionAvailable(currentVersion, latestVersion))
                    {
                        var asset = release.assets[0]; // Assuming the first asset is the installer
                        var downloadUrl = asset.browser_download_url;
                        var installerPath = await DownloadInstaller(downloadUrl);

                        if (!string.IsNullOrEmpty(installerPath))
                        {
                            StartInstaller(installerPath);
                        }
                        else
                        {
                            UIDraw.TextColor = ConsoleColor.Red;
                            ErrorHandler.ShowError("Failed to download the installer.", true);
                            UIDraw.TextColor = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        UIDraw.TextColor = Variables.MenuColor;
                        ErrorHandler.ShowError("Tried to update to the same version currently installed.", true);
                        UIDraw.TextColor = ConsoleColor.Gray;
                    }
                }
                else
                {
                    UIDraw.TextColor = ConsoleColor.Red;
                    ErrorHandler.ShowError($"Failed to check for updates: {response.StatusCode}", true);
                    UIDraw.TextColor = ConsoleColor.Gray;
                }
            }
            catch (Exception ex)
            {
                UIDraw.TextColor = ConsoleColor.Red;
                ErrorHandler.ShowError($"Failed to update: {ex.Message}", true);
                UIDraw.TextColor = ConsoleColor.Gray;
            }
        }

        private static async Task<string> DownloadInstaller(string downloadUrl)
        {
            try
            {
                var fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                using var client = new HttpClient();
                var response = await client.GetAsync(downloadUrl);

                response.EnsureSuccessStatusCode();

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);

                return filePath;
            }
            catch (Exception ex)
            {
                UIDraw.TextColor = ConsoleColor.Red;
                ErrorHandler.ShowError($"Failed to download the installer: {ex.Message}", true);
                UIDraw.TextColor = ConsoleColor.Gray;
                return string.Empty;
            }
        }

        private static void StartInstaller(string installerPath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    Arguments = "/SILENT /CLOSEAPPLICATIONS /NOCANCEL /NORESTART",
                    UseShellExecute = true,
                };

                Process.Start(startInfo);

                // Exit the current application
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                UIDraw.TextColor = ConsoleColor.Red;
                ErrorHandler.ShowError($"Failed to start the installer: {ex.Message}", true);
                UIDraw.TextColor = ConsoleColor.Gray;
            }
        }
    }
}