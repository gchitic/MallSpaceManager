using MallSpace_Plugins.Opportunity.Handlers.Commands;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;


namespace MallSpace_Plugins.Opportunity.Plugins.Commands
{
    public class SubmitForApprovalActionPlugin : CodeActivity
    {
        [Input("OpportunityId")]
        public InArgument<string> OpportunityId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext wfContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(wfContext.UserId);

            //Dependencies
            OpportunityFieldService opportunityFieldService = new OpportunityFieldService(service);
            //Services
            ApprovalService approvalService = new ApprovalService(service);

            try
            {
                //Extract the guid from input parameter
                var opportunityId = OpportunityId.Get(context);
                if (!Guid.TryParse(opportunityId, out Guid opportunityGuid))
                {
                    throw new InvalidPluginExecutionException("Invalid or missing OpportunityId.");
                }

                SubmitForApprovalHandler handler = new SubmitForApprovalHandler(approvalService, opportunityFieldService);
                handler.Handle(opportunityGuid);
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("Error in SubmitForApprovalPlugin: " + ex.Message);
            }
        }
    }
}
