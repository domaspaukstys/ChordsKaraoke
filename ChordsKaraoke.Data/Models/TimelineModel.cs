using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ChordsKaraoke.Data.Models
{
    [Serializable]
    [DataContract]
    public class TimelineModel : Model
    {
        public TimelineModel()
        {
            Chords = new List<TimeTextModel>();
            Lyrics = new List<TimeTextModel>();
        }

        [DataMember]
        public List<TimeTextModel> Chords { get; set; }

        [DataMember]
        public List<TimeTextModel> Lyrics { get; set; }

        [DataMember]
        public string MediaSource { get; set; }

        [IgnoreDataMember]
        public double MaxTime
        {
            get
            {
                double max = 0;
                if (Chords.Count > 0)
                {
                    max = Math.Max(max, Chords.Max(x => x.Length + x.Time));
                }
                
                if (Lyrics.Count > 0)
                {
                    max = Math.Max(max, Lyrics.Max(x => x.Length + x.Time));
                }
                return max;

            }
        }
    }
}