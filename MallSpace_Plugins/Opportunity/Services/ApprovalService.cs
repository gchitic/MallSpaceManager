using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Services
{
    public class ApprovalService
    {
        private readonly IOrganizationService service;

        public ApprovalService(IOrganizationService service)
        {
            this.service = service;
        }

        //GET
        public List<string> getApprovalTeams()
        {
            var approvalTeams = new List<string>();
            QueryExpression query = new QueryExpression("giulia_customconfigurations")
            {
                ColumnSet = new ColumnSet("giulia_key")
            };
            EntityCollection teamCollection = service.RetrieveMultiple(query);

            foreach (Entity config in teamCollection.Entities)
            {
                var key = config.GetAttributeValue<string>("giulia_key");
                if (!string.IsNullOrEmpty(key))
                    approvalTeams.Add(key);
            }

            return approvalTeams;
        }

        public IEnumerable<Entity> getApprovalsByOpportunity(Guid opportunityGuid)
        {
            var approvalQuery = new QueryExpression("giulia_approvals")
            {
                ColumnSet = new ColumnSet("giulia_approvalteam", "giulia_status"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("giulia_opportunity", ConditionOperator.Equal, opportunityGuid)
                    }
                }
            };
            return service.RetrieveMultiple(approvalQuery).Entities;
        }


        //SET
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

        public void setStatusApproved(List<string> userRoles, IEnumerable<Entity> approvals)
        {
            foreach (var approval in approvals)
            {
                var approvalTeam = approval.GetAttributeValue<string>("giulia_approvalteam");

                if (userRoles.Contains(approvalTeam))    //Match by name
                {
                    approval["giulia_status"] = new OptionSetValue(343530000);
                    service.Update(approval);
                }
            }
        }

        public bool areAllApproved(IEnumerable<Entity> approvals)
        {
            return approvals.All(a =>
                    a.GetAttributeValue<OptionSetValue>("giulia_status").Value == 343530000);
        }

        public bool canUserStillApprove(IEnumerable<Entity> approvals, List<string> userRoles)
        {
            return approvals.Any(a =>
                userRoles.Contains(a.GetAttributeValue<string>("giulia_approvalteam")) &&
                a.GetAttributeValue<OptionSetValue>("giulia_status").Value != 343530000);
        }
    }
}
