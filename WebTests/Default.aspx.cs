using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTests
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TalabatIfoodGPRSProxy.TalabatIfoodGPRSProxy.Logger=new Logger();
            TalabatIfoodGPRSProxy.TalabatIfoodGPRSProxy.ExecuteProxy();



            Response.Write("Execution will continue if proxy doesnt want to halt");
        }
    }
}