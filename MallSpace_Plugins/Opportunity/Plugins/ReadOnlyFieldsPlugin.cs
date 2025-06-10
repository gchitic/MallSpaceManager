using MallSpace_Plugins.Opportunity.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Plugins
{
    public class ReadOnlyFieldsPlugin : IPlugin
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
                Entity opportunity = (Entity)context.InputParameters["Target"];

                try
                {
                    if(context.MessageName == "Create" || context.MessageName == "Update")
                    {
                        var handler = new ReadOnlyFieldsHandler();
                        handler.protectReadOnlyFields(context, opportunity);
                    }            
                }
                catch (InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in OpportunityCreatePlugin: " + ex.Message, ex);
                }

            }
        }
    }
}
