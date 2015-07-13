using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using ChordsKaraoke.Data.Models;
using ChordsKaraoke.Data.Services;
using ChordsKaraoke.Data.ViewModels.Commands;

namespace ChordsKaraoke.Data.ViewModels
{
    public class TimelineViewModel : ViewModel<TimelineModel>
    {
        private const double PixelsInSecond = 5;
        private double _currentTime;
        private string _fileName;
        private double _maxTime;
        private string _mediaSource;
        private double _zoom;

        public TimelineViewModel()
            : base(new TimelineModel())
        {
            _mediaSource = string.Empty;
            Chords = new ObservableCollection<TimeTextViewModel>();
            Chords.CollectionChanged += CollectionChanged;
            Lyrics = new ObservableCollection<TimeTextViewModel>();
            Lyrics.CollectionChanged += CollectionChanged;
            Time = new ObservableCollection<TimeTextViewModel>();
            SelectedItems = new List<TimeTextViewModel>();
            NewCommand = new DelegateCommand(o => New());
            LoadCommand = new DelegateCommand(o => LoadFile());
            SaveCommand = new DelegateCommand(o => SaveFile(FileName));
            SaveAsCommand = new DelegateCommand(o => SaveFile(null));
            AddChordCommand = new DelegateCommand(o => AddChord());
            AddTextCommand = new DelegateCommand(o => AddText());
            CopyCommand = new DelegateCommand(o => Copy(), o => SelectedItems.Count > 0);
            RemoveItemsCommand = new DelegateCommand(o => RemoveItems(), o => SelectedItems.Count > 0);
            ZoomInCommand = new DelegateCommand(o =>
            {
                if (Zoom < 20)
                {
                    Zoom += 0.5;
                }
            });
            ZoomOutCommand = new DelegateCommand(o =>
            {
                if (Zoom > 0)
                {
                    Zoom -= 0.5;
                }
            });
            Zoom = 1.0d;
        }

        public DelegateCommand NewCommand { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand AddChordCommand { get; private set; }
        public DelegateCommand AddTextCommand { get; private set; }
        public DelegateCommand RemoveItemsCommand { get; private set; }
        public DelegateCommand ZoomInCommand { get; private set; }
        public DelegateCommand ZoomOutCommand { get; private set; }

        public double CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (SetField(ref _currentTime, value))
                {
                    OnPropertyChanged("CurrentPosition");
                    OnPropertyChanged("CurrentText");
                    //                    ExpandTimeline();
                }
            }
        }

        public double CurrentPosition
        {
            get { return TimeToPixels(CurrentTime); }
            set { CurrentTime = PixelsToTime(value); }
        }

        public double MaxX
        {
            get { return TimeToPixels(MaxTime); }
            set { MaxTime = PixelsToTime(value); }
        }

        public double MaxTime
        {
            get { return _maxTime; }
            set
            {
                if (_maxTime < value)
                {
                    if (SetField(ref _maxTime, value))
                    {
                        OnPropertyChanged("MaxX");
                        ExpandTimeline();
                    }
                }
            }
        }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set { SetField(ref _multiSelect, value); }
        }

        public string FileName
        {
            get { return _fileName; }
            set { SetField(ref _fileName, value); }
        }

        public ObservableCollection<TimeTextViewModel> Chords { get; private set; }
        public ObservableCollection<TimeTextViewModel> Lyrics { get; private set; }
        public ObservableCollection<TimeTextViewModel> Time { get; private set; }

        public TimeTextViewModel ActiveItem
        {
            get
            {
                return SelectedItems.LastOrDefault();
            }
        }

        public bool HasSelectedItem { get { return SelectedItems.Count > 0; } }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value > 0 && value < 20)
                    if (SetField(ref _zoom, value))
                    {
                        OnPropertyChanged("MaxX");
                    }

            }
        }

        public string CurrentText
        {
            get
            {
                Func<TimeTextViewModel, bool> currentItems =
                    x => CurrentTime >= x.Time && CurrentTime < x.Time + x.Length;
                List<TimeTextViewModel> current = Chords.Where(currentItems).ToList();
                string results = string.Empty;
                double nextTime = CurrentTime;
                if (current.Any())
                {
                    results += current.Select(x => x.Text).Aggregate(
                        (x, y) => string.Format("{0} | {1}", x, y));

                    nextTime = current.Max(x => x.Time + x.Length);
                }
                TimeTextViewModel next = Chords.Where(x => x.Time >= nextTime).Min(x => x);
                if (next != null)
                {
                    results += string.Format("\t{0} ({1})", next.Text, Math.Round(next.Time - CurrentTime, 1));
                }
                results += "\n";
                current = Lyrics.Where(currentItems).ToList();
                nextTime = CurrentTime;
                if (current.Any())
                {
                    results +=
                        Lyrics.Where(currentItems)
                            .Select(x => x.Text)
                            .Aggregate((x, y) => string.Format("{0}\n{1}", x, y));
                    nextTime = current.Max(x => x.Time + x.Length);
                }
                next = Lyrics.Where(x => x.Time >= nextTime).Min(x => x);
                if (next != null)
                {
                    results += string.Format("\t{0} ({1})", next.Text, Math.Round(next.Time - CurrentTime, 1));
                }
                return results;
            }
        }

        public string MediaSource
        {
            get { return _mediaSource; }
            set { SetField(ref _mediaSource, value); }
        }

        private void New()
        {
            Model = new TimelineModel();
            FromModel();
        }

        public void ExpandTimeline()
        {
            double max = Math.Max(MaxX, CurrentPosition);
            double curMax = 0;
            if (Time.Count > 0)
            {
                curMax = Time.Max(x => x.X + x.Width);
            }
            while (curMax < max)
            {
                curMax += AddItem(Time, false).Width;
            }
            if (MaxX < max)
            {
                MaxX = max;
            }
        }

        public double PixelsToTime(double pixel)
        {
            return pixel / PixelsInSecond / Zoom;
        }

        public double TimeToPixels(double time)
        {
            return time * PixelsInSecond * Zoom;
        }


        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            RecalculateMaxX();
        }

        public void Import(TimelineModel model)
        {
            Model = model;
            FromModel();
        }

        private void RecalculateMaxX()
        {
            double max = 0;
            if (Chords.Count > 0)
                max = Math.Max(max, Chords.Max(x => x.Width + x.X));
            if (Lyrics.Count > 0)
                max = Math.Max(max, Lyrics.Max(x => x.Width + x.X));

            MaxX = max;
        }

        private void RemoveItems()
        {
            foreach (TimeTextViewModel item in SelectedItems)
            {
                if (Chords.Contains(item))
                {
                    Chords.Remove(item);
                }
                if (Lyrics.Contains(item))
                {
                    Lyrics.Remove(item);
                }
            }
            SelectedItems.Clear();
            NotifySelectedChanged();
        }

        private void NotifySelectedChanged()
        {
            OnPropertyChanged("ActiveItem");
            OnPropertyChanged("HasSelectedItem");
            CopyCommand.RaiseCanExecuteChanged();
            RemoveItemsCommand.RaiseCanExecuteChanged();
        }

        private void AddChord()
        {
            TimeTextViewModel i = AddItem(Chords, true);
            i.IsSelected = true;
        }

        private void AddText()
        {
            TimeTextViewModel i = AddItem(Lyrics, true);
            i.IsSelected = true;
        }

        private TimeTextViewModel AddItem(ObservableCollection<TimeTextViewModel> collection, bool attach)
        {
            double max = 0;

            if (collection.Count > 0)
            {
                max = collection.Max(x => x.Time + x.Length);
            }
            var newItem = new TimeTextViewModel(this, new TimeTextModel { Time = max }, attach);
            collection.Add(newItem);
            return newItem;
        }

        public void CollectionItemChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "X":
                case "Width":
                    RecalculateMaxX();
                    ChangeSelected(sender as TimeTextViewModel);
                    break;
                case "IsSelected":
                    ChooseSelected(sender as TimeTextViewModel);
                    break;
            }
        }

        private bool _moving;
        private void ChangeSelected(TimeTextViewModel viewModel)
        {
            if (!_moving && SelectedItems.Count > 1)
            {
                _moving = true;
                double deltaTime = viewModel.Time - viewModel.Model.Time;
                double deltaLength = viewModel.Length - viewModel.Model.Length;
                foreach (TimeTextViewModel chord in SelectedItems.Where(x => !x.Equals(viewModel)))
                {
                    chord.Time += deltaTime;
                    chord.Length += deltaLength;
                    chord.ToModel();
                }
                viewModel.ToModel();
                _moving = false;
            }
        }

        private bool _selectedChanging;
        private bool _multiSelect;

        private void ChooseSelected(TimeTextViewModel viewModel)
        {
            if (viewModel != null && !_selectedChanging)
            {
                _selectedChanging = true;
                bool select = viewModel.IsSelected || (SelectedItems.Count > 1 && !MultiSelect);
                if (!MultiSelect)
                {
                    SelectedItems.ForEach(x => x.IsSelected = false);
                    SelectedItems.Clear();
                }
                if (select)
                {
                    viewModel.IsSelected = true;
                    SelectedItems.Add(viewModel);
                }
                OnPropertyChanged("ActiveItem");
                OnPropertyChanged("HasSelectedItem");
                _selectedChanging = false;
                NotifySelectedChanged();
            }
        }

        public List<TimeTextViewModel> SelectedItems { get; private set; }

        public void LoadFile(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                var service = GetService<IModalService>();
                path = service.OpenFile("Open chords file", "Chord Files (*.clf)|*.clf");
            }
            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    var serialiser = new DataContractJsonSerializer(typeof(TimelineModel));
                    Model = (TimelineModel)serialiser.ReadObject(stream);
                    FromModel();
                    stream.Close();
                    FileName = path;
                }
            }
        }

        public void SaveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var service = GetService<IModalService>();
                path = service.SaveFile("Save File As...", "Chord Files (*.clf)|*.clf", "clf");
            }

            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (FileStream stream = File.OpenWrite(path))
                {
                    var serialiser = new DataContractJsonSerializer(typeof(TimelineModel));
                    ToModel();
                    serialiser.WriteObject(stream, Model);
                    stream.Close();
                    FileName = path;
                }
            }
        }

        private void Copy()
        {
            if (SelectedItems.Count > 0)
            {
                List<TimeTextViewModel> copied = new List<TimeTextViewModel>();
                double max = 0;
                if (Chords.Count > 0)
                {
                    max = Math.Max(max, Chords.Max(x => x.Time + x.Length));
                }
                if (Lyrics.Count > 0)
                {
                    max = Math.Max(max, Lyrics.Max(x => x.Time + x.Length));
                }
                double diff = max - SelectedItems.Min(x => x.Time);

                foreach (TimeTextViewModel item in SelectedItems)
                {
                    TimeTextModel model = new TimeTextModel
                    {
                        Length = item.Length,
                        Text = item.Text,
                        Time = item.Time + diff
                    };
                    TimeTextViewModel newItem = new TimeTextViewModel(this, model, true);
                    if (Chords.Contains(item))
                    {
                        Chords.Add(newItem);
                    }
                    else if (Lyrics.Contains(item))
                    {
                        Lyrics.Add(newItem);
                    }
                    copied.Add(newItem);
                }
                MultiSelect = true;
                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    SelectedItems[i].IsSelected = false;
                }
                SelectedItems.Clear();
                copied.ForEach(x => x.IsSelected = true);
                MultiSelect = false;
            }
        }

        public override void FromModel()
        {
            SelectedItems.Clear();
            Chords.Clear();
            Lyrics.Clear();

            Model.Chords.Select(x =>
            {
                var model = new TimeTextViewModel(this, x, true);
                return model;
            }).ToList().ForEach(x => Chords.Add(x));

            Model.Lyrics.Select(x =>
            {
                var model = new TimeTextViewModel(this, x, true);
                return model;
            }).ToList().ForEach(x => Lyrics.Add(x));
            MediaSource = Model.MediaSource;
            OnPropertyChanged();
            NotifySelectedChanged();
        }

        public override void ToModel()
        {
            Model.Chords.Clear();
            Model.Lyrics.Clear();
            Model.Chords.AddRange(Chords.Select(x =>
            {
                x.ToModel();
                return x.Model;
            }));
            Model.Lyrics.AddRange(Lyrics.Select(x =>
            {
                x.ToModel();
                return x.Model;
            }));
            Model.MediaSource = MediaSource;
        }
    }
}