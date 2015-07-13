namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class SaveTimelineCommand : ViewModelCommand<TimelineViewModel>
    {
        public SaveTimelineCommand(TimelineViewModel model) : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.SaveFile(ViewModel.Model.Path);
        }
    }
}
