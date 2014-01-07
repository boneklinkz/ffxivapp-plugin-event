// FFXIVAPP.Plugin.Event
// Initializer.cs
// 
// Copyright © 2013 ZAM Network LLC

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FFXIVAPP.Plugin.Event.Models;
using FFXIVAPP.Plugin.Event.ViewModels;

namespace FFXIVAPP.Plugin.Event
{
    internal static class Initializer
    {
        #region Declarations

        #endregion

        public static void LoadSettings()
        {
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Setting"))
                {
                    var xKey = (string)xElement.Attribute("Key");
                    var xValue = (string)xElement.Element("Value");
                    if (String.IsNullOrWhiteSpace(xKey) || String.IsNullOrWhiteSpace(xValue))
                    {
                        continue;
                    }
                    //SettingsHelper.Event.SetValue(xKey, xValue);
                    if (!Constants.Settings.Contains(xKey))
                    {
                        Constants.Settings.Add(xKey);
                    }
                }
            }
        }

        public static void LoadSounds()
        {
            PluginViewModel.Instance.SoundFiles.Clear();
            //do your gui stuff here
            var files = Directory.GetFiles(Constants.BaseDirectory)
                                 .Where(file => Regex.IsMatch(file, @"^.+\.(wav)$"))
                                 .Select(file => new FileInfo(file));
            foreach (var file in files)
            {
                PluginViewModel.Instance.SoundFiles.Add(file.Name);
            }

            PluginViewModel.Instance.SoundFiles.Insert(0, " ");
        }

        public static void LoadSoundEvents()
        {
            PluginViewModel.Instance.Events.Clear();
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants().Elements("Event"))
                {
                    var xId = Guid.Empty;
                    // migrate regex from key, if necessary
                    var xRegEx = xElement.Element("RegEx") != null ? (string)xElement.Element("RegEx") : (string)xElement.Attribute("Key");
                    var xValue = (string)xElement.Element("Value");
                    var xSound = (string)xElement.Element("Sound");
                    var xDelay = (string)xElement.Element("Delay");
                    var xCategory = (string)xElement.Element("Category");
                    var xExecutable = (string)xElement.Element("Executable");
                    var xEnabled = true;

                    try
                    {
                        xEnabled = (bool)xElement.Element("Enabled");
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        xId = (Guid)xElement.Element("Id");
                    }
                    catch (Exception)
                    {
                    }

                    if (String.IsNullOrWhiteSpace(xRegEx))
                    {
                        continue;
                    }

                    xId = xId != Guid.Empty ? xId : Guid.NewGuid();
                    xSound = String.IsNullOrWhiteSpace(xValue) ? xSound : xValue;
                    xCategory = String.IsNullOrWhiteSpace(xCategory) ? "Miscellaneous" : xCategory;

                    var logEvent = new LogEvent
                    {
                        Id = xId,
                        Sound = xSound,
                        Delay = 0,
                        RegEx = xRegEx,
                        Category = xCategory,
                        Enabled = xEnabled,
                        Executable = xExecutable
                    };

                    int result;
                    if (Int32.TryParse(xDelay, out result))
                    {
                        logEvent.Delay = result;
                    }

                    var found = PluginViewModel.Instance.Events.Any(@event => @event.Id == logEvent.Id);
                    if (!found)
                    {
                        PluginViewModel.Instance.Events.Add(logEvent);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public static void ApplyTheming()
        {
            MainViewModel.SetupGrouping();
        }
    }
}
