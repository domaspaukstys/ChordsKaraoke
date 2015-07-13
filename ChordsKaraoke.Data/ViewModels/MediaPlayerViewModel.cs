using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using ChordsKaraoke.Data.Services;
using ChordsKaraoke.Data.ViewModels.Commands;

namespace ChordsKaraoke.Data.ViewModels
{
    public class MediaPlayerViewModel : ViewModel
    {
        private readonly TimelineViewModel _parent;
        private readonly Timer _timer;
        private bool _isPaused;
        private bool _isPlaying;
        private DateTime _lastUpdateTime;
        private bool _mediaLoaded;

        public MediaPlayerViewModel(TimelineViewModel parent)
        {
            _timer = new Timer(25);
            _timer.Elapsed += UpdateCurrentTime;
            _parent = parent;
            _parent.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                switch (args.PropertyName)
                {
                    case "CurrentTime":
                    case "MaxTime":
                    case "MediaSource":
                        OnPropertyChanged(args.PropertyName);
                        break;
                }
            };

            PlayPauseCommand = new DelegateCommand(PlayPause);
            StopCommand = new DelegateCommand(Stop);
            LoadCommand = new DelegateCommand(Load);
        }

        public DelegateCommand PlayPauseCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }

        public bool IsPaused
        {
            get { return _isPaused; }
            set { SetField(ref _isPaused, value); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetField(ref _isPlaying, value); }
        }

        public double CurrentTime
        {
            get { return _parent.CurrentTime; }
            set { _parent.CurrentTime = value; }
        }

        public double MaxTime
        {
            get { return _parent.MaxTime; }
            set { _parent.MaxTime = value; }
        }

        public string MediaSource
        {
            get { return _parent.MediaSource; }
            set { _parent.MediaSource = value; }
        }

        public bool MediaLoaded
        {
            get { return _mediaLoaded; }
            set
            {
                _mediaLoaded = value;
                if (value)
                {
                    _timer.Stop();
                }
            }
        }

        public DelegateCommand LoadCommand { get; private set; }

        private void Load(object obj)
        {
            var service = GetService<IModalService>();
            //            Media
            string path = service.OpenFile("Open media file",
                "All Supported Audio | *.asx; *.wm; *.wma; *.wmx; *.wav; *.mp3; *.m3u; *.aac");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                MediaSource = path;
            }
        }

        private void UpdateCurrentTime(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            DateTime now = DateTime.Now;
            double diff = (now - _lastUpdateTime).TotalMilliseconds/1000;
            if (CurrentTime + diff > MaxTime)
            {
                Stop(null);
            }
            else
            {
                CurrentTime += diff;
            }
            _lastUpdateTime = now;
        }

        private void PlayPause(object obj)
        {
            if (MediaLoaded)
            {
                if (IsPlaying)
                {
                    IsPaused = !IsPaused;
                }
                else
                {
                    IsPlaying = true;
                }
            }
            else
            {
                if (!IsPlaying)
                {
                    IsPlaying = true;
                    IsPaused = true;
                }
                if (IsPaused)
                {
                    StartTimer();
                    IsPaused = false;
                }
                else
                {
                    _timer.Stop();
                    IsPaused = true;
                }
            }
        }

        private void StartTimer()
        {
            _lastUpdateTime = DateTime.Now;
            _timer.Start();
        }

        private void Stop(object obj)
        {
            if (MediaLoaded)
            {
                IsPlaying = false;
            }
            else
            {
                _timer.Stop();
                CurrentTime = 0;
                IsPlaying = false;
                IsPaused = true;
            }
        }
    }
}