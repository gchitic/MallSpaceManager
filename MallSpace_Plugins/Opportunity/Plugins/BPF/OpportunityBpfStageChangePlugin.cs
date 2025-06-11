using MallSpace_Plugins.Opportunity.Handlers.BPF;
using Microsoft.Xrm.Sdk;
using System;


namespace MallSpace_Plugins.Opportunity.Plugins.BPF
{
    public class OpportunityBpfStageChangePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof (IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity opportunityBPF = (Entity)context.InputParameters["Target"];

                if(opportunityBPF.LogicalName != "giulia_opportunitybpf") { return; }

                try
                {
                    var stageChangeHandler = new StageChangeHandler();
                    stageChangeHandler.Handle(service, opportunityBPF);
                }
                catch(InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in OpportunityBpfChangeStagePlugin: " + ex.Message + ex);
                }
            }
        }
    }
}
