﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DesktopWidgets.WidgetBase.Settings
{
    public class WidgetClockSettingsBase : WidgetSettingsBase
    {
        [Category("General")]
        [DisplayName("Refresh Interval")]
        public int UpdateInterval { get; set; } = -1;

        [Category("Style")]
        [DisplayName("Time Format")]
        public List<string> DateTimeFormat { get; set; } = new List<string> {"{hh}:{mm} {tt}"};

        [Category("General")]
        [DisplayName("Time Offset")]
        public TimeSpan TimeOffset { get; set; }
    }
}