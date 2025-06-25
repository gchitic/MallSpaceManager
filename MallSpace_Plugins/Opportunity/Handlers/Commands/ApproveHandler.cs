using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Linq;

namespace MallSpace_Plugins.Opportunity.Handlers.Commands
{
    public class ApproveHandler
    {
        private readonly UserAccesService userAccesService;
        private readonly OpportunityFieldService opportunityFieldService;
        private readonly ApprovalService approvalService;

        public ApproveHandler(UserAccesService userAccesService, OpportunityFieldService opportunityFieldService, ApprovalService approvalService)
        {
            this.userAccesService = userAccesService;
            this.opportunityFieldService = opportunityFieldService;
            this.approvalService = approvalService;
        }

        public void Handle(IWorkflowContext wfContext, IOrganizationService service, Guid opportunityGuid)
        {
            var userRoles = userAccesService.getUserSecurityRoles(wfContext);
            var approvals = approvalService.getApprovalsByOpportunity(opportunityGuid);

            approvalService.setStatusApproved(userRoles, approvals);

            //Check the permission of approval
            bool allApproved = approvalService.areAllApproved(approvals);
            bool canUserStillApprove = approvalService.canUserStillApprove(approvals, userRoles);

            if (allApproved)
                opportunityFieldService.setOpportunityAsApproved(opportunityGuid);

        }
    }
}
