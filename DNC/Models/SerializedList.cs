using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace DNC.Models
{

    public enum ListAction
    {
        Save = 0,
        Load = 1
    }

    public class SerializedListAction
    {
        public ListAction Action;

        public SerializedListAction(ListAction action)
        {
            Action = action;
        }
    }

    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public class SerializedList<T> : ObservableCollection<T>, IObservable<T>
    {
        public string FileName { get; set; } // no property changed on this

        public SerializedList(string fileName = "SerializedList.bin")
        {
            FileName = fileName;
            LoadList();
        }

        public void LoadList()
        {
            try
            {
                Debug.WriteLine("Loading Serialized List...");
                using FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                IFormatter formatter = new BinaryFormatter();
                IList<T> list = formatter.Deserialize(fs) as IList<T> ?? new List<T>();

                foreach (T t in list)
                    Add(t);

                Debug.WriteLine("Load Complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Load Failed {ex}");
            }
        }

        public void SaveList()
        {
            Debug.WriteLine("Saving Serialized List...");
            using FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, this);
            Debug.WriteLine("Save Complete");
        }

        public void DoSerializedListAction(SerializedListAction action)
        {
            switch (action.Action)
            {
                case ListAction.Load:
                    LoadList();
                    break;
                
                case ListAction.Save:
                    SaveList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            SaveList();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            SaveList();
        }


        #region IObservable

        private readonly IList<IObserver<T>> _observers = new List<IObserver<T>>();
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscribe(_observers, observer);
        }

        internal class Unsubscribe : IDisposable
        {
            private readonly IList<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            internal Unsubscribe(IList<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            #region IDisposable

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
            #endregion

        }

        #endregion

        
    }
}
