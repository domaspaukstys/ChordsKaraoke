using System.Windows;
using ChordsKaraoke.Editor.Models;

namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public class ExitCommand : ViewModelCommand<MainViewModel>
    {
        public ExitCommand(MainViewModel model)
            : base(model)
        {
        }

        public override void Execute(object parameter)
        {
            TimelineModel timelineModel = ViewModel.TimelineModel.Model;
            if (timelineModel.Chords.Count > 0 ||
                timelineModel.Lyrics.Count > 0)
            {
                MessageBoxResult boxResult = MessageBox.Show("There is changes. Do you want to save your progress?", "Changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                switch (boxResult)
                {
                    case MessageBoxResult.Yes:
                        ViewModel.TimelineModel.SaveCommand.Execute(null);
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            Application.Current.Shutdown();
        }
    }
}
