﻿// FFXIVAPP.Plugin.Event
// Initializer.cs
// 
// Copyright © 2013 ZAM Network LLC
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
// following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
//    disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
//    following disclaimer in the documentation and/or other materials provided with the distribution. 
//  * Neither the name of ZAM Network LLC nor the names of its contributors may be used to endorse or promote products 
//    derived from this software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
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
            var legacyFiles = Directory.GetFiles(Constants.BaseDirectory)
                                       .Where(file => Regex.IsMatch(file, @"^.+\.(wav|mp3)$"))
                                       .Select(file => new FileInfo(file));
            foreach (var fileInfo in legacyFiles)
            {
                var fileName = Path.GetFileName(fileInfo.FullName);
                if (File.Exists(Path.Combine(Common.Constants.SoundsPath, fileName)))
                {
                    continue;
                }
                File.Copy(fileInfo.FullName, Path.Combine(Common.Constants.SoundsPath, fileName));
                SoundPlayerHelper.TryGetSetSoundFile(fileName);
            }
            foreach (var cachedSoundFile in SoundPlayerHelper.GetSoundFiles())
            {
                PluginViewModel.Instance.SoundFiles.Add(cachedSoundFile);
            }
            PluginViewModel.Instance.SoundFiles.Insert(0, " ");
        }

        public static void LoadLogEvents()
        {
            PluginViewModel.Instance.Events.Clear();
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Event"))
                {
                    var xId = Guid.Empty;
                    // migrate regex from key, if necessary
                    var xRegEx = xElement.Element("RegEx") != null ? (string) xElement.Element("RegEx") : (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
                    var xSound = (string) xElement.Element("Sound");
                    var xDelay = (string) xElement.Element("Delay");
                    var xCategory = (string) xElement.Element("Category");
                    var xExecutable = (string) xElement.Element("Executable");
                    var xEnabled = true;
                    try
                    {
                        xEnabled = (bool) xElement.Element("Enabled");
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        xId = (Guid) xElement.Element("Id");
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
