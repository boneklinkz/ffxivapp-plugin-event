// FFXIVAPP.Plugin.Event
// MainViewModel.cs
// 
// Copyright © 2013 ZAM Network LLC

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.RegularExpressions;
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
        public ICommand DeleteCategoryCommand { get; private set; }
        public ICommand ToggleCategoryCommand { get; private set; }
        public ICommand SelectExecutableCommand { get; private set; }

        #endregion

        public MainViewModel()
        {
            RefreshSoundListCommand = new DelegateCommand(RefreshSoundList);
            AddEventCommand = new DelegateCommand(AddEvent);
            DeleteEventCommand = new DelegateCommand(DeleteEvent);
            EventSelectionCommand = new DelegateCommand(EventSelection);
            DeleteCategoryCommand = new DelegateCommand<string>(DeleteCategory);
            ToggleCategoryCommand = new DelegateCommand<string>(ToggleCategory);
            SelectExecutableCommand = new DelegateCommand(SelectExecutable);
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
            Guid? selectedId = null;
            try
            {
                if (MainView.View.Events.SelectedItems.Count == 1)
                {
                    selectedId = new Guid(GetValueBySelectedItem(MainView.View.Events, "Id"));
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }

            if (MainView.View.TDelay.Text.Trim() == "" || MainView.View.TRegEx.Text.Trim() == "")
            {
                return;
            }

            if (MainView.View.TCategory.Text.Trim() == "")
            {
                MainView.View.TCategory.Text = PluginViewModel.Instance.Locale["event_MiscellaneousLabel"];
            }

            if (Regex.IsMatch(MainView.View.TDelay.Text, @"[^0-9]+"))
            {
                var popupContent = new PopupContent
                {
                    PluginName = Plugin.PName,
                    Title = PluginViewModel.Instance.Locale["app_WarningMessage"],
                    Message = "Delay can only be numeric."
                };
                Plugin.PHost.PopupMessage(Plugin.PName, popupContent);
                return;
            }

            var logEvent = new LogEvent
            {
                Sound = MainView.View.TSound.Text,
                Delay = 0,
                RegEx = MainView.View.TRegEx.Text,
                Category = MainView.View.TCategory.Text,
                Enabled = true,
                Executable = MainView.View.TExecutable.Text
            };

            int result;
            if (Int32.TryParse(MainView.View.TDelay.Text, out result))
            {
                logEvent.Delay = result;
            }

            if (selectedId == null || selectedId == Guid.Empty)
            {
                logEvent.Id = Guid.NewGuid();
                PluginViewModel.Instance.Events.Add(logEvent);
            }
            else
            {
                var index = PluginViewModel.Instance.Events.TakeWhile(@event => @event.Id != selectedId).Count();
                PluginViewModel.Instance.Events[index] = logEvent;
            }

            MainView.View.Events.UnselectAll();
            MainView.View.TRegEx.Text = "";
            MainView.View.TExecutable.Text = "";
        }

        /// <summary>
        /// </summary>
        private static void DeleteEvent()
        {
            string selectedKey;
            try
            {
                selectedKey = GetValueBySelectedItem(MainView.View.Events, "Id");
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                return;
            }
            var index = PluginViewModel.Instance.Events.TakeWhile(@event => @event.Id.ToString() != selectedKey).Count();
            PluginViewModel.Instance.Events.RemoveAt(index);
        }

        /// <summary>
        /// </summary>
        private static void SelectExecutable()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                MainView.View.TExecutable.Text = dialog.FileName;
            }
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
            MainView.View.TExecutable.Text = GetValueBySelectedItem(MainView.View.Events, "Executable");
        }

        private static void DeleteCategory(string categoryName)
        {
            var categoryRegEx = new Regex(@"(?<category>.+) \(\d+\)", SharedRegEx.DefaultOptions);
            var matches = categoryRegEx.Match(categoryName);
            if (!matches.Success)
            {
                return;
            }
            var name = matches.Groups["category"].Value;
            var events = new List<LogEvent>(PluginViewModel.Instance.Events.ToList());
            foreach (var @event in events.Where(@event => @event.Category == name))
            {
                PluginViewModel.Instance.Events.Remove(@event);
            }
        }

        private static void ToggleCategory(string categoryName)
        {
            var categoryRegEx = new Regex(@"(?<category>.+) \(\d+\)", SharedRegEx.DefaultOptions);
            var matches = categoryRegEx.Match(categoryName);
            if (!matches.Success)
            {
                return;
            }
            MainView.View.Events.SelectedItem = null;
            var name = matches.Groups["category"].Value;
            var events = new List<LogEvent>(PluginViewModel.Instance.Events.ToList());
            var enabledCount = PluginViewModel.Instance.Events.Count(@event => @event.Enabled);
            var enable = enabledCount == 0 || (enabledCount < PluginViewModel.Instance.Events.Count);
            if (enable)
            {
                for (var i = 0; i < @events.Count; i++)
                {
                    if (@events[i].Category == name)
                    {
                        PluginViewModel.Instance.Events[i].Enabled = true;
                    }
                }
            }
            else
            {
                for (var i = 0; i < @events.Count; i++)
                {
                    if (@events[i].Category == name)
                    {
                        PluginViewModel.Instance.Events[i].Enabled = false;
                    }
                }
            }
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
