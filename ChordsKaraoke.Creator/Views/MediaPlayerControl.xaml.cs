using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChordsKaraoke.Creator.Views
{
    /// <summary>
    ///     Interaction logic for MediaPlayerControl.xaml
    /// </summary>
    public partial class MediaPlayerControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof (string), typeof (MediaPlayerControl),
            new PropertyMetadata(string.Empty, SourcePropertyChangedCallback));

        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register(
            "CurrentTime", typeof (double), typeof (MediaPlayerControl),
            new PropertyMetadata(default(double), CurrentTimePropertyChangedCallback));

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration", typeof (double), typeof (MediaPlayerControl), new PropertyMetadata(default(double)));


        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
            "IsPlaying", typeof (bool), typeof (MediaPlayerControl),
            new PropertyMetadata(false, IsPlayingPropertyChangedCallback));

        public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register(
            "IsPaused", typeof (bool), typeof (MediaPlayerControl),
            new PropertyMetadata(true, IsPausedPropertyChangedCallback));

        public static readonly DependencyProperty IsMediaLoadedProperty = DependencyProperty.Register(
            "IsMediaLoaded", typeof (bool), typeof (MediaPlayerControl), new PropertyMetadata(false));

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof (double),
            typeof (MediaPlayerControl), new PropertyMetadata(0.5d));

        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register("IsMuted", typeof (bool),
            typeof (MediaPlayerControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty BalanceProperty = DependencyProperty.Register("Balance",
            typeof (double), typeof (MediaPlayerControl), new PropertyMetadata(0.0d));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch",
            typeof (Stretch), typeof (MediaPlayerControl), new PropertyMetadata(Stretch.Uniform));

        public static readonly DependencyProperty StretchDirectionProperty =
            DependencyProperty.Register("StretchDirection", typeof (StretchDirection), typeof (MediaPlayerControl),
                new PropertyMetadata(StretchDirection.Both));

        private readonly MediaTimeline _timeline;
        private bool _autoChange;
        private MediaClock _clock;
        private ClockController _controller;
        private bool _timeChanging;

        public MediaPlayerControl()
        {
            InitializeComponent();
            _timeline = new MediaTimeline();
            _timeline.CurrentTimeInvalidated += CurrentTimeChanged;
            _timeline.Completed += MediaEnded;
            _timeline.CurrentStateInvalidated += ClockStateChanged;
        }

        public double Duration
        {
            get { return (double) GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }


        public bool IsMediaLoaded
        {
            get { return (bool) GetValue(IsMediaLoadedProperty); }
            set { SetValue(IsMediaLoadedProperty, value); }
        }

        public bool IsPlaying
        {
            get { return (bool) GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        public bool IsPaused
        {
            get { return (bool) GetValue(IsPausedProperty); }
            set { SetValue(IsPausedProperty, value); }
        }

        public double CurrentTime
        {
            get { return (double) GetValue(CurrentTimeProperty); }
            set
            {
                if (!_timeChanging)
                    SetValue(CurrentTimeProperty, value);
            }
        }

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public double Volume
        {
            get { return (double) GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public bool IsMuted
        {
            get { return (bool) GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }

        public double Balance
        {
            get { return (double) GetValue(BalanceProperty); }
            set { SetValue(BalanceProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch) GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public StretchDirection StretchDirection
        {
            get { return (StretchDirection) GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        private static void SourcePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var control = dependencyObject as MediaPlayerControl;
            if (control != null && args.NewValue != null)
            {
                control.OpenMedia(new Uri((string) args.NewValue));
            }
        }

        private static void CurrentTimePropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var control = dependencyObject as MediaPlayerControl;
            if (control != null && args.NewValue != args.OldValue && !control._autoChange)
            {
                control.ChangeCurrentTime((double) args.NewValue);
            }
        }

        private void ChangeCurrentTime(double time)
        {
            _timeChanging = true;
            if (_controller != null)
            {
                bool isPaused = IsPaused || !IsPlaying;
                if (!IsPlaying)
                {
                    IsPlaying = true;
                }
                _controller.Seek(new TimeSpan(0, 0, 0, (int) time), TimeSeekOrigin.BeginTime);
                IsPaused = isPaused;
            }
            _timeChanging = false;
        }

        private static void IsPlayingPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var control = dependencyObject as MediaPlayerControl;
            if (control != null && args.NewValue != args.OldValue)
            {
                control.PlayStop((bool) args.NewValue);
            }
        }

        private static void IsPausedPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var control = dependencyObject as MediaPlayerControl;
            if (control != null && args.NewValue != args.OldValue)
            {
                control.PlayPause(!(bool) args.NewValue);
            }
        }

        private void OpenMedia(Uri source)
        {
            Reset();
            _timeline.Source = source;
            MediaPlayerElement.Clock = _timeline.CreateClock();
        }

        private void ClockStateChanged(object sender, EventArgs eventArgs)
        {
            IsPlaying = _clock != null && _clock.CurrentState == ClockState.Active;
        }

        private void MediaEnded(object sender, EventArgs eventArgs)
        {
            IsPlaying = false;
        }

        private void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            Duration = 0;
            CurrentTime = 0;
            _clock = null;
            _controller = null;
            IsMediaLoaded = false;
            IsPlaying = false;
            IsPaused = false;
            _timeline.Source = null;
        }

        private void PlayStop(bool play)
        {
            if (_controller != null)
            {
                if (play)
                {
                    _controller.Begin();
                    IsPaused = false;
                }
                else
                {
                    _controller.Stop();
                }
            }
        }

        private void PlayPause(bool play)
        {
            if (_controller != null)
            {
                if (IsPlaying)
                {
                    if (play)
                    {
                        _controller.Resume();
                    }
                    else
                    {
                        _controller.Pause();
                    }
                }
            }
        }

        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaPlayerElement.Clock != null &&
                MediaPlayerElement.Clock.Controller != null)
            {
                _clock = MediaPlayerElement.Clock;
                _controller = MediaPlayerElement.Clock.Controller;

                IsMediaLoaded = true;
                _controller.Stop();
                if (_clock.NaturalDuration.HasTimeSpan)
                {
                    Duration = _clock.NaturalDuration.TimeSpan.TotalMilliseconds/1000;
                }
            }
            else
            {
                Reset();
            }
        }

        private void CurrentTimeChanged(object sender, EventArgs eventArgs)
        {
            if (sender is MediaClock && sender == _clock)
            {
                _autoChange = true;
                CurrentTime = _clock.CurrentTime.HasValue ? _clock.CurrentTime.Value.TotalSeconds : 0;
                _autoChange = false;
            }
        }
    }
}