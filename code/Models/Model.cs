using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Sitecore.Web.Feature.ChatBot.Models
{
    public class ChatRequest: SharedFields
    {
        public string DisplayMessage { get; set; }

        public List<Option> AnswerOptions { get; set; }

        public string AnswerChoosed { get; set; }

        public bool IsOptionBased { get; set; }
        public string PlaceHolderText { get; set; }
        public string RegexPatternToValidate { get; set; }

        public List<ChatRequest> ExistingChat { get; set; }

        public Guid ChatId { get; set; }
    }


    public class Option: SharedFields
    {
        public string OptionDisplayText { get; set; }

    }

    public class SharedFields
    {
        public Guid Id { get; set; }
        public string QuestionDisplay { get; set; }
        public bool CanGoBack { get; set; }

    }

    public class EndChat
    {
        public bool EndOfChat { get; set; }

        public string EndMessage { get; set; }

        public Guid ChatId { get; set; }
    }
       
}