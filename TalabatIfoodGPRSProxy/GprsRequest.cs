using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TalabatIfoodGPRSProxy
{
    /// <summary>
    /// Request Helper class
    /// </summary>
    internal class GprsRequest
    {
        HttpRequest _request;
        public GprsRequest(HttpRequest request)
        {
            _request = request;
        }


        int _terminalNo=-1;

        /// <summary>
        /// Terminal Id of the Device which made request
        /// </summary>
        public int TerminalId
        {
            get
            {
                if (_terminalNo < 0)
                {
                    if (!string.IsNullOrEmpty(_request.QueryString["a"]))
                    {
                        if (!int.TryParse(_request.QueryString["a"], NumberStyles.Integer, CultureInfo.CurrentCulture, out _terminalNo))
                        {
                            return -1;
                        }
                    }
                }

                return _terminalNo;
            }
        }


        /// <summary>
        /// The Url request is done
        /// </summary>
        public Uri Url
        {
            get { return _request.Url; } 
        }

        public override string ToString()
        {
            return String.Format("Terminal Id : {0} Url: {1}", TerminalId, Url.AbsoluteUri);
        }
    }
}
