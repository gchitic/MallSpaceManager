using MallSpace_Plugins.Opportunity.Business_Logic;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Handlers.Commands
{
    public class SubmitForApprovalHandler
    {
        private readonly ApprovalService approvalService;
        private readonly OpportunityFieldService opportunityFieldService;

        public SubmitForApprovalHandler(ApprovalService approvalService, OpportunityFieldService opportunityFieldService)
        {
            this.approvalService = approvalService;
            this.opportunityFieldService = opportunityFieldService;
        }
        public void Handle(Guid opportunityGuid)
        {
            List<string> approvalTeams = approvalService.getApprovalTeams();

            if (approvalTeams.Count == 0)
            {
                throw new InvalidPluginExecutionException("No approval teams configured.");
            }

            approvalService.createApprovalRecords(opportunityGuid, approvalTeams);
            opportunityFieldService.setApprovalSubmitedAsYes(opportunityGuid);
        }
    }
}
