using Microsoft.Win32;
using MVVM.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace MVVM.Services
{
    public class UserDialogService : IUserDialog
    {
        /// <summary>App Active window</summary>
        protected static Window? ActiveWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

        /// <summary>App Focused window</summary>
        protected static Window? FocusedWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsFocused);

        /// <summary>App current window</summary>
        protected static Window? CurrentWindow => FocusedWindow ?? ActiveWindow;

        #region [Open/Save]

        /// <inheritdoc/>
        public virtual FileInfo? OpenFile(string Title, string Filter = "All files (*.*)|*.*", string? DefaultFilePath = null)
        {
            var dialog = new OpenFileDialog
            {
                Title = Title,
                RestoreDirectory = true,
                Filter = Filter ?? throw new ArgumentNullException(nameof(Filter)),
            };
            if (DefaultFilePath is { Length: > 0 })
                dialog.FileName = DefaultFilePath;

            return dialog.ShowDialog(CurrentWindow) == true
                ? new(dialog.FileName)
                : DefaultFilePath is null ? null : new(DefaultFilePath);
        }

        /// <inheritdoc/>
        public virtual FileInfo? SaveFile(string Title, string Filter = "All files (*.*)|*.*", string? DefaultFilePath = null)
        {
            var dialog = new SaveFileDialog
            {
                Title = Title,
                RestoreDirectory = true,
                Filter = Filter ?? throw new ArgumentNullException(nameof(Filter)),
            };
            if (DefaultFilePath is { Length: > 0 })
                dialog.FileName = DefaultFilePath;

            return dialog.ShowDialog(CurrentWindow) == true
                ? new(dialog.FileName)
                : DefaultFilePath is null ? null : new(DefaultFilePath);
        }

        #endregion [Open/Save]

        #region [Question dialog]

        /// <inheritdoc/>
        public virtual bool YesNoQuestion(string Text, string Title = "Question...")
        {
            var result = CurrentWindow is null
                ? MessageBox.Show(Text, Title, MessageBoxButton.YesNo, MessageBoxImage.Question)
                : MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <inheritdoc/>
        public virtual bool OkCancelQuestion(string Text, string Title = "Question...")
        {
            var result = CurrentWindow is null
                ? MessageBox.Show(Text, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question)
                : MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.OK;
        }

        #endregion [Question dialog]

        #region [Message]

        /// <inheritdoc/>
        public virtual void Information(string Text, string Title = "Message...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <inheritdoc/>
        public virtual void Warning(string Text, string Title = "Message...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <inheritdoc/>
        public virtual void Error(string Text, string Title = "Message...")
        {
            if (CurrentWindow is null)
                MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(CurrentWindow, Text, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion [Message]
    }
}