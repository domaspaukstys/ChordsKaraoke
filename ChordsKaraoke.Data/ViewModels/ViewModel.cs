using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChordsKaraoke.Data.Annotations;
using ChordsKaraoke.Data.Models;
using ChordsKaraoke.Data.Services;

namespace ChordsKaraoke.Data.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();
        public event PropertyChangedEventHandler PropertyChanged;

        public static void AddService(Type interfaceType, IService service)
        {
//            Type type = service.GetType();
            if (!Services.ContainsKey(interfaceType))
            {
                Services.Add(interfaceType, service);
            }
        }

        public static T GetService<T>()
            where T : class, IService
        {
            Type type = typeof (T);
            IService service;
            Services.TryGetValue(type, out service);
            return service as T;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
// ReSharper disable once CompareNonConstrainedGenericWithNull
            bool result = field != null ? !field.Equals(value) : !Equals(field, value);
            if (result)
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
            return result;
        }
    }

    public abstract class ViewModel<T> : ViewModel
        where T : Model
    {
        protected ViewModel(T model)
        {
            Model = model;
        }

        public T Model { get; protected set; }

        public abstract void FromModel();
        public abstract void ToModel();
    }
}