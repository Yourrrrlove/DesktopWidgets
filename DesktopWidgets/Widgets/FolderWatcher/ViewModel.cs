﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.Stores;
using DesktopWidgets.WidgetBase;
using DesktopWidgets.WidgetBase.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DesktopWidgets.Widgets.FolderWatcher
{
    public class ViewModel : WidgetViewModelBase
    {
        private readonly Queue<string> _notificationQueue;

        private string _currentFilePath;

        private BitmapImage _currentImage;
        private DirectoryWatcher _directoryWatcher;


        private FileType _fileType = FileType.None;

        private bool _isImage;

        private bool _isShowing;

        public ViewModel(WidgetId guid) : base(guid)
        {
            Settings = guid.GetSettings() as Settings;
            if (Settings == null)
                return;

            IsImage = false;

            OpenFile = new RelayCommand(OpenFileExecute);

            _notificationQueue = new Queue<string>();
            _directoryWatcher =
                new DirectoryWatcher(
                    new DirectoryWatcherSettings
                    {
                        WatchFolder = Settings.WatchFolder,
                        IncludeFilter = Settings.IncludeFilter,
                        ExcludeFilter = Settings.ExcludeFilter,
                        Recursive = Settings.Recursive,
                        CheckInterval = TimeSpan.FromMilliseconds(Settings.FolderCheckIntervalMS)
                    }, AddToFileQueue);
            _directoryWatcher.Start();
        }

        public ICommand OpenFile { get; set; }

        public Settings Settings { get; }

        public bool IsImage
        {
            get { return _isImage; }
            set
            {
                if (_isImage != value)
                {
                    _isImage = value;
                    RaisePropertyChanged(nameof(IsImage));
                }
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
            {
                _currentFilePath = value;
                RaisePropertyChanged(nameof(CurrentFilePath));
            }
        }

        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                if (!Equals(_currentImage, value))
                {
                    _currentImage = value;
                    RaisePropertyChanged(nameof(CurrentImage));
                }
            }
        }

        public FileType FileType
        {
            get { return _fileType; }
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    RaisePropertyChanged(nameof(FileType));
                }
            }
        }

        private void AddToFileQueue(FileInfo path, DirectoryChange change)
        {
            if (change == DirectoryChange.FileChanged && !Settings.DetectModifiedFiles)
                return;
            var lastCheck = Settings.LastCheck;
            Settings.LastCheck = DateTime.Now;
            if (Settings.EnableTimeout)
                if (DateTime.Now - lastCheck >= Settings.TimeoutDuration)
                    return;
            _notificationQueue.Enqueue(path.FullName);
            if (!Settings.QueueFiles || (!_isShowing && _notificationQueue.Count == 1))
                HandleDirectoryChange();
        }

        private void HandleDirectoryChange()
        {
            if (_notificationQueue.Count == 0)
                return;
            _isShowing = true;
            CurrentFilePath = _notificationQueue.Dequeue();

            if (HandleFileImage())
                FileType = FileType.Image;
            else if (HandleFileContent())
                FileType = FileType.Text;
            else if (HandleFileMedia())
                FileType = FileType.Audio;
            else
            {
                FileType = FileType.Other;
            }

            if (!App.IsMuted)
                MediaPlayerStore.PlaySoundAsync(Settings.EventSoundPath, Settings.EventSoundVolume);
            if (Settings.OpenOnEvent)
                View?.ShowIntro(Settings.OpenOnEventStay ? 0 : (int) Settings.OpenOnEventDuration.TotalMilliseconds,
                    false, false, false);
        }

        private bool HandleFileImage()
        {
            if (Settings.ShowImages && ImageHelper.IsSupported(Path.GetExtension(CurrentFilePath)))
            {
                UpdateImage(CurrentFilePath);
                return true;
            }
            return false;
        }

        private bool HandleFileContent()
        {
            var contentFilter = !string.IsNullOrWhiteSpace(Settings.ShowContentFilter)
                ? Settings.ShowContentFilter.Split('|')
                : null;
            return contentFilter != null &&
                   contentFilter.Any(
                       x => x.EndsWith(Path.GetExtension(CurrentFilePath), StringComparison.OrdinalIgnoreCase));
        }

        private bool HandleFileMedia()
        {
            if (!Settings.PlayMedia)
                return false;
            MediaPlayerStore.PlaySoundAsync(CurrentFilePath, Settings.PlayMediaVolume);
            return MediaPlayerHelper.IsSupported(Path.GetExtension(CurrentFilePath));
        }

        public override void OnIntroEnd()
        {
            base.OnIntroEnd();
            _isShowing = false;
            HandleDirectoryChange();
        }

        private void UpdateImage(string imagePath)
        {
            var bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmi.CacheOption = BitmapCacheOption.OnLoad;
            bmi.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bmi.EndInit();

            CurrentImage = bmi;
        }

        private void OpenFileExecute()
        {
            ProcessHelper.Launch(CurrentFilePath);
            Hide();
        }

        private void Hide()
        {
            View?.HideUi();
        }

        public override void OnClose()
        {
            base.OnClose();
            _directoryWatcher.Stop();
            _directoryWatcher = null;
        }

        public override void OnRefresh()
        {
            base.OnRefresh();
            _directoryWatcher.SetWatchPath(Settings.WatchFolder);
        }
    }
}