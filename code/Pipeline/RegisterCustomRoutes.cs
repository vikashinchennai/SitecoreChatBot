using Sitecore.Pipelines;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Web.Feature.ChatBot.Pipelines
{
    public class RegisterCustomRoutes
    {
        public void Process(PipelineArgs args)
        {
            const string solutionName = "Sitecore.Web.Feature.ChatBot";

            RouteTable.Routes.MapRoute(
                name: $"{solutionName}.ChatBot.Api",
                url: $"MyApi/Custom/{{controller}}/{{action}}/{{id}}",
                defaults: new { controller = "ChatBot", id = UrlParameter.Optional },
                namespaces: new[] { $"Sitecore.Web.Feature.ChatBot.Controllers" }
            );
        }
    }
}