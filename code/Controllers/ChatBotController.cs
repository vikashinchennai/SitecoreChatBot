using Sitecore.Web.Feature.ChatBot.Analytics;
using Sitecore.Web.Feature.ChatBot.Models;
using Sitecore.Web.Feature.ChatBot.Models.Facets;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Sitecore.Web.Feature.ChatBot.Controllers
{
    public class ChatBotController : Controller
    {

        public ChatBotController()
        {
        }

        private Guid ChatItemId { get; set; }
        [System.Web.Http.HttpGet]
        public JsonResult GetData(string chatId = null, string questionId = null, string answerId = null, string value = null, string goBack = null)
        {
            Guid answerItemId = Guid.Empty;
            ChatItemId = GetChat(chatId);
            Guid questionItemId = Guid.Empty;
            if (ChatItemId == Guid.Empty)
            {
                var chatItemData = getItem(ChatItemId);
                return BuildResponseJson(new EndChat()
                {
                    EndOfChat = true,
                    EndMessage = FieldValue(chatItemData, "Injection Error")
                });
            }
            //To Go to Previous Question
            if (!string.IsNullOrEmpty(goBack) && goBack == "1")
            {
                //From Analytics get the last stored records and get the Question Id and then do get item and return here
                var id = GetPreviousQuestionFromAnalytics(ChatItemId);

                //If the Analytics returns proper Id we Return previous Question
                if (id.Guid != Guid.Empty)
                    return BuildChartModel(getItem(ChatItemId));
            }

            //1st Time chat - Answer not provided
            if ((!IsValidGuid(questionId, ref questionItemId) && string.IsNullOrEmpty(value)) && !IsValidGuid(answerId, ref answerItemId))
            {
                XDbProcessing ob = new XDbProcessing();
                var identifier = Tracker.Current?.Contact.ContactId.ToString("N");
                var chatList = ob.GetLastStoredChat(ChatItemId.ToString(), identifier);

                List<ChatRequest> chatRequestList = new List<ChatRequest>();

                if (chatList != null && chatList.Any())
                {
                    foreach (var list in chatList)
                    {
                        var itemCB = getItem(new Guid(list.QuestionId));
                        var modelCB = BuildBaseRequest(itemCB);
                        BuildRequestOptions(itemCB, modelCB);

                        Guid _AnswerItemId = Guid.Empty;
                        if (!IsValidGuid(list.AnswerId.ToString(), ref _AnswerItemId))
                        {
                            modelCB.AnswerChoosed = list.AnswerId.ToString();
                        }
                        else
                        {
                            var answer = getItem(_AnswerItemId);
                            modelCB.AnswerChoosed = answer.Fields["Option Display Text"].Value;
                        }

                        chatRequestList.Add(modelCB);
                    }
                }

                return BuildChartModel(getItem(ChatItemId), chatRequestList);
            }

            if (IsValidGuid(questionId, ref questionItemId))
            {
                IsValidGuid(answerId, ref answerItemId);
                //If User Submited manual Answer, then question id and value will have content
                if (!string.IsNullOrEmpty(value))
                {
                    InsertAnalyticsData(chatId, questionItemId, value);

                    var questionItem = getItem(questionItemId);

                    return ReturnNextQuestionOrEndChat(questionItem);
                }
                else if (answerItemId != Guid.Empty)
                {
                    //2nd Time Onwards
                    var item = getItem(answerItemId);
                    //to Make Sure the Answer is of Sent Question Id
                    if (item != null && item.ParentID.Guid == questionItemId)
                    {
                        InsertAnalyticsData(chatId, questionItemId, answerItemId);
                        
                        return ReturnNextQuestionOrEndChat(item);
                    }
                    else //Fake Request
                    {
                        return BuildEndOfChatJsonFromItem(item);
                    }
                }

                //If the request Doesnt have proper Answer, we then return same question to UI
                return BuildChartModel(getItem(questionItemId));
            }

            var chatItem = getItem(ChatItemId);

            var model = new EndChat()
            {
                EndOfChat = true,
                EndMessage = FieldValue(chatItem, "Injection Error")
            };

            return BuildResponseJson(model);
        }

        private JsonResult ReturnNextQuestionOrEndChat(Item questionItem)
        {
            if (GetBoolvalue(questionItem, "End of Chat"))
                return BuildEndOfChatJsonFromItem(questionItem);

            return BuildNextQuestionResponse(questionItem);
        }


        private JsonResult BuildNextQuestionResponse(Item item)
        {
            var nextQuestion = GetNextItem(item);
            //Chat has Next Question to Process & also not end of chat
            if (nextQuestion != null)
                return BuildChartModel(nextQuestion);

            Sitecore.Diagnostics.Log.Error("ChatBot: Item {0} has no Next Question Setup", this);
            var chatItem = getItem(ChatItemId);

            var model = new EndChat()
            {
                ChatId = ChatItemId,
                EndOfChat = true,
                EndMessage = FieldValue(chatItem, "Incorrect Flow")
            };

            return BuildResponseJson(model);
        }
        private JsonResult BuildEndOfChatJsonFromItem(Item item)
        {
            var model = new EndChat()
            {
                EndOfChat = GetBoolvalue(item, "End of Chat"),
                EndMessage = FieldValue(item, "End Message")
            };

            return BuildResponseJson(model);
        }

        private void InsertAnalyticsData(string chatId, Guid questionItemId, object valueToLog)
        {
            var source = Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource;
            var identifier = Tracker.Current?.Contact.ContactId.ToString("N");
            var AnalyticsData = getItem(ChatItemId);
            var channelId = AnalyticsData.Fields["ChannelId"]?.Value;
            var goalId = AnalyticsData.Fields["Goal Id"]?.Value;

            string[] userContact = { source, identifier };

            var XDbProcessing = new XDbProcessing();
            XDbProcessing.RecordAnalytics(userContact, channelId, goalId, chatId, questionItemId.ToString("D"), valueToLog.ToString());

        }

        private static Sitecore.Data.ID GetPreviousQuestionFromAnalytics(Guid ChatItemId)
        {
            return new ID("{7D3F64B7-4020-4D4A-8092-75BE56BA57D9}");
        }
        private Guid GetChat(string chatId=null)
        {
            Guid itemId = Guid.Empty;
            return IsValidGuid(chatId, ref itemId) ? itemId : new Guid("{7D3F64B7-4020-4D4A-8092-75BE56BA57D9}");
        }

        private bool IsValidGuid(string input,ref Guid itemId)
        {
            return Guid.TryParse(input, out itemId) && !Guid.Equals(itemId, Guid.Empty);
        }

        private JsonResult BuildResponseJson(object  model)
        {
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private JsonResult BuildChartModel(Item item, List<ChatRequest> chatRequests = null)
        {
            var model = BuildBaseRequest(item);
            BuildRequestOptions(item, model);

            if (chatRequests != null && chatRequests.Any())
            {
                model.ExistingChat = chatRequests;
            }
            return BuildResponseJson(model);
        }

        private void BuildRequestOptions(Item item, ChatRequest model)
        {
            MultilistField multilistField = item.Fields["Answer Options"];
            if (multilistField != null && multilistField.Count > 0)
            {
                model.IsOptionBased = true;
                model.AnswerOptions = new List<Option>();
                foreach (Item eachItem in multilistField.GetItems())
                {
                    model.AnswerOptions.Add(GetOptionDetails(eachItem));
                }
            }
        }

        private ChatRequest BuildBaseRequest(Item item)
        {
            var model = new ChatRequest()
            {
                Id = item.ID.Guid,
                CanGoBack = GetBoolvalue(item, "Can Go back"),
                DisplayMessage = FieldValue(item, "Display Message"),
                QuestionDisplay = FieldValue(item, "Question Display"),
                PlaceHolderText = FieldValue(item, "PlaceHolder Text To Display"),
                RegexPatternToValidate = FieldValue(item, "Regex Pattern To Check"),
                IsOptionBased = false,
                ChatId = ChatItemId
            };
            return model;
        }
        private string FieldValue(Item item, string fieldName)
        {
            return item?.Fields[fieldName]?.Value;
        }
        private Option GetOptionDetails(Item item)
        {
         return   new Option()
            {
                CanGoBack = GetBoolvalue(item, "Can Go back"),
                Id = item.ID.Guid,
                OptionDisplayText = item.Fields["Option Display Text"].Value,
            };
        }
        private Item GetNextItem(Item item)
        {
            LookupField address = (LookupField)item.Fields["Next Question To Point"];
            return address?.TargetItem;
        }
        private Item getItem(Guid guid)
        {
            var item = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(guid));
            if (!item?.Template?.ID?.IsNull ?? false)
            {
                var templateId = item?.TemplateID.Guid.ToString("D");
                List<string> allowedTemplateIds = new List<string>();
                allowedTemplateIds.Add("D31983A8-A8F4-4A6B-8E15-BC77B88A69F3");
                allowedTemplateIds.Add("D46E65EC-6275-4C26-86C0-CA2022492189");
                allowedTemplateIds.Add("B3B7E257-5BBC-4D01-9BCE-919DA03DB528");
                allowedTemplateIds.Add("280FDF9C-CAB4-4BF9-B09D-9FB08D61B379");
                if (allowedTemplateIds.Any(f => f.Equals(templateId, StringComparison.OrdinalIgnoreCase)))
                    return item;
            }
            return null;
        }
        private bool GetBoolvalue(Item item, string fieldName)
        {
            CheckboxField chk = item.Fields[fieldName];

            return chk?.Checked ?? false;
        }

    }
}