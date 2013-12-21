﻿// FFXIVAPP.Plugin.Event
// MainViewModel.cs
// 
// Copyright © 2013 ZAM Network LLC

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Common.ViewModelBase;
using FFXIVAPP.Plugin.Event.Models;
using FFXIVAPP.Plugin.Event.Views;
using NLog;

namespace FFXIVAPP.Plugin.Event.ViewModels
{
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static MainViewModel _instance;

        public static MainViewModel Instance
        {
            get { return _instance ?? (_instance = new MainViewModel()); }
        }

        #endregion

        #region Declarations

        public ICommand RefreshSoundListCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand DeleteEventCommand { get; private set; }
        public ICommand EventSelectionCommand { get; private set; }

        #endregion

        public MainViewModel()
        {
            RefreshSoundListCommand = new DelegateCommand(RefreshSoundList);
            AddEventCommand = new DelegateCommand(AddEvent);
            DeleteEventCommand = new DelegateCommand(DeleteEvent);
            EventSelectionCommand = new DelegateCommand(EventSelection);
        }

        public static void SetupGrouping()
        {
            var cvEvents = CollectionViewSource.GetDefaultView(MainView.View.Events.ItemsSource);
            if (cvEvents != null && cvEvents.CanGroup == true)
            {
                cvEvents.GroupDescriptions.Clear();
                cvEvents.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            }
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        /// <summary>
        /// </summary>
        /// <param name="listView"> </param>
        /// <param name="key"> </param>
        private static string GetValueBySelectedItem(Selector listView, string key)
        {
            var type = listView.SelectedItem.GetType();
            var property = type.GetProperty(key);
            return property.GetValue(listView.SelectedItem, null)
                           .ToString();
        }

        #endregion

        #region Command Bindings

        /// <summary>
        /// </summary>
        private static void RefreshSoundList()
        {
            Initializer.LoadSounds();
            SetupGrouping();
        }

        /// <summary>
        /// </summary>
        private static void AddEvent()
        {
            var selectedKey = "";
            try
            {
                if (MainView.View.Events.SelectedItems.Count == 1)
                {
                    selectedKey = GetValueBySelectedItem(MainView.View.Events, "RegEx");
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
            if (MainView.View.TSound.Text.Trim() == "" || MainView.View.TDelay.Text.Trim() == "" || MainView.View.TRegEx.Text.Trim() == "")
            {
                return;
            }
            if (MainView.View.TCategory.Text.Trim() == "")
            {
                MainView.View.TCategory.Text = PluginViewModel.Instance.Locale["event_MiscellaneousLabel"];
            }
            if (Regex.IsMatch(MainView.View.TDelay.Text, @"[^0-9]+"))
            {
                var popupContent = new PopupContent();
                popupContent.PluginName = Plugin.PName;
                popupContent.Title = PluginViewModel.Instance.Locale["app_WarningMessage"];
                popupContent.Message = "Delay can only be numeric.";
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
                return;
            }
            var soundEvent = new SoundEvent
            {
                Sound = MainView.View.TSound.Text,
                Delay = 0,
                RegEx = MainView.View.TRegEx.Text,
                Category = MainView.View.TCategory.Text,
                Enabled = true
            };
            int result;
            if (Int32.TryParse(MainView.View.TDelay.Text, out result))
            {
                soundEvent.Delay = result;
            }
            if (String.IsNullOrWhiteSpace(selectedKey))
            {
                var found = PluginViewModel.Instance.Events.Any(se => se.RegEx == soundEvent.RegEx);
                if (!found)
                {
                    PluginViewModel.Instance.Events.Add(soundEvent);
                }
            }
            else
            {
                var index = PluginViewModel.Instance.Events.TakeWhile(se => se.RegEx != selectedKey)
                                           .Count();
                PluginViewModel.Instance.Events[index] = soundEvent;
            }
            MainView.View.Events.UnselectAll();
            MainView.View.TRegEx.Text = "";
        }

        /// <summary>
        /// </summary>
        private static void DeleteEvent()
        {
            string selectedKey;
            try
            {
                selectedKey = GetValueBySelectedItem(MainView.View.Events, "RegEx");
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                return;
            }
            var index = PluginViewModel.Instance.Events.TakeWhile(se => se.RegEx != selectedKey)
                                       .Count();
            PluginViewModel.Instance.Events.RemoveAt(index);
        }

        /// <summary>
        /// </summary>
        private static void EventSelection()
        {
            if (MainView.View.Events.SelectedItems.Count != 1)
            {
                return;
            }
            if (MainView.View.Events.SelectedIndex < 0)
            {
                return;
            }
            MainView.View.TSound.Text = GetValueBySelectedItem(MainView.View.Events, "Sound");
            MainView.View.TDelay.Text = GetValueBySelectedItem(MainView.View.Events, "Delay");
            MainView.View.TRegEx.Text = GetValueBySelectedItem(MainView.View.Events, "RegEx");
            MainView.View.TCategory.Text = GetValueBySelectedItem(MainView.View.Events, "Category");
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
