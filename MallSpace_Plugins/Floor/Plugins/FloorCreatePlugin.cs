using MallSpace_Plugins.Floor.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Floor.Plugins
{
    public  class FloorCreatePlugin : IPlugin
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
