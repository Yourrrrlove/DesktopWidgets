﻿using System.Windows;
using DesktopWidgets.Helpers;
using DesktopWidgets.Windows;

namespace DesktopWidgets
{
    partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
        }

        private void menuItemManageWidgets_OnClick(object sender, RoutedEventArgs e)
        {
            new ManageWidgets().Show();
        }

        private void menuItemOptions_OnClick(object sender, RoutedEventArgs e)
        {
            new Options().Show();
        }

        private void menuItemCheckForUpdates_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateHelper.CheckForUpdatesAsync(false);
        }

        private void menuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            AppHelper.ShutdownApplication();
        }

        private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            WidgetHelper.ShowAllWidgetIntros();
        }
    }
}