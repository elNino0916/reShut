using Windows.Storage;

namespace reShut.Services;

/// <summary>
/// Service for managing application settings persistence.
/// </summary>
public static class SettingsService
{
    private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

    private const string ThemeSettingKey = "AppTheme";
    private const string ConfirmationDialogsKey = "ConfirmationDialogs";
    private const string ForceCloseAppsKey = "ForceCloseApps";

    /// <summary>
    /// Gets or sets the app theme setting.
    /// Values: "Default", "Light", "Dark"
    /// </summary>
    public static string Theme
    {
        get => LocalSettings.Values[ThemeSettingKey] as string ?? "Default";
        set => LocalSettings.Values[ThemeSettingKey] = value;
    }

    /// <summary>
    /// Gets or sets whether confirmation dialogs are enabled.
    /// </summary>
    public static bool ConfirmationDialogsEnabled
    {
        get => LocalSettings.Values[ConfirmationDialogsKey] as bool? ?? true;
        set => LocalSettings.Values[ConfirmationDialogsKey] = value;
    }

    /// <summary>
    /// Gets or sets whether to force close applications on shutdown/restart.
    /// </summary>
    public static bool ForceCloseApps
    {
        get => LocalSettings.Values[ForceCloseAppsKey] as bool? ?? true;
        set => LocalSettings.Values[ForceCloseAppsKey] = value;
    }
}
