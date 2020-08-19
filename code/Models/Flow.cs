using Amgen.Web.Helper.Common.GlassMapper;
using Amgen.Web.Helper.Common.GlassMapper.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amgen.Web.Feature.ChatBot.Models
{

    [SitecoreType(TemplateId = ModelConstants.FlowName.TemplateId)]
    public interface IFlow : IGoBack, IOptionBase, IQuestionsection
    {
        [SitecoreField(FieldId = ModelConstants.FlowName.UserInputId)]
        string UserInputField { get; set; }

        [SitecoreField(FieldId = ModelConstants.Option.OptionDisplayTextId)]
        string OptionDisplayText { get; set; }

        [SitecoreField(FieldId = ModelConstants.FlowName.PlaceholderTextId)]
        string PlaceholderText { get; set; }

        [SitecoreField(FieldId = ModelConstants.FlowName.IsMandatoryId)]
        bool IsMandatory { get; set; }

        [SitecoreField(FieldId = ModelConstants.FlowName.RegexPatternId)]
        string RegexPattern { get; set; }



     
    }
}