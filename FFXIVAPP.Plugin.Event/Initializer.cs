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
                    var xKey = (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
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
        }

        public static void LoadSoundEvents()
        {
            PluginViewModel.Instance.Events.Clear();
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Event"))
                {
                    var xRegEx = (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
                    var xSound = (string) xElement.Element("Sound");
                    var xDelay = (string) xElement.Element("Delay");
                    var xCategory = (string) xElement.Element("Category");
                    var xEnabled = true;
                    try
                    {
                        xEnabled = (bool) xElement.Element("Enabled");
                    }
                    catch (Exception ex)
                    {
                    }
                    if (String.IsNullOrWhiteSpace(xRegEx))
                    {
                        continue;
                    }
                    xSound = String.IsNullOrWhiteSpace(xSound) ? "aruba.wav" : String.IsNullOrWhiteSpace(xValue) ? xSound : xValue;
                    xCategory = String.IsNullOrWhiteSpace(xCategory) ? "Miscellaneous" : xCategory;
                    var soundEvent = new SoundEvent
                    {
                        Sound = xSound,
                        Delay = 0,
                        RegEx = xRegEx,
                        Category = xCategory,
                        Enabled = xEnabled
                    };
                    int result;
                    if (Int32.TryParse(xDelay, out result))
                    {
                        soundEvent.Delay = result;
                    }
                    var found = PluginViewModel.Instance.Events.Any(se => se.RegEx == soundEvent.RegEx);
                    if (!found)
                    {
                        PluginViewModel.Instance.Events.Add(soundEvent);
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
