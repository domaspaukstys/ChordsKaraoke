namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class NewTimelineCommand : ViewModelCommand<TimelineViewModel>
    {
        public NewTimelineCommand(TimelineViewModel model) : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.Model.Path = null;
            ViewModel.Model.Lyrics.Clear();
            ViewModel.Model.Chords.Clear();
        }
    }
}
