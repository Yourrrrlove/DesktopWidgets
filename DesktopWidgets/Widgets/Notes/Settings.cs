﻿using System.Windows;
using DesktopWidgets.Classes;

namespace DesktopWidgets.Widgets.Notes
{
    public class Settings : WidgetSettings
    {
        public Settings()
        {
            Width = 160;
            Height = 200;
        }

        public string Text { get; set; }
    }
}