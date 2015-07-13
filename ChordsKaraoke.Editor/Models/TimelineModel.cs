using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace ChordsKaraoke.Editor.Models
{
    [DataContract]
    public class TimelineModel : Model
    {
        private string _path;
        private double _zoom;

        [IgnoreDataMember]
        public string Path
        {
            get { return _path; }
            set
            {
                if (SetProperty(ref _path, value))
                    OnPropertyChanged("FileName");
            }
        }

        [IgnoreDataMember]
        public double Zoom
        {
            get { return _zoom; }
            set { SetProperty(ref _zoom, value); }
        }

        public string FileName
        {
            get { return (_path != null ? System.IO.Path.GetFileName(_path) : ""); }
        }

        public TimelineModel()
        {
            TimestampTextModel.TimestampTextModelComparer comparer = new TimestampTextModel.TimestampTextModelComparer();
            Chords = new ObservableSortedList<TimestampTextModel>(comparer);
            Chords.CollectionChanged += TimelineItemsChanged;
            Lyrics = new ObservableSortedList<TimestampTextModel>(comparer);
            Lyrics.CollectionChanged += TimelineItemsChanged;
            _zoom = 2.0d;
        }

        private void TimelineItemsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnPropertyChanged("Length");
        }

        [DataMember]
        public ObservableSortedList<TimestampTextModel> Chords { get; set; }
        [DataMember]
        public ObservableSortedList<TimestampTextModel> Lyrics { get; set; }

        public uint Length
        {
            get
            {
                uint length = 0;
                TimestampTextModel last = Chords.LastOrDefault();
                if (last != null)
                {
                    length = last.Timestamp + last.Length;
                }
                last = Lyrics.LastOrDefault();
                if (last != null && last.Timestamp + last.Length > length)
                {
                    length = last.Timestamp + last.Length;
                }
                return length;
            }
        }
    }
}
