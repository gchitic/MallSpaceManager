using MallSpace_Plugins.Opportunity.Business_Logic;
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
        public void Handle(ApprovalService approvalService, Guid opportunityGuid)
        {
            List<string> approvalTeams = approvalService.getApprovalTeams();

            if (approvalTeams.Count == 0)
            {
                throw new InvalidPluginExecutionException("No approval teams configured.");
            }

            approvalService.createApprovalRecords(opportunityGuid, approvalTeams);
            approvalService.markApprovalSubmitedAsYes(opportunityGuid);
        }
    }
}
