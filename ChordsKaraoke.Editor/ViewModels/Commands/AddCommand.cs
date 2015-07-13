using ChordsKaraoke.Editor.Models;

namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class AddCommand : ViewModelCommand<TimelineViewModel>
    {
        public AddCommand(TimelineViewModel model) : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.Add();
        }
    }
}
