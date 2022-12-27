namespace MVVM.ViewModels.Base
{
    public class TitledViewModel : ViewModel
    {
        #region [Title]

        private string? _title;

        /// <summary> Title (Header) </summary>
        public string? Title { get => _title; set => Set(ref _title, value); }

        #endregion [Title]
    }
}