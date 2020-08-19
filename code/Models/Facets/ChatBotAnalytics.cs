using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Web.Feature.ChatBot.Models.Facets
{
    [FacetKey(DefaultFacetKey)]
    [Serializable]
    public class ChatBotAnalytics : Facet
    {
        public const string DefaultFacetKey = "CustomerChatBot";

        public ChatBotAnalytics()
        {

        }
        public string ChatId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    [Serializable]
    public class ChatInfo
    {
        /// <summary>
        /// Every Chat will have unique Id, a website can have more than 1 Chat bot
        /// </summary>
        public string ChatId { get; set; }
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
        public DateTime LastModified { get; set; }
        
    }


}