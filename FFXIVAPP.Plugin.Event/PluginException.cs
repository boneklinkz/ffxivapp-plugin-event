// FFXIVAPP.Plugin.Event
// PluginException.cs
// 
// © 2013 ZAM Network LLC

#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace FFXIVAPP.Plugin.Event
{
    [Serializable]
    public class PluginException : Exception
    {
        public PluginException()
        {
        }

        public PluginException(string message) : base(message)
        {
        }

        public PluginException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PluginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
