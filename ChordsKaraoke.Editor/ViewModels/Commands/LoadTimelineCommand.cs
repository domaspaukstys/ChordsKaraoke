namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class LoadTimelineCommand : ViewModelCommand<TimelineViewModel>
    {
        public LoadTimelineCommand(TimelineViewModel model) : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.LoadFile();
        }
    }
}
