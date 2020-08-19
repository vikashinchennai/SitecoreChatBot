using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amgen.Web.Feature.ChatBot.Models
{
    public class FlowName
    {
      
        
           public string UserInputField { get; set; }

           public string OptionDisplaytext { get; set; }
           public  string PlaceholderText { get; set; }

          
          public  bool IsMandatory { get; set; }

           public List<IFlow> NextQuestion { get; set; }
           public string RegexPattern { get; set; }

        public bool EndChat { get; set; }

        public string EndMessage { get; set; }

        
    }
}