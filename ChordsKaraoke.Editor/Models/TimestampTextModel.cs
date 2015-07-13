using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChordsKaraoke.Editor.Models
{
    [Serializable]
    [DataContract]
    public class TimestampTextModel : Model
    {
        private uint _timestamp;
        private uint _length;
        private string _text;

        public TimestampTextModel()
            : this(0, 15)
        {
        }

        public TimestampTextModel(uint timestamp)
            : this(timestamp, 15)
        {
        }
        public TimestampTextModel(uint timestamp, uint lenght)
        {
            _timestamp = timestamp;
            _length = lenght;
        }

        [DataMember]
        public uint Timestamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }

        [DataMember]
        public uint Length
        {
            get { return _length; }
            set { SetProperty(ref _length, value); }
        }

        [DataMember]
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public class TimestampTextModelComparer : IComparer<TimestampTextModel>
        {
            public int Compare(TimestampTextModel x, TimestampTextModel y)
            {
                return (x.Timestamp + x.Length).CompareTo(y.Timestamp + y.Length);
            }
        }
    }
}
