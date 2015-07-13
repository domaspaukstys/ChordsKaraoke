using ChordsKaraoke.Data.Models;
using ChordsKaraoke.Data.Services;
using Microsoft.Win32;

namespace ChordsKaraoke.Creator.Services
{
    public class ModalService : IModalService
    {
        public string OpenFile(string title = "Open File", string filter = "All Files|*.*")
        {
            string result = string.Empty;
            var dialog = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                Filter = filter,
                Title = title
            };
            if (dialog.ShowDialog() == true)
            {
                result = dialog.FileName;
            }
            return result;
        }

        public string SaveFile(string title = "Save File", string filter = "All Files|*.*",
            string defaultExtension = null)
        {
            string result = string.Empty;
            var dialog = new SaveFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = false,
                Filter = filter,
                Title = title,
                AddExtension = true
            };
            if (!string.IsNullOrEmpty(defaultExtension))
            {
                dialog.DefaultExt = defaultExtension;
            }
            if (dialog.ShowDialog() == true)
            {
                result = dialog.FileName;
            }
            return result;
        }

        public TimelineModel OpenImporter()
        {
            TimelineModel model = null;
            ImporterWindow window = new ImporterWindow();
            if (window.ShowDialog() == true)
            {
                model = window.Model;
            }
            return model;
        }
    }
}