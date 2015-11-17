using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TalabatIfoodGPRSProxy;

namespace WebTests
{
    public class Logger : ILogger
    {
        const string FILENAME = "D:\\Silinecek\\proxy.log";
        public void Debug(string message)
        {
            System.IO.File.AppendAllText(FILENAME, String.Format("{0}DEBUG - {1} - {2}",Environment.NewLine,DateTime.Now.ToString("g"), message));
        }

        public void Error(string message)
        {
            System.IO.File.AppendAllText(FILENAME, String.Format("{0}ERROR - {1} - {2}", Environment.NewLine, DateTime.Now.ToString("g"), message));
        }
    }
}