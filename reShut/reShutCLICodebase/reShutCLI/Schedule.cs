using System;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;

namespace reShutCLI
{
    internal class Schedule
    {
        public static bool Plan()
        {
            CultureInfo culture = new CultureInfo(Variables.lang);
            ResourceManager rm = new ResourceManager("reShutCLI.Resources.Strings", typeof(Program).Assembly);

            string type = "";

            while (true) // Action selection
            {
                Console.Clear();
                UIDraw.TextColor = Variables.MenuColor;
                UIDraw.DrawCenteredLine("╭──────────────────────────────────╮");
                UIDraw.DrawCenteredLine(string.Format("│ {0,-32} │", rm.GetString("Schedule_PromptAction", culture)));
                UIDraw.DrawCenteredLine(string.Format("│ {0,-32} │", rm.GetString("Schedule_SelectOption", culture)));
                UIDraw.DrawCenteredLine(string.Format("│ 1) {0,-29} │", rm.GetString("Schedule_ShutdownOption", culture)));
                UIDraw.DrawCenteredLine(string.Format("│ 2) {0,-29} │", rm.GetString("Schedule_RebootOption", culture)));
                UIDraw.DrawCenteredLine("├──────────────────────────────────┤");
                UIDraw.DrawCenteredLine(string.Format("│ 0) {0,-29} │", rm.GetString("Schedule_Cancel", culture)));
                UIDraw.DrawCenteredLine(string.Format("│ 9) {0,-29} │", rm.GetString("Back", culture)));
                UIDraw.DrawCenteredLine("╰──────────────────────────────────╯");

                var key = Console.ReadKey().KeyChar.ToString();

                if (key == "0")
                {
                    Process.Start(@"cmd.exe", "/c shutdown -a");
                    Console.Clear();
                    UIDraw.TextColor = ConsoleColor.Green;
                    UIDraw.DrawBoxedMessage(rm.GetString("Schedule_ActionCancelled", culture));
                    Thread.Sleep(500);
                    return false;
                }
                else if (key == "1")
                {
                    type = rm.GetString("Shutdown", culture).ToLower();
                    break;
                }
                else if (key == "2")
                {
                    type = rm.GetString("Reboot", culture).ToLower();
                    break;
                }
                else if (key == "9")
                {
                    Console.Clear();
                    return false;
                }
                else
                {
                    UIDraw.TextColor = ConsoleColor.Red;
                    UIDraw.DrawBoxedMessage(rm.GetString("ErrorOccurred", culture));
                    UIDraw.TextColor = ConsoleColor.White;
                }
            }

            DateTime targetTime;
            while (true)
            {
                Console.Clear();
                UIDraw.TextColor = Variables.MenuColor;

                string timeDisplay = DateTime.Now.ToString("tt") == ""
                    ? Time.GetTime(true)
                    : Time.GetTime(false);

                UIDraw.DrawBoxedMessage(string.Format(" {0} {1} ", rm.GetString("Schedule_CurrentTime", culture), timeDisplay));
                UIDraw.DrawBoxedMessage("Enter time (minutes, 'in 2 hours', or 'yyyy-MM-dd HH:mm')");
                UIDraw.TextColor = ConsoleColor.White;
                UIDraw.Draw(rm.GetString("Schedule_InputPrompt", culture) + " ");

                string inputStr = Console.ReadLine();
                if (int.TryParse(inputStr, out int minutes))
                {
                    targetTime = DateTime.Now.AddMinutes(minutes);
                    break;
                }

                var match = Regex.Match(inputStr, @"in\s+(\d+)\s*(hour|hours|minute|minutes)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    int val = int.Parse(match.Groups[1].Value);
                    string unit = match.Groups[2].Value.ToLower();
                    targetTime = unit.StartsWith("hour") ? DateTime.Now.AddHours(val) : DateTime.Now.AddMinutes(val);
                    break;
                }

                if (DateTime.TryParse(inputStr, out targetTime))
                    break;

                UIDraw.TextColor = ConsoleColor.Red;
                UIDraw.DrawBoxedMessage("Could not parse time input.");
            }

            while (true)
            {
                Console.Clear();
                int seconds = Math.Max(0, (int)(targetTime - DateTime.Now).TotalSeconds);
                int minutes = (int)Math.Ceiling((targetTime - DateTime.Now).TotalMinutes);
                int hours = minutes / 60;

                UIDraw.TextColor = Variables.MenuColor;

                string translatedType = type.Equals(rm.GetString("Shutdown", culture).ToLower(), StringComparison.OrdinalIgnoreCase)
                    ? rm.GetString("Shutdown", culture)
                    : rm.GetString("Reboot", culture);

                string header = string.Format(rm.GetString("Schedule_ConfirmActionSeconds", culture), translatedType, minutes);
                string info = string.Format(rm.GetString("Schedule_TimeBreakdown", culture), hours);
                string option1 = $"1) {string.Format(rm.GetString("Schedule_ConfirmYes", culture), translatedType)}";
                string option2 = $"2) {rm.GetString("Schedule_ConfirmNoReenter", culture)}";
                string option0 = $"0) {rm.GetString("BackToMainMenu", culture)}";

                int maxLength = Math.Max(header.Length, Math.Max(option1.Length, Math.Max(option2.Length, option0.Length)));
                int borderLength = maxLength + 4;

                UIDraw.DrawCenteredLine("╭" + new string('─', borderLength) + "╮");
                UIDraw.DrawCenteredLine("│ " + header.PadRight(maxLength) + "   │");
                UIDraw.DrawCenteredLine("│ " + info.PadRight(maxLength) + "   │");
                UIDraw.DrawCenteredLine("├" + new string('─', borderLength) + "┤");
                UIDraw.DrawCenteredLine("│ " + option1.PadRight(maxLength) + "   │");
                UIDraw.DrawCenteredLine("│ " + option2.PadRight(maxLength) + "   │");
                UIDraw.DrawCenteredLine("│ " + option0.PadRight(maxLength) + "   │");
                UIDraw.DrawCenteredLine("╰" + new string('─', borderLength) + "╯");

                UIDraw.TextColor = ConsoleColor.White;
                UIDraw.Draw(rm.GetString("Schedule_InputPrompt", culture) + " ");

                string confirmKey = Console.ReadKey().KeyChar.ToString();
                Console.Clear();

                if (confirmKey == "0")
                {
                    return false;
                }
                else if (confirmKey == "2")
                {
                    return Plan();
                }
                else if (confirmKey == "1")
                {
                    try
                    {
                        string character = type.Equals(rm.GetString("Shutdown", culture).ToLower(), StringComparison.OrdinalIgnoreCase) ? "s" : "r";
                        UIDraw.TextColor = Variables.MenuColor;
                        UIDraw.DrawBoxedMessage("Recurring schedule? (d)aily, (w)eekly, (n)one");
                        UIDraw.TextColor = ConsoleColor.White;
                        var recur = Console.ReadKey().KeyChar.ToString().ToLower();
                        Console.Clear();
                        if (recur == "d" || recur == "w")
                        {
                            string scheduleType = recur == "d" ? "DAILY" : "WEEKLY";
                            string taskName = $"reShutCLI_{Guid.NewGuid().ToString("N")}";
                            string st = targetTime.ToString("HH:mm");
                              var psi = new ProcessStartInfo
                              {
                                  FileName = "schtasks",
                                  RedirectStandardOutput = true,
                                  RedirectStandardError = true,
                                  UseShellExecute = false,
                                  CreateNoWindow = true
                              };
                              psi.ArgumentList.Add("/Create");
                              psi.ArgumentList.Add("/TN");
                              psi.ArgumentList.Add(taskName);
                              psi.ArgumentList.Add("/TR");
                                psi.ArgumentList.Add($"shutdown /{character} /f");
                              psi.ArgumentList.Add("/SC");
                              psi.ArgumentList.Add(scheduleType);
                              psi.ArgumentList.Add("/ST");
                              psi.ArgumentList.Add(st);
                            using (var process = Process.Start(psi))
                            {
                                process.WaitForExit();
                                if (process.ExitCode != 0)
                                {
                                    string errorOutput = process.StandardError.ReadToEnd();
                                    UIDraw.TextColor = ConsoleColor.Red;
                                    UIDraw.DrawBoxedMessage(rm.GetString("Schedule_ErrorOccurred", culture) + (string.IsNullOrWhiteSpace(errorOutput) ? "" : $"\n{errorOutput}"));
                                    UIDraw.TextColor = ConsoleColor.White;
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            Process.Start("cmd.exe", $"/c shutdown /{character} /f /t {seconds}");
                        }
                        UIDraw.TextColor = ConsoleColor.Green;
                        UIDraw.DrawBoxedMessage(rm.GetString("Schedule_ActionScheduled", culture));
                        UIDraw.TextColor = ConsoleColor.White;
                        return true;
                    }
                    catch
                    {
                        UIDraw.TextColor = ConsoleColor.Red;
                        UIDraw.DrawBoxedMessage(rm.GetString("Schedule_ErrorOccurred", culture));
                        UIDraw.TextColor = ConsoleColor.White;
                        return false;
                    }
                }
                else
                {
                    UIDraw.TextColor = ConsoleColor.Red;
                    UIDraw.DrawBoxedMessage(rm.GetString("Schedule_ErrorOccurred", culture));
                    UIDraw.TextColor = ConsoleColor.White;
                }
            }
        }
    }
}
