using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ChordsKaraoke.Editor.Annotations;

namespace ChordsKaraoke.Editor.Models
{
    [Serializable]
    [DataContract]
    public abstract class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T holder, T value, [CallerMemberName] string propertyName = null)
        {
            bool result = holder == null ? !Equals(holder, value) : !holder.Equals(value);
            if (result)
            {
                holder = value;
                OnPropertyChanged(propertyName);
            }
            return result;
        }
    }
}
