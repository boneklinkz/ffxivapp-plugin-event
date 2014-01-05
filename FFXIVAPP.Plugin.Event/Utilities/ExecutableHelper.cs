using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.Diagnostics;
using System.IO;

namespace FFXIVAPP.Plugin.Event.Utilities
{
    public static class ExecutableHelper
    {
        public static void Run(string path)
        {
            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            try
            {
                DispatcherHelper.Invoke(() => Process.Start(path));
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "Executable failed.", ex);
            }
        }
    }
}
