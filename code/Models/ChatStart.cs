
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amgen.Web.Feature.ChatBot.Models;
using Amgen.Web.Helper.Common.GlassMapper;
using Amgen.Web.Helper.Common.GlassMapper.Models;
using Sitecore.Foundation.Field.MultiSingleLineListField.CustomField;
using Sitecore.Data.Fields;


namespace Amgen.Web.Feature.ChatBot.Models
{
    [SitecoreType(TemplateId = ModelConstants.ChatStartPoint.TemplateId)]

    public class ChatStart : GlassBase
    {
        [SitecoreField(FieldId = ModelConstants.ChatStartPoint.DisplayMessageId)]
        public string DisplayMessage { get; set; }

        [SitecoreField(FieldId = ModelConstants.QuestionSection.QuestionDisplayId)]
        public string QuestionDisplay { get; set; }

        [SitecoreField(FieldId = ModelConstants.QuestionSection.AnswerOptionsId)]
        public IEnumerable<IOption> AnswerOptions { get; set; }
        
    }
    [SitecoreType(TemplateId = ModelConstants.QuestionSection.TemplateId)]
    public interface IQuestionsection
    {
        [SitecoreField(FieldId = ModelConstants.QuestionSection.QuestionDisplayId)]
         string QuestionDisplay { get; set; }

        [SitecoreField(FieldId = ModelConstants.QuestionSection.AnswerOptionsId)]
         IEnumerable<IOption> AnswerOptions { get; set; }
    }
    [SitecoreType(TemplateId = ModelConstants.Option.TemplateId)]
    public interface IOption : IOptionBase, IGoBack
    {
        [SitecoreField(FieldId = ModelConstants.Option.OptionDisplayTextId)]
        string OptionDisplayText { get; set; }
    }

    [SitecoreType(TemplateId =ModelConstants.OptionBase.TemplateId)]
    public interface IOptionBase : IGlassBase
    {
        [SitecoreField(FieldId = ModelConstants.OptionBase.CallerApiId)]
        string CallerApi { get; set; }

        [SitecoreField(FieldId = ModelConstants.OptionBase.EndChatId)]
        bool EndChat { get; set; }

        [SitecoreField(FieldId = ModelConstants.OptionBase.NextQuestionId)]
        IEnumerable<IFlow> NextQuestion { get; set; }

        [SitecoreField(FieldId = ModelConstants.OptionBase.EndMessageId)]
        string EndMessage { get; set; }
    }

    [SitecoreType(TemplateId =ModelConstants.GoBack.TemplateId)]
    public interface IGoBack : IGlassBase
    {
        [SitecoreField(FieldId = ModelConstants.GoBack.GoBackId)]
        bool GoBack { get; set; }
    }
}