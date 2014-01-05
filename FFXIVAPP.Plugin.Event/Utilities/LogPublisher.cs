// FFXIVAPP.Plugin.Event
// LogPublisher.cs
// 
// Copyright © 2013 ZAM Network LLC

using System;
using System.Text.RegularExpressions;
using System.Timers;
using FFXIVAPP.Common.Core.Memory;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.RegularExpressions;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Plugin.Event.Models;
using NLog;

namespace FFXIVAPP.Plugin.Event.Utilities
{
    public static class LogPublisher
    {
        public static void Process(ChatLogEntry chatLogEntry)
        {
            try
            {
                var line = chatLogEntry.Line.Replace("  ", " ");
                foreach (var item in PluginViewModel.Instance.Events)
                {
                    if (!item.Enabled)
                    {
                        continue;
                    }
                    var resuccess = false;
                    var check = new Regex(item.RegEx);
                    if (SharedRegEx.IsValidRegex(item.RegEx))
                    {
                        var reg = check.Match(line);
                        if (reg.Success)
                        {
                            resuccess = true;
                        }
                    }
                    else
                    {
                        resuccess = (item.RegEx == line);
                    }
                    if (!resuccess)
                    {
                        continue;
                    }

                    PlaySound(item);
                    RunExecutable(item);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        private static void PlaySound(LogEvent logEvent)
        {
            if (String.IsNullOrWhiteSpace(logEvent.Sound)) return;

            var delay = logEvent.Delay;

            Func<bool> playSound = () =>
            {
                var timer = new Timer(delay > 0 ? delay * 1000 : 1);
                ElapsedEventHandler timerEventHandler = null;
                timerEventHandler = delegate
                {
                    DispatcherHelper.Invoke(
                        () => SoundPlayerHelper.Play(Constants.BaseDirectory, logEvent.Sound));
                    timer.Elapsed -= timerEventHandler;
                };
                timer.Elapsed += timerEventHandler;
                timer.Start();
                return true;
            };
            playSound.BeginInvoke(null, null);
        }

        private static void RunExecutable(LogEvent logEvent)
        {
            if (String.IsNullOrWhiteSpace(logEvent.Executable)) return;

            var delay = logEvent.Delay;

            Func<bool> runExecutable = () =>
            {
                var timer = new Timer(delay > 0 ? delay * 1000 : 1);
                ElapsedEventHandler timerEventHandler = null;
                timerEventHandler = delegate
                {
                    ExecutableHelper.Run(logEvent.Executable);
                    timer.Elapsed -= timerEventHandler;
                };
                timer.Elapsed += timerEventHandler;
                timer.Start();
                return true;
            };
            runExecutable.BeginInvoke(null, null);
        }
    }
}
