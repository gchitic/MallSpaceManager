using MallSpace_Plugins.Opportunity.Business_Logic;
using MallSpace_Plugins.Opportunity.Handlers.Commands;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Plugins.Commands
{
    public class ApproveActionPlugin : CodeActivity
    {
        [Input("OpportunityId")]
        public InArgument<string> OpportunityId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext wfContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(wfContext.UserId);

            try
            {
                Guid opportunityGuid = wfContext.PrimaryEntityId;

                //throw new InvalidPluginExecutionException("Succes!" + opportunityGuid.ToString());

                ApproveHandler handler = new ApproveHandler();
                handler.Handle(wfContext, service, opportunityGuid);
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("Error in ApproveActionPlugin: " + ex.Message);
            }
        }
    }
}
