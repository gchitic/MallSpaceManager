using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace MallSpace_Plugins.Opportunity.Business_Logic
{
    public class ApprovalService
    {
        private readonly IOrganizationService service;

        public ApprovalService(IOrganizationService service)
        {
            this.service = service;
        }
        public List<string> getApprovalTeams()
        {
            var approvalTeams = new List<string>();
            QueryExpression query = new QueryExpression("giulia_customconfigurations")
            {
                ColumnSet = new ColumnSet("giulia_key")
            };
            EntityCollection teamCollection =  service.RetrieveMultiple(query);

            foreach (Entity config in teamCollection.Entities)
            {
                var key = config.GetAttributeValue<string>("giulia_key");
                if (!string.IsNullOrEmpty(key))
                    approvalTeams.Add(key);
            }

            return approvalTeams;
        }

        public void createApprovalRecords(Guid opportunityGuid, List<string> approvalTeams)
        {
            foreach (var team in approvalTeams)
            {
                Entity approvalsEntity = new Entity("giulia_approvals");
                approvalsEntity["giulia_status"] = new OptionSetValue(343530002);
                approvalsEntity["giulia_opportunity"] = new EntityReference("giulia_opportunity", opportunityGuid);
                approvalsEntity["giulia_approvalteam"] = team;
                service.Create(approvalsEntity);
            }
        }

        public void markApprovalSubmitedAsYes(Guid opportunityGuid)
        {
            Entity opportunity = new Entity("giulia_opportunity", opportunityGuid);
            opportunity["giulia_approvalsubmited"] = true;
            service.Update(opportunity);
        }
    }
}
