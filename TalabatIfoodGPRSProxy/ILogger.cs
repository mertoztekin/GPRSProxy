using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalabatIfoodGPRSProxy
{
    public interface ILogger
    {
        void Debug(string message);

        void Error(string message);
    }
}
