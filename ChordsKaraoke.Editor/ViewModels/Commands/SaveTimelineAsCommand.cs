namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class SaveTimelineAsCommand : ViewModelCommand<TimelineViewModel>
    {
        public SaveTimelineAsCommand(TimelineViewModel model)
            : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.SaveFile(null);
        }
    }
}
