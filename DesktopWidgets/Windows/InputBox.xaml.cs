﻿#region

using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;

#endregion

namespace DesktopWidgets.Windows
{
    /// <summary>
    ///     Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window, INotifyPropertyChanged
    {
        private string _inputData;

        public InputBox(string title, string displayData = null)
        {
            InitializeComponent();
            Title = title;
            InputData = displayData;
            IsDisplayData = !string.IsNullOrEmpty(displayData);
            if (IsDisplayData)
                txtData.SelectAll();
            DataContext = this;
            txtData.Focus();
        }

        public bool Cancelled { get; private set; }
        public bool IsDisplayData { get; }

        public string InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData != value)
                {
                    _inputData = value;
                    RaisePropertyChanged(nameof(InputData));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            DialogResult = true;
        }

        private void btnSave_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*"
            };
            dialog.ShowDialog();
            File.WriteAllText(dialog.FileName, InputData);
        }

        private void btnCopy_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(InputData);
        }
    }
}