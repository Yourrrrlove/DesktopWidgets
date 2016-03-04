﻿using System.Windows.Input;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.Properties;
using DesktopWidgets.WidgetBase.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DesktopWidgets.WindowViewModels
{
    public class ManageWidgetsViewModel : ViewModelBase
    {
        private WidgetSettingsBase _selectedWidget;

        public ManageWidgetsViewModel()
        {
            MouseDoubleClick = new RelayCommand<MouseButtonEventArgs>(MouseDoubleClickExecute);
            DeselectAll = new RelayCommand(DeselectAllExecute);
            NewWidget = new RelayCommand(NewWidgetExecute);
            EditWidget = new RelayCommand(EditWidgetExecute);
            ReloadWidget = new RelayCommand(ReloadWidgetExecute);
            MuteUnmuteWidget = new RelayCommand(MuteUnmuteWidgetExecute);
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

        public ICommand MouseDoubleClick { get; private set; }

        public ICommand DeselectAll { get; private set; }

        public ICommand NewWidget { get; private set; }

        public ICommand EditWidget { get; private set; }

        public ICommand ReloadWidget { get; private set; }

        public ICommand MuteUnmuteWidget { get; private set; }

        public ICommand DisableWidget { get; private set; }

        public ICommand RemoveWidget { get; private set; }

        public ICommand CloneWidget { get; private set; }

        public ICommand ExportWidget { get; private set; }

        public ICommand ImportWidget { get; private set; }

        private void MouseDoubleClickExecute(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                SelectedWidget?.Identifier?.GetView()?.ShowIntro(new IntroData {ExecuteFinishAction = true});
        }

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

        private void ReloadWidgetExecute()
        {
            SelectedWidget.Identifier.Reload();
            DeselectAllExecute();
        }

        private void MuteUnmuteWidgetExecute()
        {
            SelectedWidget.Identifier.ToggleMute(Settings.Default.MuteDuration);
            DeselectAllExecute();
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
            WidgetHelper.Export(SelectedWidget);
        }

        private void ImportWidgetExecute()
        {
            WidgetHelper.Import();
        }
    }
}