using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using ChordsKaraoke.Data.Models;

namespace ChordsKaraoke.Data.ViewModels
{
    public class ImporterViewModel : ViewModel
    {
        private string _text;
        private double _rowLength;

        public ImporterViewModel()
        {
            Items = new ObservableCollection<ImporterRowViewModel>();
            Items.CollectionChanged += (sender, args) => OnPropertyChanged("HasItem");
            RowLength = 5;
        }

        public ObservableCollection<ImporterRowViewModel> Items { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                if (SetField(ref _text, value))
                {
                    RegenerateCollection();
                }
            }
        }

        private void RegenerateCollection()
        {
            StringReader reader = new StringReader(_text);
            string line;
            int index = 0;
            bool isPreviousChords = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                ImporterRowViewModel row;
                if (index < Items.Count)
                {
                    row = Items[index];
                }
                else
                {
                    row = new ImporterRowViewModel(this)
                    {
                        IsChordsRow = !isPreviousChords
                    };
                    Items.Add(row);
                }
                row.Row = line;
                isPreviousChords = row.IsChordsRow;
                index++;
            }

            while (index + 1 < Items.Count)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }

        public TimelineModel Export()
        {
            TimelineModel model = new TimelineModel();

            List<ImporterRowViewModel> rows = new List<ImporterRowViewModel>(Items);
            List<Pair> pairs = new List<Pair>();
            Pair pair = new Pair();
            pairs.Add(pair);
            while (rows.Count > 0)
            {
                if (rows[0].IsChordsRow)
                {
                    if (pair.Lyrics.Count > 0)
                    {
                        pair = new Pair();
                        pairs.Add(pair);
                    }
                    pair.Chords.Add(rows[0].Row);
                }
                else
                {
                    pair.Lyrics.Add(rows[0].Row);
                }
                rows.RemoveAt(0);
            }

            double currentPosition = 0;
            TimeTextModel previousChord = null;
            Regex regex = new Regex(@"\S+");
            double lyricsStartTime = 0;
            foreach (Pair p in pairs)
            {

                for (int i = 0; i < Math.Max(p.Chords.Count, p.Lyrics.Count); i++)
                {
                    int count = 0;
                    string lyrics = null;
                    if (p.Lyrics.Count > i)
                    {
                        lyrics = p.Lyrics[i];
                        count = Math.Max(lyrics.Length, count);
                        if (p.Chords.Count <= i)
                        {
                            model.Lyrics.Add(new TimeTextModel { Length = RowLength, Text = lyrics, Time = currentPosition });
                        }
                    }
                    if (p.Chords.Count > i)
                    {
                        count = Math.Max(p.Chords[i].Length, count);
                        double coef = RowLength / count;
                        string chordRow = p.Chords[i];
                        MatchCollection collection = regex.Matches(chordRow);
                        int prevSplitPoint = 0;
                        foreach (Match match in collection)
                        {
                            double time = currentPosition + (match.Index * coef);
                            if (previousChord != null)
                            {
                                previousChord.Length = time - previousChord.Time;
                            }

                            previousChord = new TimeTextModel { Length = 0, Text = match.Value, Time = time };
                            if (!string.IsNullOrEmpty(lyrics))
                            {
                                int splitPoint = lyrics.Length >= (match.Index - prevSplitPoint) ? (match.Index - prevSplitPoint) : lyrics.Length - 1;
                                if (splitPoint > 0)
                                {
                                    model.Lyrics.Add(new TimeTextModel { Length = time - lyricsStartTime, Text = lyrics.Substring(0, splitPoint), Time = lyricsStartTime });
                                    lyricsStartTime = time;
                                    lyrics = lyrics.Substring(splitPoint);
                                    prevSplitPoint = splitPoint;
                                }
                            }
                            model.Chords.Add(previousChord);
                        }
                        if (!string.IsNullOrEmpty(lyrics))
                        {
                            model.Lyrics.Add(new TimeTextModel { Length = currentPosition + RowLength - lyricsStartTime, Time = lyricsStartTime, Text = lyrics });
                        }
                    }
                    currentPosition += RowLength;
                    lyricsStartTime = currentPosition;
                }

            }

            return model;
        }

        public double RowLength
        {
            get { return _rowLength; }
            set { SetField(ref _rowLength, value); }
        }

        public bool HasItem
        {
            get { return Items.Count > 0; }
        }

        private class Pair
        {
            internal Pair()
            {
                Chords = new List<string>();
                Lyrics = new List<string>();
            }
            internal List<string> Chords { get; set; }
            internal List<string> Lyrics { get; set; }
        }
    }
}
