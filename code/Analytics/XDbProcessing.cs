using Sitecore.Web.Feature.ChatBot.Models;
using Sitecore.Web.Feature.ChatBot.Models.Facets;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Operations;
using Sitecore.XConnect.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sitecore.Web.Feature.ChatBot.Analytics
{
    public class XDbProcessing : IXDbProcessing
    {
        public void RecordAnalytics(string[] contactDetails, string channel = null, string goalId = null, string chatId = null, string questionId = null, string answerId = null)
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    Contact contact = null, existingContact = null;
                    //// Retrieve contact

                    existingContact = client.Get<Contact>(new IdentifiedContactReference(contactDetails[0], contactDetails[1]), new ContactExpandOptions());

                    if (existingContact != null)
                    {
                        contact = existingContact;
                    }
                    else
                    {
                        contact = new Contact(new ContactIdentifier(contactDetails[0], contactDetails[1], ContactIdentifierType.Known));
                        client.AddContact(contact);
                    }
                    if (contact != null)
                    {
                        var result = getInteractionFacets(contact.Id.GetValueOrDefault(), chatId, questionId, answerId);
                        if (!result && !String.IsNullOrEmpty(answerId))
                        {
                            
                            var channelId = Guid.Parse(channel);
                            string userAgent = HttpContext.Current.Request.UserAgent;

                            Interaction interaction = new Interaction(contact, InteractionInitiator.Brand, channelId, userAgent);

                            ChatBotAnalytics CBData = new ChatBotAnalytics()
                            {
                                ChatId = chatId,
                                Question = questionId,
                                Answer = answerId
                            };

                            client.SetFacet<ChatBotAnalytics>(interaction, ChatBotAnalytics.DefaultFacetKey, CBData);

                            var offlineGoal = Guid.Parse(goalId);
                            var xConnectEvent = new Goal(offlineGoal, DateTime.UtcNow);

                            interaction.Events.Add(xConnectEvent);
                            client.AddInteraction(interaction);

                            client.Submit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Info(ex.ToString(), this);
                }

            }

        }

        public bool getInteractionFacets(Guid contactId, string chatId, string questionId, string answerId)
        {
            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    //Contact reference from ID
                   var reference = new Sitecore.XConnect.ContactReference(contactId);
                    // Retrieve contact
                    var results = client.Get<Contact>(reference, new Sitecore.XConnect.ContactExpandOptions()
                    {
                        Interactions = new Sitecore.XConnect.RelatedInteractionsExpandOptions(ChatBotAnalytics.DefaultFacetKey)
                        {

                        }
                    });

                    var interactionFacet = results?.Interactions.Where(x => x.GetFacet<ChatBotAnalytics>()?.ChatId == chatId && x.GetFacet<ChatBotAnalytics>()?.Question == questionId && x.GetFacet<ChatBotAnalytics>()?.Answer == answerId);

                    if(interactionFacet != null && interactionFacet.Count() > 0)
                    {
                        return true;
                    }
                }
                catch (XdbExecutionException ex)
                {
                    // Manage exceptions
                }
            }

            return false;
        }

        public List<ChatInfo> GetLastStoredChat(string chatId = null, string contactId=null)
        {
            List<ChatInfo> chatInfo = new List<ChatInfo>();

            using (Sitecore.XConnect.Client.XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    //Contact reference from ID
                        var reference = new IdentifiedContactReference(Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource, contactId);
                        // Retrieve contact
                        var results = client.Get<Contact>(reference, new Sitecore.XConnect.ContactExpandOptions()
                        {
                            Interactions = new Sitecore.XConnect.RelatedInteractionsExpandOptions(ChatBotAnalytics.DefaultFacetKey)
                            {

                            }
                        });

                        if (chatId != null)
                        {
                            var interactionFacet = results?.Interactions.Where(x => x.GetFacet<ChatBotAnalytics>()?.ChatId == chatId);

                            if (interactionFacet != null && interactionFacet.Count() > 0)
                            {
                                foreach (var i in interactionFacet)
                                {
                                    ChatBotAnalytics ipInfoFacet = i.GetFacet<ChatBotAnalytics>(ChatBotAnalytics.DefaultFacetKey);

                                    chatInfo.Add(new ChatInfo
                                    {
                                        ChatId = ipInfoFacet.ChatId,
                                        QuestionId = ipInfoFacet.Question,
                                        AnswerId = ipInfoFacet.Answer,
                                        LastModified = ipInfoFacet.LastModified.GetValueOrDefault()
                                    });
                                }
                            }
                        }

                    chatInfo = chatInfo.GroupBy(x => x.QuestionId).Select(latest => latest.OrderByDescending(x => x.LastModified).First()).ToList();
                }
                catch (Exception ex)
                {
                    Log.Info(ex.ToString(), this);
                }
            }

            return chatInfo;
        }
    }

    public interface IXDbProcessing
    {
        void RecordAnalytics(string[] contactDetails, string channel, string goalId, string chatId, string questionId, string answerId);
        List<ChatInfo> GetLastStoredChat(string ChatId, string contactId);
    }
}