﻿using System.ComponentModel;
using System.IO;
using System.Windows;
using DesktopWidgets.Classes;

namespace DesktopWidgets.Actions
{
    internal class PopupAction : ActionBase
    {
        public FilePath FilePath { get; set; } = new FilePath();

        [DisplayName("Text")]
        public string Text { get; set; } = "";

        [DisplayName("Input Mode")]
        public InputMode InputMode { get; set; } = InputMode.Text;

        [DisplayName("Title")]
        public string Title { get; set; } = "";

        [DisplayName("Image")]
        public MessageBoxImage Image { get; set; }

        protected override void ExecuteAction()
        {
            base.ExecuteAction();
            var input = string.Empty;
            switch (InputMode)
            {
                case InputMode.Clipboard:
                    input = Clipboard.GetText();
                    break;
                case InputMode.File:
                    input = File.ReadAllText(FilePath.Path);
                    break;
                case InputMode.Text:
                    input = Text;
                    break;
            }
            MessageBox.Show(input, Title, MessageBoxButton.OK, Image);
        }
    }
}