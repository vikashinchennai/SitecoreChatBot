using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amgen.Web.Feature.ChatBot.Models
{
    public class Response  
    {
        public string OptionDisplayText { get; set; }

        public bool Goback { get; set; }


        public string CallerAPI { get; set; }

        public List<ResponseData> NextQuestion {get;set;}

        public bool EndChat { get; set; }

        public string EndMessage { get; set; }

       // public IResponseData Ires { get; set; }
    }
    public class ResponseData
    {
        public string Data { get; set; }
        
      public string UserInput { get; set; }

      public string PlaceholderText { get; set; }

    public bool IsMandatory { get; set; }

       public  bool GoBack { get; set; }

         string callerApi { get; set; }

         string Regexpattern { get; set; }

         string Endmessage { get; set; }

         bool EndChat { get; set; }

    }

    public interface IResponse2
    {
         string OptionDisplayText { get; set; }

         bool Goback { get; set; }


         string CallerAPI { get; set; }

         List<IFlow> NextQuestion { get; set; }

         bool EndChat { get; set; }

         string EndMessage { get; set; }


    }
}