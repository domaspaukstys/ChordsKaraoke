using ChordsKaraoke.Data.Models;

namespace ChordsKaraoke.Data.Services
{
    public interface IModalService : IService
    {
        string OpenFile(string title = "Open File", string filter = "All Files|*.*");

        string SaveFile(string title = "Save File", string filter = "All Files|*.*",
            string defaultExtension = null);

        TimelineModel OpenImporter();
    }
}
