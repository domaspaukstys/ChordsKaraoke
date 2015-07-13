
using ChordsKaraoke.Editor.Models;

namespace ChordsKaraoke.Editor.ViewModels
{
    public abstract class ViewModel
    {
    }

    public abstract class ViewModel<T> : ViewModel
        where T : Model
    {
        protected ViewModel(T model)
        {
            Model = model;
        }
        public T Model { get; protected set; }
    }
}
