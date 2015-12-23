﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.ViewModelBase;
using NHotkey;
using NHotkey.Wpf;

namespace DesktopWidgets.View
{
    /// <summary>
    ///     Interaction logic for View.xaml
    /// </summary>
    public partial class WidgetView : Window
    {
        private readonly MouseChecker _mouseChecker;
        public readonly WidgetSettingsBase Settings;
        public readonly UserControl UserControl;
        public readonly WidgetViewModelBase ViewModel;
        private DispatcherTimer _introTimer;
        private DispatcherTimer _onTopForceTimer;
        public bool NeedUpdate;

        public bool QueueIntro;

        public WidgetView(WidgetId id, WidgetViewModelBase viewModel, UserControl userControl)
        {
            InitializeComponent();
            HideOpacity();
            Visibility = Visibility.Visible;
            Id = id;
            Settings = id.GetSettings();
            ViewModel = viewModel;
            UserControl = userControl;

            if (!App.Arguments.Contains("-systemstartup"))
                QueueIntro = true;

            userControl.Style = (Style) FindResource("UserControlStyle");
            MainContentContainer.Content = userControl;
            MainContentContainer.ContextMenu =
                (ContextMenu)
                    (userControl.TryFindResource("WidgetContextMenu") ?? TryFindResource("WidgetContextMenu"));
            userControl.MouseDown += OnMouseDown;

            var frameTop = userControl.TryFindResource("FrameTop") as Grid;
            if (frameTop != null)
                FrameContainerTop.Child = frameTop;
            var frameBottom = userControl.TryFindResource("FrameBottom") as Grid;
            if (frameBottom != null)
                FrameContainerBottom.Child = frameBottom;

            _mouseChecker = new MouseChecker(this, Settings);
            NeedUpdate = true;
            UpdateUi();
            _mouseChecker.Start();
        }

        public bool IsContextMenuOpen => ViewModel.IsContextMenuOpen;

        public WidgetId Id { get; private set; }
        public bool AnimationRunning { get; set; } = false;

        public new bool IsVisible => !(Opacity < 1);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            var widgetSrc = HwndSource.FromHwnd(hwnd);

            widgetSrc?.AddHook(WndProc);

            if (Settings.Unclickable)
                new Win32App(hwnd).SetWindowExTransparent();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == SingleInstanceHelper.WM_SHOWAPP)
                ShowIntro();

            return IntPtr.Zero;
        }

        private void OnHotKey(object sender, HotkeyEventArgs e)
        {
            if (e.Name == "Show")
                ShowIntro();
            e.Handled = true;
        }

        public void ShowIntro(int duration = -1, bool reversable = true)
        {
            if (Settings.OpenMode == OpenMode.AlwaysOpen || !Settings.ShowIntro)
                return;

            if (NeedUpdate)
            {
                QueueIntro = true;
                return;
            }
            QueueIntro = false;

            if (_introTimer == null)
            {
                _introTimer = new DispatcherTimer();
                _introTimer.Tick += delegate
                {
                    _introTimer.Stop();
                    _mouseChecker.KeepOpenForIntro = false;
                };
            }

            _introTimer.Stop();
            _introTimer.Interval = TimeSpan.FromMilliseconds(duration == -1 ? Settings.IntroDuration : duration);
            if (_mouseChecker.KeepOpenForIntro && reversable)
            {
                _mouseChecker.KeepOpenForIntro = false;
            }
            else
            {
                _mouseChecker.KeepOpenForIntro = true;
                if (duration != 0)
                    _introTimer.Start();
            }
        }

        public void ShowUI()
        {
            _mouseChecker.Show();
        }

        public void HideUI()
        {
            _mouseChecker.Hide();
        }

        private void ReloadHotKeys()
        {
            try
            {
                if (Settings.OpenMode == OpenMode.Keyboard || Settings.OpenMode == OpenMode.MouseAndKeyboard)
                {
                    HotkeyManager.Current.AddOrReplace("Show", Settings.HotKey, Settings.HotKeyModifiers, OnHotKey);
                }
            }
            catch (HotkeyAlreadyRegisteredException)
            {
            }
        }


        public void UpdateUi(ScreenDockPosition? dockPosition = null,
            ScreenDockAlignment? dockAlignment = null)
        {
            if (!IsVisible)
                Refresh(false);
            else
                this.Animate(AnimationMode.Hide, null, () => Refresh(true), dockPosition, dockAlignment);
        }

        private void Refresh(bool showIntro)
        {
            UpdateLayout();
            DataContext = null;
            UpdateLayout();
            DataContext = ViewModel;
            UpdateLayout();
            ViewModel.UpdateSize();
            UpdateLayout();
            ViewModel.UpdatePosition();
            UpdateLayout();
            ViewModel.UpdatePosition();
            UpdateLayout();
            UpdateTimers();
            ReloadHotKeys();
            if (showIntro && !_mouseChecker.KeepOpenForIntro)
                ShowIntro();
        }

        private void UpdateTimers()
        {
            _mouseChecker.UpdateIntervals();
            if (_mouseChecker.IsRunning)
            {
                _mouseChecker.Stop();
                _mouseChecker.Start();
            }
            if (Settings.ForceOnTop && Settings.ForceOnTopInterval > 0)
            {
                if (_onTopForceTimer == null)
                {
                    _onTopForceTimer = new DispatcherTimer();
                    _onTopForceTimer.Tick += delegate
                    {
                        Settings.OnTop = false;
                        Settings.OnTop = true;
                    };
                }
                _onTopForceTimer.Interval = TimeSpan.FromMilliseconds(Settings.ForceOnTopInterval);
                if (_onTopForceTimer.IsEnabled)
                {
                    _onTopForceTimer.Stop();
                    _onTopForceTimer.Start();
                }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && Settings.DockPosition == ScreenDockPosition.None &&
                Settings.DragToMove)
                DragMove();
        }

        private void WidgetView_OnClosing(object sender, CancelEventArgs e)
        {
            _mouseChecker.Stop();
        }

        private void WidgetView_OnLocationChanged(object sender, EventArgs e)
        {
            if (Settings.SnapToScreenEdges)
                this.Snap();
        }

        public void ShowOpacity()
        {
            Opacity = 1;
        }

        public void HideOpacity()
        {
            Opacity = 0;
        }
    }
}