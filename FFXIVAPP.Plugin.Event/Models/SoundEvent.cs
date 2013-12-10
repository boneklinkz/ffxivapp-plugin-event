// FFXIVAPP.Plugin.Event
// SoundEvent.cs
// 
// Copyright © 2013 ZAM Network LLC

namespace FFXIVAPP.Plugin.Event.Models
{
    public class SoundEvent
    {
        public string Sound { get; set; }
        public int Delay { get; set; }
        public string RegEx { get; set; }
        public string Category { get; set; }
        public bool Enabled { get; set; }
    }
}
