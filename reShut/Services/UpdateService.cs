using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace reShut.Services;

/// <summary>
/// Service for checking for application updates from GitHub.
/// </summary>
public static class UpdateService
{
    public static string CurrentVersion => AppInfo.Version;

    public static string GitHubUrl => AppInfo.GitHubUrl;

    public record UpdateCheckResult(
        bool Success,
        bool UpdateAvailable,
        string? LatestVersion,
        string? ReleaseUrl,
        string? ErrorMessage
    );

    public static async Task<UpdateCheckResult> CheckForUpdatesAsync()
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"{AppInfo.AppName}-UpdateChecker/{AppInfo.Version}");
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync(AppInfo.GitHubReleasesApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return new UpdateCheckResult(
                    Success: false,
                    UpdateAvailable: false,
                    LatestVersion: null,
                    ReleaseUrl: null,
                    ErrorMessage: $"GitHub API returned {(int)response.StatusCode}: {response.ReasonPhrase}"
                );
            }

            var json = await response.Content.ReadAsStringAsync();
            var release = JsonSerializer.Deserialize(json, GitHubReleaseContext.Default.GitHubRelease);

            if (release?.tag_name == null)
            {
                return new UpdateCheckResult(
                    Success: false,
                    UpdateAvailable: false,
                    LatestVersion: null,
                    ReleaseUrl: null,
                    ErrorMessage: "Could not parse release information from GitHub."
                );
            }

            // Remove 'v' prefix if present
            string latestVersion = release.tag_name.TrimStart('v', 'V');
            bool updateAvailable = IsNewerVersionAvailable(CurrentVersion, latestVersion);

            return new UpdateCheckResult(
                Success: true,
                UpdateAvailable: updateAvailable,
                LatestVersion: latestVersion,
                ReleaseUrl: release.html_url ?? $"{AppInfo.GitHubUrl}/releases/latest",
                ErrorMessage: null
            );
        }
        catch (TaskCanceledException)
        {
            return new UpdateCheckResult(
                Success: false,
                UpdateAvailable: false,
                LatestVersion: null,
                ReleaseUrl: null,
                ErrorMessage: "Update check timed out. Please check your internet connection."
            );
        }
        catch (HttpRequestException ex)
        {
            return new UpdateCheckResult(
                Success: false,
                UpdateAvailable: false,
                LatestVersion: null,
                ReleaseUrl: null,
                ErrorMessage: $"Network error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            return new UpdateCheckResult(
                Success: false,
                UpdateAvailable: false,
                LatestVersion: null,
                ReleaseUrl: null,
                ErrorMessage: $"Error checking for updates: {ex.Message}"
            );
        }
    }

    private static bool IsNewerVersionAvailable(string currentVersion, string latestVersion)
    {
        if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(latestVersion))
            return false;

        try
        {
            return new Version(latestVersion) > new Version(currentVersion);
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// GitHub release information from the API.
/// </summary>
public class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string? tag_name { get; set; }

    [JsonPropertyName("html_url")]
    public string? html_url { get; set; }
}

/// <summary>
/// Source-generated JSON serialization context for AOT compatibility.
/// </summary>
[JsonSerializable(typeof(GitHubRelease))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
internal partial class GitHubReleaseContext : JsonSerializerContext
{
}
