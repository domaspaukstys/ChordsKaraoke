using System;
using System.Windows;
using ChordsKaraoke.Data.Models;
using ChordsKaraoke.Data.Services;
using ChordsKaraoke.Data.ViewModels.Commands;

namespace ChordsKaraoke.Data.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private MediaPlayerViewModel _mediaPlayer;
        private TimelineViewModel _timeline;

        public MainViewModel()
        {
            Timeline = new TimelineViewModel();
            if (Application.Current.Properties.Contains("ArbitraryArgName"))
            {
                Timeline.LoadFile(((Uri)Application.Current.Properties["ArbitraryArgName"]).AbsolutePath);
            }

            MediaPlayer = new MediaPlayerViewModel(Timeline);
            ExitCommand = new DelegateCommand(o => Application.Current.Shutdown());
            ImportCommand = new DelegateCommand(o =>
            {
                IModalService service = GetService<IModalService>();
                TimelineModel model = service.OpenImporter();
                if (model != null)
                {
                    Timeline.Import(model);
                }
            });
        }

        public TimelineViewModel Timeline
        {
            get { return _timeline; }
            private set { SetField(ref _timeline, value); }
        }

        public MediaPlayerViewModel MediaPlayer
        {
            get { return _mediaPlayer; }
            private set { SetField(ref _mediaPlayer, value); }
        }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand ImportCommand { get; private set; }
    }
}