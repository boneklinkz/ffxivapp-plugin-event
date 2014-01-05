// FFXIVAPP.Plugin.Event
// Settings.cs
// 
// Copyright © 2013 ZAM Network LLC

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using NLog;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;

namespace FFXIVAPP.Plugin.Event.Properties
{
    public class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private static Settings _default;

        public static Settings Default
        {
            get { return _default ?? (_default = ((Settings) (Synchronized(new Settings())))); }
        }

        public override void Save()
        {
            // this call to default settings only ensures we keep the settings we want and delete the ones we don't (old)
            DefaultSettings();
            SaveSettingsNode();
            SaveEventsNode();
            Constants.XSettings.Save(Constants.BaseDirectory + "Settings.xml");
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();
        }

        public new void Reset()
        {
            DefaultSettings();
            foreach (var key in Constants.Settings)
            {
                var settingsProperty = Default.Properties[key];
                if (settingsProperty == null)
                {
                    continue;
                }
                var value = settingsProperty.DefaultValue.ToString();
                SetValue(key, value);
            }
        }

        public static void SetValue(string key, string value)
        {
            try
            {
                var type = Default[key].GetType()
                                       .Name;
                switch (type)
                {
                    case "Boolean":
                        Default[key] = Convert.ToBoolean(value);
                        break;
                    case "Color":
                        var cc = new ColorConverter();
                        var color = cc.ConvertFrom(value);
                        Default[key] = color ?? Colors.Black;
                        break;
                    case "Double":
                        Default[key] = Convert.ToDouble(value);
                        break;
                    case "Font":
                        var fc = new FontConverter();
                        var font = fc.ConvertFromString(value);
                        Default[key] = font ?? new Font(new FontFamily("Microsoft Sans Serif"), 12);
                        break;
                    default:
                        Default[key] = value;
                        break;
                }
            }
            catch (SettingsPropertyNotFoundException ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
            catch (SettingsPropertyWrongTypeException ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        #region Property Bindings (Settings)

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF000000")]
        public Color ChatBackgroundColor
        {
            get { return ((Color) (this["ChatBackgroundColor"])); }
            set
            {
                this["ChatBackgroundColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF800080")]
        public Color TimeStampColor
        {
            get { return ((Color) (this["TimeStampColor"])); }
            set
            {
                this["TimeStampColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Microsoft Sans Serif, 12pt")]
        public Font ChatFont
        {
            get { return ((Font) (this["ChatFont"])); }
            set
            {
                this["ChatFont"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public Double Zoom
        {
            get { return ((Double) (this["Zoom"])); }
            set
            {
                this["Zoom"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Iterative Settings Saving

        private void SaveSettingsNode()
        {
            if (Constants.XSettings == null)
            {
                return;
            }
            var xElements = Constants.XSettings.Descendants()
                                     .Elements("Setting");
            var enumerable = xElements as XElement[] ?? xElements.ToArray();
            foreach (var setting in Constants.Settings)
            {
                var element = enumerable.FirstOrDefault(e => e.Attribute("Key")
                                                              .Value == setting);
                if (element == null)
                {
                    var xKey = setting;
                    var xValue = Default[xKey].ToString();
                    var keyPairList = new List<XValuePair>
                    {
                        new XValuePair
                        {
                            Key = "Value",
                            Value = xValue
                        }
                    };
                    XmlHelper.SaveXmlNode(Constants.XSettings, "Settings", "Setting", xKey, keyPairList);
                }
                else
                {
                    var xElement = element.Element("Value");
                    if (xElement != null)
                    {
                        xElement.Value = Default[setting].ToString();
                    }
                }
            }
        }

        private void SaveEventsNode()
        {
            if (Constants.XSettings == null)
            {
                return;
            }

            Constants.XSettings.Descendants("Event")
                     .Where(node => PluginViewModel.Instance.Events.All(e => e.Id.ToString() != node.Attribute("Key").Value))
                     .Remove();

            var xElements = Constants.XSettings.Descendants().Elements("Event");
            var enumerable = xElements as XElement[] ?? xElements.ToArray();

            foreach (var item in PluginViewModel.Instance.Events)
            {
                var xId = item.Id != Guid.Empty ? item.Id : Guid.NewGuid();
                var xRegEx = item.RegEx;
                var xSound = item.Sound;
                var xDelay = item.Delay;
                var xCategory = item.Category;
                var xEnabled = item.Enabled;
                var xExecutable = item.Executable;
                var keyPairList = new List<XValuePair>
                {
                    new XValuePair
                    {
                        Key = "RegEx",
                        Value = xRegEx
                    },
                    new XValuePair
                    {
                        Key = "Sound",
                        Value = xSound
                    },
                    new XValuePair
                    {
                        Key = "Delay",
                        Value = xDelay.ToString(CultureInfo.InvariantCulture)
                    },
                    new XValuePair
                    {
                        Key = "Category",
                        Value = xCategory
                    },
                    new XValuePair
                    {
                        Key = "Enabled",
                        Value = xEnabled.ToString()
                    },
                    new XValuePair
                    {
                        Key = "Executable",
                        Value = xExecutable
                    }
                };

                var element = enumerable.FirstOrDefault(e => e.Attribute("Key").Value == xId.ToString());
                if (element == null)
                {
                    XmlHelper.SaveXmlNode(Constants.XSettings, "Settings", "Event", xId.ToString(), keyPairList);
                }
                else
                {
                    var xIdElement = element.Element("Id");
                    if (xIdElement != null)
                    {
                        xIdElement.Value = xId.ToString();
                    }
                    var xRegExElement = element.Element("RegEx");
                    if (xRegExElement != null)
                    {
                        xRegExElement.Value = xRegEx;
                    }
                    var xSoundElement = element.Element("Sound");
                    if (xSoundElement != null)
                    {
                        xSoundElement.Value = !String.IsNullOrWhiteSpace(xSound) ? xSound : String.Empty;
                    }
                    var xDelayElement = element.Element("Delay");
                    if (xDelayElement != null)
                    {
                        xDelayElement.Value = xDelay.ToString(CultureInfo.InvariantCulture);
                    }
                    var xCategoryElement = element.Element("Category");
                    if (xCategoryElement != null)
                    {
                        xCategoryElement.Value = xCategory;
                    }
                    var xEnabledElement = element.Element("Enabled");
                    if (xEnabledElement != null)
                    {
                        xEnabledElement.Value = xEnabled.ToString();
                    }
                    var xExecutableElement = element.Element("Executable");
                    if (xExecutableElement != null)
                    {
                        xExecutableElement.Value = !String.IsNullOrWhiteSpace(xExecutable) ? xExecutable : String.Empty;
                    }
                }
            }
        }

        #endregion
    }
}
