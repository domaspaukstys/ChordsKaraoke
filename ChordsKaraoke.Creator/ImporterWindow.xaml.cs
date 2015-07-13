using System.Windows.Controls;
using System.Windows.Input;
using ChordsKaraoke.Data.Models;
using ChordsKaraoke.Data.ViewModels;
using ChordsKaraoke.Data.ViewModels.Commands;

namespace ChordsKaraoke.Creator
{
    /// <summary>
    /// Interaction logic for ImporterWindow.xaml
    /// </summary>
    public partial class ImporterWindow
    {
        public TimelineModel Model { get; private set; }
        public ImporterWindow()
        {
            InitializeComponent();
            ImportCommand = new DelegateCommand(o =>
            {
                ImporterViewModel model = DataContext as ImporterViewModel;
                if (model != null && model.Items.Count > 0)
                {

                    Model = model.Export();
                    DialogResult = true;
                    Close();
                }
            });
            ExitCommand = new DelegateCommand(o =>
            {
                DialogResult = false;
                Close();
            });
        }

        public ICommand ExitCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
            {
                double num;
                e.Handled = !double.TryParse(e.Text, out num);
            }
        }
    }
}
