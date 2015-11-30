﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DesktopWidgets.Classes;
using DesktopWidgets.View;
using DesktopWidgets.Widgets.TimeClock;
using DesktopWidgets.Windows;

namespace DesktopWidgets.Helpers
{
    public static class WidgetHelper
    {
        private static readonly List<string> AvailableWidgets = new List<string>
        {
            Metadata.FriendlyName,
            Widgets.CountdownClock.Metadata.FriendlyName,
            Widgets.StopwatchClock.Metadata.FriendlyName
        };

        public static WidgetSettings GetWidgetSettingsFromId(WidgetId id)
        {
            return App.WidgetsSettingsStore.Widgets.First(v => v.ID == id);
        }

        public static WidgetView GetWidgetViewFromId(WidgetId id)
        {
            return App.WidgetViews.First(v => v.ID == id);
        }

        public static string GetWidgetName(WidgetId id)
        {
            var settings = GetWidgetSettingsFromId(id);
            var index = App.WidgetsSettingsStore.Widgets.IndexOf(settings);
            var name = (settings.Name == "" ? $"Widget {index + 1}" : settings.Name);
            return $"{name}";
        }

        public static void NewWidget()
        {
            var dialog =
                new SelectItem(AvailableWidgets);
            dialog.ShowDialog();
            AddWidget((string) dialog.SelectedItem);
        }

        private static void AddWidget(string type)
        {
            WidgetSettings newWidget;
            switch (type)
            {
                case Metadata.FriendlyName:
                    newWidget = new Settings();
                    break;
                case Widgets.CountdownClock.Metadata.FriendlyName:
                    newWidget = new Widgets.CountdownClock.Settings();
                    break;
                case Widgets.StopwatchClock.Metadata.FriendlyName:
                    newWidget = new Widgets.StopwatchClock.Settings();
                    break;
                default:
                    return;
            }
            App.WidgetsSettingsStore.Widgets.Add(newWidget);
            LoadWidget(newWidget.ID);
        }

        private static void DisableWidget(WidgetId id)
        {
            var settings = GetWidgetSettingsFromId(id);
            if (settings.Disabled)
                return;
            settings.Disabled = true;
            var view = GetWidgetViewFromId(id);
            view.Close();
            App.WidgetViews.Remove(view);
        }

        private static void EnableWidget(WidgetId id)
        {
            var settings = GetWidgetSettingsFromId(id);
            if (!settings.Disabled)
                return;
            settings.Disabled = false;
            LoadWidget(id);
        }

        public static void ToggleWidgetEnabled(WidgetId id)
        {
            if (GetWidgetSettingsFromId(id).Disabled)
                EnableWidget(id);
            else
                DisableWidget(id);
        }

        public static void RemoveWidget(WidgetId id, bool msg = false)
        {
            if (msg && Popup.Show("Are you sure you want to delete this widget?\n\nThis cannot be undone.",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
                return;

            var view = GetWidgetViewFromId(id);
            var settings = GetWidgetSettingsFromId(id);
            view.Close();
            App.WidgetsSettingsStore.Widgets.Remove(settings);
            App.WidgetViews.Remove(view);
        }

        public static void EditWidget(WidgetId id)
        {
            new PropertyView(GetWidgetSettingsFromId(id)).ShowDialog();
        }

        public static void LoadWidget(WidgetId id)
        {
            var settings = GetWidgetSettingsFromId(id);
            var widgetView = new WidgetView(id);
            var userControlStyle = (Style) widgetView.FindResource("UserControlStyle");
            UserControl userControl;
            object dataContext;

            if (settings is Settings)
            {
                dataContext = new Widgets.TimeClock.ViewModel(id);
                userControl = new ControlView();
            }
            else if (settings is Widgets.CountdownClock.Settings)
            {
                dataContext = new Widgets.CountdownClock.ViewModel(id);
                userControl = new Widgets.CountdownClock.ControlView();
            }
            else if (settings is Widgets.StopwatchClock.Settings)
            {
                dataContext = new Widgets.StopwatchClock.ViewModel(id);
                userControl = new Widgets.StopwatchClock.ControlView();
            }
            else
            {
                return;
            }

            userControl.Style = userControlStyle;
            widgetView.DataContext = dataContext;
            widgetView.MainContentContainer.Child = userControl;
            App.WidgetViews.Add(widgetView);

            widgetView.Show();
        }

        public static void LoadWidgets()
        {
            if (App.WidgetViews != null)
                foreach (var view in App.WidgetViews)
                    view.Close();
            App.WidgetViews = new ObservableCollection<WidgetView>();

            if (App.WidgetsSettingsStore == null)
                App.WidgetsSettingsStore = new WidgetsSettingsStore
                {
                    Widgets = new ObservableCollection<WidgetSettings>()
                };

            foreach (
                var id in App.WidgetsSettingsStore.Widgets.Where(x => !x.Disabled).Select(settings => settings.ID))
                LoadWidget(id);
        }
    }
}