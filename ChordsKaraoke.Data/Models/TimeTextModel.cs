using System;
using System.Runtime.Serialization;

namespace ChordsKaraoke.Data.Models
{
    [Serializable]
    [DataContract]
    public class TimeTextModel : Model
    {
        public TimeTextModel()
        {
            Time = 0;
            Length = 10;
            Text = string.Empty;
        }

        [DataMember]
        public double Time { get; set; }

        [DataMember]
        public double Length { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}