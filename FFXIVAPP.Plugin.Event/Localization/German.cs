﻿// FFXIVAPP.Plugin.Event
// German.cs
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

using System.Windows;

namespace FFXIVAPP.Plugin.Event.Localization
{
    public abstract class German
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();
            Dictionary.Add("event_PLACEHOLDER", "*PH*");
            Dictionary.Add("event_AddUpdateEventButtonText", "Ereignis hinzufügen oder updaten");
            Dictionary.Add("event_RegExHeader", "RegEx");
            Dictionary.Add("event_RegExLabel", "RegEx:");
            Dictionary.Add("event_SampleText", "The scout vulture readies Wing Cutter.");
            Dictionary.Add("event_SoundHeader", "Klang");
            Dictionary.Add("event_SoundLabel", "Klang:");
            Dictionary.Add("event_DelayHeader", "Verzögerung (secs)");
            Dictionary.Add("event_DelayLabel", "Verzögerung (secs):");
            Dictionary.Add("event_EnabledHeader", "Enabled");
            Dictionary.Add("event_CategoryLabel", "Category:");
            Dictionary.Add("event_CategoryHeader", "Category");
            Dictionary.Add("event_MiscellaneousLabel", "Miscellaneous");
            Dictionary.Add("event_RefreshSoundListButtonText", "Refresh Sound List");
            Dictionary.Add("event_ExecutableLabel", "Run:");
            Dictionary.Add("event_SelectExecutableButtonText", "Select Executable");
            Dictionary.Add("event_ExecutableHeader", "Run");
            return Dictionary;
        }
    }
}
