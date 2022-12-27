using System.IO;

namespace MVVM.Services.Interfaces
{
    public interface IUserDialog
    {
        #region [Open/Save]

        /// <summary>Open file dialog to read</summary>
        /// <param name="Title">Title</param>
        /// <param name="Filter">Extension filter</param>
        /// <param name="DefaultFilePath">Default path</param>
        /// <returns>Selected file or null if dialog was cancelled</returns>
        FileInfo? OpenFile(string Title, string Filter = "All files (*.*)|*.*", string? DefaultFilePath = null);

        /// <summary>Open file dialog to write</summary>
        /// <param name="Title">Title</param>
        /// <param name="Filter">Extension filter</param>
        /// <param name="DefaultFilePath">Default path</param>
        /// <returns>Selected file or null if dialog was cancelled</returns>
        FileInfo? SaveFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null);

        #endregion [Open/Save]

        #region [Question dialogs]

        /// <summary>Dialog with message and Yes/No buttons</summary>
        /// <param name="Text">Message of dialog</param>
        /// <param name="Title">Title</param>
        /// <returns><see cref="true"/> if Yes button pressed</returns>
        bool YesNoQuestion(string Text, string Title = "Question...");

        /// <summary>Dialog with message and Ok/Cancel buttons</summary>
        /// <param name="Text">Message of dialog</param>
        /// <param name="Title">Title</param>
        /// <returns><see cref="true"/> if Ok button pressed</returns>
        bool OkCancelQuestion(string Text, string Title = "Question...");

        #endregion [Question dialogs]

        #region [Message]

        /// <summary>Information dialog</summary>
        /// <param name="Text">Dialog message</param>
        /// <param name="Title">Title</param>
        void Information(string Text, string Title = "Message...");

        /// <summary>Warning dialog</summary>
        /// <param name="Text">Dialog message</param>
        /// <param name="Title">Title</param>
        void Warning(string Text, string Title = "Message...");

        /// <summary>Error dialog</summary>
        /// <param name="Text">Dialog message</param>
        /// <param name="Title">Title</param>
        void Error(string Text, string Title = "Message...");

        #endregion [Message]
    }
}