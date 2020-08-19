using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amgen.Web.Feature.ChatBot.Models
{
    public class Class1
    {
        public string DisplayMessage { get; set; }
        public string Question { get; set; }
        public List<IOption> AnswerOption { get; set; }

        
    }

    public class Class2
    {
        public string DisplayMessage { get; set; }
        public string Question { get; set; }
        public List<string> AnswerOption { get; set; }


    }

}