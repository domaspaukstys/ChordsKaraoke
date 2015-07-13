using System.Windows.Input;
using ChordsKaraoke.Editor.ViewModels.Commands;

namespace ChordsKaraoke.Editor.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public ICommand ExitCommand { get; private set; }
        public MainViewModel()
        {
            TimelineModel = new TimelineViewModel();
            ExitCommand = new ExitCommand(this);
        }
        public TimelineViewModel TimelineModel { get; set; }
    }
}
