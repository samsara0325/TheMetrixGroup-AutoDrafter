using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins.Esign_Automation
{
   /**
    * @Author - Wilfred
    * This class models the JSON response that is received when the token is refreshed
    * */
   class AccessTokenModel
   {
      public string access_token { get; set; }
      public string token_type { get; set; }
      public int expires_in { get; set; }
   }
}
