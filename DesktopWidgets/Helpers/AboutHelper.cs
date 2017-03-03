﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DesktopWidgets.Classes;
using DesktopWidgets.Properties;

namespace DesktopWidgets.Helpers
{
    public static class AboutHelper
    {
        public static string AboutText
        {
            get
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{AssemblyInfo.Title} ({AssemblyInfo.Version})");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Project: {Resources.GitHubMainPage}");
                stringBuilder.AppendLine($"Changes: {Resources.GitHubCommits}");
                stringBuilder.AppendLine($"Issues: {Resources.GitHubIssues}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Icon made by {IconCredits}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Libraries:");
                foreach (var library in Libraries
                    .Select(x => $"  {x.Key}: {x.Value}"))
                {
                    stringBuilder.AppendLine(library);
                }
                stringBuilder.AppendLine();
                stringBuilder.Append(AssemblyInfo.Copyright);

                return stringBuilder.ToString();
            }
        }

        private static string IconCredits { get; } =
            "Freepik (http://www.freepik.com) from www.flaticon.com" +
            " is licensed under CC BY 3.0 (http://creativecommons.org/licenses/by/3.0/)";

        private static Dictionary<string, string> Libraries { get; } = new Dictionary<string, string>
        {
            {"Common Service Locator", "commonservicelocator.codeplex.com"},
            {"Extended WPF Toolkit", "wpftoolkit.codeplex.com"},
            {"NotifyIcon", "hardcodet.net/projects/wpf-notifyicon"},
            {"MVVM Light", "galasoft.ch/mvvm"},
            {"Json.NET", "newtonsoft.com/json"},
            {"NHotkey", "github.com/thomaslevesque/NHotkey"},
            {"WpfAppBar", "github.com/PhilipRieck/WpfAppBar"}
        };

        public static string LicensesDirectory { get; } = Path.Combine(
            Path.GetDirectoryName(
                Assembly.GetExecutingAssembly()
                    .Location),
            "Resources",
            "Licenses");
    }
}