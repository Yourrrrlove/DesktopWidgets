﻿using System;
using System.ComponentModel;
using DesktopWidgets.Classes;

namespace DesktopWidgets.Widgets.FolderWatcher
{
    public class Settings : WidgetSettingsBase, IEventWidget
    {
        public Settings()
        {
            Width = 384;
            Height = 216;
        }

        [Category("General")]
        [DisplayName("Folder Check Interval (ms)")]
        public int FolderCheckIntervalMS { get; set; } = 500;

        [Category("General")]
        [DisplayName("Watch Folder")]
        public string WatchFolder { get; set; } = "";

        [Category("General")]
        [DisplayName("Include Filter")]
        public string IncludeFilter { get; set; } = "*.*";

        [Category("General")]
        [DisplayName("Exclude Filter")]
        public string ExcludeFilter { get; set; } = "";

        [Category("Style")]
        [DisplayName("Show File Name")]
        public bool ShowFileName { get; set; } = true;

        [Category("General")]
        [DisplayName("Timeout Duration")]
        public TimeSpan TimeoutDuration { get; set; } = TimeSpan.FromMinutes(1);

        [DisplayName("Last File Check")]
        public DateTime LastCheck { get; set; } = DateTime.Now;

        [Category("General")]
        [DisplayName("Enable Timeout")]
        public bool EnableTimeout { get; set; } = false;

        [Category("Behavior")]
        [DisplayName("Event Sound Path")]
        public string EventSoundPath { get; set; }

        [Category("Behavior")]
        [DisplayName("Event Sound Volume")]
        public double EventSoundVolume { get; set; } = 1.0;

        [Category("Behavior")]
        [DisplayName("Replace Existing File")]
        public bool ReplaceExistingFile { get; set; } = false;

        [Category("General")]
        [DisplayName("Recursive")]
        public bool Recursive { get; set; } = false;

        [Category("Behavior (Hideable)")]
        [DisplayName("Open On Event")]
        public bool OpenOnEvent { get; set; } = true;

        [Category("Behavior (Hideable)")]
        [DisplayName("Stay Open On Event")]
        public bool OpenOnEventStay { get; set; } = false;

        [Category("Behavior (Hideable)")]
        [DisplayName("Open On Event Duration")]
        public TimeSpan OpenOnEventDuration { get; set; } = TimeSpan.FromSeconds(10);
    }
}