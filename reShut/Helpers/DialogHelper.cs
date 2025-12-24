using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace reShut.Helpers;

/// <summary>
/// Helper class to show content dialogs.
/// </summary>
public static class DialogHelper
{
    public static async System.Threading.Tasks.Task ShowErrorAsync(XamlRoot xamlRoot, string title, string message)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = xamlRoot
        };
        await dialog.ShowAsync();
    }

    public static async System.Threading.Tasks.Task ShowSuccessAsync(XamlRoot xamlRoot, string title, string message)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = xamlRoot
        };
        await dialog.ShowAsync();
    }

    public static async System.Threading.Tasks.Task<bool> ShowConfirmationAsync(XamlRoot xamlRoot, string title, string message)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = xamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}
