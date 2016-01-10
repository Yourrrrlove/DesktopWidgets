﻿using System.Windows.Input;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.Windows;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace DesktopWidgets.ViewModel
{
    public class ManageWidgetsViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private WidgetSettingsBase _selectedWidget;

        public ManageWidgetsViewModel()
        {
            DeselectAll = new RelayCommand(DeselectAllExecute);
            NewWidget = new RelayCommand(NewWidgetExecute);
            EditWidget = new RelayCommand(EditWidgetExecute);
            DisableWidget = new RelayCommand(DisableWidgetExecute);
            RemoveWidget = new RelayCommand(RemoveWidgetExecute);
            CloneWidget = new RelayCommand(CloneWidgetExecute);
            ExportWidget = new RelayCommand(ExportWidgetExecute);
            ImportWidget = new RelayCommand(ImportWidgetExecute);
        }

        public WidgetSettingsBase SelectedWidget
        {
            get { return _selectedWidget; }
            set
            {
                if (_selectedWidget != value)
                {
                    _selectedWidget = value;
                    RaisePropertyChanged(nameof(SelectedWidget));
                }
            }
        }

        public ICommand DeselectAll { get; private set; }

        public ICommand NewWidget { get; private set; }

        public ICommand EditWidget { get; private set; }

        public ICommand DisableWidget { get; private set; }

        public ICommand RemoveWidget { get; private set; }

        public ICommand CloneWidget { get; private set; }

        public ICommand ExportWidget { get; private set; }

        public ICommand ImportWidget { get; private set; }


        private void DeselectAllExecute()
        {
            SelectedWidget = null;
        }

        private void NewWidgetExecute()
        {
            WidgetHelper.NewWidget();
        }

        private void EditWidgetExecute()
        {
            SelectedWidget.Identifier.Edit();
        }

        private void DisableWidgetExecute()
        {
            SelectedWidget.Identifier.ToggleEnable();
            DeselectAllExecute();
        }

        private void RemoveWidgetExecute()
        {
            SelectedWidget.Identifier.Remove(true);
        }

        private void CloneWidgetExecute()
        {
            SelectedWidget.Identifier.Clone();
            DeselectAllExecute();
        }

        private void ExportWidgetExecute()
        {
            var dialog = new InputBox("Export Widget",
                JsonConvert.SerializeObject(SelectedWidget, SettingsHelper.JsonSerializerSettings));
            dialog.ShowDialog();
        }

        private void ImportWidgetExecute()
        {
            var dialog = new InputBox("Import Widget");
            dialog.ShowDialog();
            if (dialog.Cancelled)
                return;
            try
            {
                WidgetHelper.Import(JsonConvert.DeserializeObject(dialog.InputData,
                    SettingsHelper.JsonSerializerSettings));
            }
            catch
            {
                Popup.Show("Import failed. Data may be corrupt.");
            }
        }
    }
}