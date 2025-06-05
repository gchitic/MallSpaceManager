using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MallSpace.Plugins.Floor
{
    public class Create_GenerateFloorName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity floor = (Entity)context.InputParameters["Target"];

                var generator = new FloorNameGenerator(context, service);
                generator.Generate(floor);

                
            }

        }
    }
}
