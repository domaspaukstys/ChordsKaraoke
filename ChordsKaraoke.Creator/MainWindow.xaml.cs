using ChordsKaraoke.Creator.Services;
using ChordsKaraoke.Data.Services;
using ChordsKaraoke.Data.ViewModels;

namespace ChordsKaraoke.Creator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            ViewModel.AddService(typeof(IModalService), new ModalService());
            InitializeComponent();
        }
    }
}