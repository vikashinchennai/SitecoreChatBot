using Sitecore.Web.Feature.ChatBot.Models.Facets;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Web.Feature.ChatBot.Models
{
    public static class XDBChatBotModel
    {
        public static XdbModel Model { get; } = XDBChatBotModel.BuilddCustomModel();
        private static XdbModel BuilddCustomModel()
        {
            XdbModelBuilder xdbModelBuilder = new XdbModelBuilder("ChatBotInteractionFacetModel", new XdbModelVersion(1, 0));
            xdbModelBuilder.ReferenceModel(CollectionModel.Model);
            xdbModelBuilder.DefineFacet<Interaction, ChatBotAnalytics>(ChatBotAnalytics.DefaultFacetKey);
            return xdbModelBuilder.BuildModel();
        }
    }
}