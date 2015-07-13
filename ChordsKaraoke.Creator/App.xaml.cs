using System.Linq;
using System.Threading;
using System.Windows;

namespace ChordsKaraoke.Creator
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Any())
            {
                Properties["ArbitraryArgName"] = new System.Uri(e.Args[0]);
            }
            base.OnStartup(e);
        }
    }
}