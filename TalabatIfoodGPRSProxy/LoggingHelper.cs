using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalabatIfoodGPRSProxy
{
    /// <summary>
    /// Simplifies Logging methods
    /// </summary>
    internal static class LoggingHelper
    {
        public static void Debug(ILogger logger, string message)
        {
            if (logger != null)
                logger.Debug(message);
        }
        public static void Error(ILogger logger, string message)
        {
            if (logger != null)
                logger.Error(message);
        }
    }
}
