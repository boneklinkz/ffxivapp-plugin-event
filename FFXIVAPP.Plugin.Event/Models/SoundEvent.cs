// FFXIVAPP.Plugin.Event
// SoundEvent.cs
// 
// Copyright © 2013 ZAM Network LLC

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FFXIVAPP.Plugin.Event.Models
{
    public class SoundEvent : INotifyPropertyChanged
    {
        private string _sound;
        private int _delay;
        private string _regEx;
        private string _category;
        private bool _enabled;

        public string Sound
        {
            get { return _sound; }
            set
            {
                _sound = value;
                RaisePropertyChanged();
            }
        }

        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                RaisePropertyChanged();
            }
        }

        public string RegEx
        {
            get { return _regEx; }
            set
            {
                _regEx = value;
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged();
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChanged();
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
