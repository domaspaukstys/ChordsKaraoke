namespace ChordsKaraoke.Data.ViewModels
{
    public class ImporterRowViewModel : ViewModel
    {
        private string _row;
        private bool _isChordsRow;
        public ImporterViewModel Parent { get; private set; }
        public ImporterRowViewModel(ImporterViewModel parent)
        {
            Parent = parent;
        }

        public string Row
        {
            get { return _row; }
            set { SetField(ref _row, value); }
        }

        public bool IsChordsRow
        {
            get { return _isChordsRow; }
            set { SetField(ref _isChordsRow, value); }
        }
    }
}
