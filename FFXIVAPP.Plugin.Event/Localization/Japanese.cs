// FFXIVAPP.Plugin.Event
// Japanese.cs
// 
// Copyright © 2013 ZAM Network LLC

using System.Windows;

namespace FFXIVAPP.Plugin.Event.Localization
{
    public abstract class Japanese
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();
            Dictionary.Add("event_PLACEHOLDER", "*PH*");
            Dictionary.Add("event_AddUpdateEventButtonText", "イベントの追加/保存");
            Dictionary.Add("event_UpdateEventButtonText", "Update event");
            Dictionary.Add("event_AddEventButtonText", "Add event");
            Dictionary.Add("event_RegExHeader", "正規表現");
            Dictionary.Add("event_RegExLabel", "正規表現:");
            Dictionary.Add("event_SampleText", "スカウトヴァルチャーは「ウィングカッター」の構え。");
            Dictionary.Add("event_SoundHeader", "サウンド");
            Dictionary.Add("event_SoundLabel", "サウンド:");
            Dictionary.Add("event_DelayHeader", "遅延(秒)");
            Dictionary.Add("event_DelayLabel", "遅延(秒):");
            Dictionary.Add("event_EnabledHeader", "有効");
            Dictionary.Add("event_CategoryLabel", "カテゴリ:");
            Dictionary.Add("event_CategoryHeader", "カテゴリ");
            Dictionary.Add("event_MiscellaneousLabel", "一般");
            Dictionary.Add("event_RefreshSoundListButtonText", "サウンドリストを更新");
            Dictionary.Add("event_ExecutableLabel", "Run:");
            Dictionary.Add("event_SelectExecutableText", "Select File");
            Dictionary.Add("event_ExecutableHeader", "Run");
            return Dictionary;
        }
    }
}
