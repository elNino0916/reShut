using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using reShut.Services;
using System;

namespace reShut.Pages;

public sealed partial class SchedulePage : Page
{
    public SchedulePage()
    {
        InitializeComponent();
        UpdateSummary();
        HoursInput.ValueChanged += (s, e) => UpdateSummary();
        MinutesInput.ValueChanged += (s, e) => UpdateSummary();
    }

    private void UpdateSummary()
    {
        int hours = (int)HoursInput.Value;
        int minutes = (int)MinutesInput.Value;
        int totalMinutes = hours * 60 + minutes;
        
        if (totalMinutes == 0)
        {
            SummaryText.Text = "Please set a delay greater than 0.";
        }
        else
        {
            var targetTime = DateTime.Now.AddMinutes(totalMinutes);
            SummaryText.Text = $"Scheduled action will occur at {targetTime:HH:mm:ss} ({totalMinutes} minutes from now)";
        }
    }

    private void SetPreset(int minutes)
    {
        HoursInput.Value = minutes / 60;
        MinutesInput.Value = minutes % 60;
        UpdateSummary();
    }

    private void Preset15_Click(object sender, RoutedEventArgs e) => SetPreset(15);
    private void Preset30_Click(object sender, RoutedEventArgs e) => SetPreset(30);
    private void Preset60_Click(object sender, RoutedEventArgs e) => SetPreset(60);
    private void Preset120_Click(object sender, RoutedEventArgs e) => SetPreset(120);

    private void ScheduleButton_Click(object sender, RoutedEventArgs e)
    {
        int hours = (int)HoursInput.Value;
        int minutes = (int)MinutesInput.Value;
        int totalSeconds = (hours * 60 + minutes) * 60;

        if (totalSeconds == 0)
        {
            ShowStatus("Please set a delay greater than 0.", InfoBarSeverity.Warning);
            return;
        }

        var selectedRadio = ActionButtons.SelectedItem as RadioButton;
        bool isReboot = selectedRadio?.Tag?.ToString() == "restart";
        string action = isReboot ? "restart" : "shutdown";

        var result = PowerService.ScheduleShutdown(totalSeconds, isReboot);

        if (result.Success)
        {
            var targetTime = DateTime.Now.AddSeconds(totalSeconds);
            ShowStatus($"Scheduled {action} at {targetTime:HH:mm:ss}", InfoBarSeverity.Success);
        }
        else
        {
            ShowStatus($"Failed to schedule: {result.ErrorMessage}", InfoBarSeverity.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        var result = PowerService.CancelScheduledShutdown();

        if (result.Success)
        {
            ShowStatus("Scheduled shutdown/restart has been cancelled.", InfoBarSeverity.Success);
        }
        else
        {
            ShowStatus($"Failed to cancel: {result.ErrorMessage}", InfoBarSeverity.Error);
        }
    }

    private void ShowStatus(string message, InfoBarSeverity severity)
    {
        StatusBar.Message = message;
        StatusBar.Severity = severity;
        StatusBar.IsOpen = true;
    }
}
