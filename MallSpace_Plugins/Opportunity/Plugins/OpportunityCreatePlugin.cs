using MallSpace_Plugins.Opportunity.Business_Logic;
using MallSpace_Plugins.Opportunity.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Plugins
{
    public class OpportunityCreatePlugin : IPlugin
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

                //Dependencies
                var calculator = new OpportunityRentCostCalculator();

                try
                {
                    if(context.MessageName == "Create")
                    {
                        var handler = new OpportunityCreateHandler();
                        handler.Handle(opportunity, calculator);
                    }
                    if(context.MessageName == "Update")
                    {
                        var preImage = context.PreEntityImages.Contains("PreImage") ?
                            context.PreEntityImages["PreImage"] : null;

                        var updateHandler = new OpportunityUpdateHandler();
                        updateHandler.Handle(opportunity, preImage, calculator);
                    }

                }
                catch(InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in OpportunityCreatePlugin: " + ex.Message, ex);
                }

            }
        }
    }
}
