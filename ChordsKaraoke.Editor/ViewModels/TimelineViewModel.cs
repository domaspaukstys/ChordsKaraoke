using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Input;
using ChordsKaraoke.Editor.Models;
using ChordsKaraoke.Editor.ViewModels.Commands;
using ChordsKaraoke.Editor.Views;
using Microsoft.Win32;

namespace ChordsKaraoke.Editor.ViewModels
{
    public class TimelineViewModel : ViewModel<TimelineModel>
    {
        public ICommand LoadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        public TimelineViewModel()
            : base(new TimelineModel())
        {
            NewCommand = new NewTimelineCommand(this);
            LoadCommand = new LoadTimelineCommand(this);
            SaveCommand = new SaveTimelineCommand(this);
            SaveAsCommand = new SaveTimelineAsCommand(this);
            AddCommand = new AddCommand(this);
        }


        public void LoadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    using (FileStream stream = File.OpenRead(path))
                    {
                        DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(TimelineModel));
                        var model = (TimelineModel)serialiser.ReadObject(stream);
                        Model.Chords.AddRange(model.Chords);
                        Model.Lyrics.AddRange(model.Lyrics);
                        Model.Path = path;
                        stream.Close();
                    }
                }
            }
        }

        public void SaveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    path = dialog.FileName;
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (FileStream stream = File.OpenWrite(path))
                {
                    DataContractJsonSerializer serialiser = new DataContractJsonSerializer(typeof(TimelineModel));
                    Model.Path = string.Empty;
                    serialiser.WriteObject(stream, Model);
                    Model.Path = path;
                    stream.Close();
                }
            }
        }

        public void Add()
        {
            Model.Chords.Add(new TimestampTextModel{Text = "BBD"});
        }
    }
}
