using System;
using System.Collections.Generic;
using System.ComponentModel;
using ChordsKaraoke.Data.Models;

namespace ChordsKaraoke.Data.ViewModels
{
    public class TimeTextViewModel : ViewModel<TimeTextModel>, IComparable<TimeTextViewModel>
    {
        private bool _isSelected;
        private double _length;
        private string _text;
        private double _time;
        private bool _attach;

        public TimeTextViewModel(TimelineViewModel parent)
            : this(parent, new TimeTextModel(), false)
        {
        }

        public TimeTextViewModel(TimelineViewModel parent, TimeTextModel model, bool attach)
            : base(model)
        {
            _attach = attach;
            Parent = parent;
            Parent.PropertyChanged += ParentOnPropertyChanged;
            if (attach)
            {
                PropertyChanged += Parent.CollectionItemChanged;
            }
            FromModel();
        }

        public TimelineViewModel Parent { get; private set; }

        public double Width
        {
            get { return Parent.TimeToPixels(Length); }
            set { Length = Parent.PixelsToTime(value); }
        }

        public double X
        {
            get { return Parent.TimeToPixels(Time); }
            set { Time = Parent.PixelsToTime(value); }
        }

        public double Time
        {
            get { return _time; }
            set
            {
                if (SetField(ref _time, value))
                {
                    OnPropertyChanged("X");
                }
            }
        }

        public double Length
        {
            get { return _length; }
            set
            {
                if (SetField(ref _length, value))
                {
                    OnPropertyChanged("Width");
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set { SetField(ref _text, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetField(ref _isSelected, value); }
        }

        private void ParentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Zoom")
            {
                OnPropertyChanged("Width");
                OnPropertyChanged("X");
            }
        }

        public override sealed void FromModel()
        {
            Text = Model.Text;
            Time = Model.Time;
            Length = Model.Length;
        }

        public override void ToModel()
        {
            Model.Text = Text;
            Model.Time = Time;
            Model.Length = Length;
        }

        public class TimeTextViewModelComparer : IComparer<TimeTextViewModel>
        {
            public int Compare(TimeTextViewModel x, TimeTextViewModel y)
            {
                return x.Time.CompareTo(y.Time);
            }
        }

        public int CompareTo(TimeTextViewModel other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}