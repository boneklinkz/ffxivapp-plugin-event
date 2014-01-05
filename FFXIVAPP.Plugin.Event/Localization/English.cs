// FFXIVAPP.Plugin.Event
// English.cs
// 
// Copyright © 2013 ZAM Network LLC

using System.Windows;

namespace FFXIVAPP.Plugin.Event.Localization
{
    public abstract class English
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();
            Dictionary.Add("event_PLACEHOLDER", "*PH*");
            Dictionary.Add("event_AddUpdateEventButtonText", "Add Or Update Event");
            Dictionary.Add("event_RegExHeader", "RegEx");
            Dictionary.Add("event_RegExLabel", "RegEx:");
            Dictionary.Add("event_SampleText", "The scout vulture readies Wing Cutter.");
            Dictionary.Add("event_SoundHeader", "Sound");
            Dictionary.Add("event_SoundLabel", "Sound:");
            Dictionary.Add("event_DelayHeader", "Delay (secs)");
            Dictionary.Add("event_DelayLabel", "Delay (secs):");
            Dictionary.Add("event_EnabledHeader", "Enabled");
            Dictionary.Add("event_CategoryLabel", "Category:");
            Dictionary.Add("event_CategoryHeader", "Category");
            Dictionary.Add("event_MiscellaneousLabel", "Miscellaneous");
            Dictionary.Add("event_RefreshSoundListButtonText", "Refresh Sound List");
            Dictionary.Add("event_ExecutableLabel", "Run:");
            Dictionary.Add("event_SelectExecutableText", "Select File");
            Dictionary.Add("event_ExecutableHeader", "Run");
            return Dictionary;
        }
    }
}
