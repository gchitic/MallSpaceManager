using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Linq;

namespace MallSpace_Plugins.Opportunity.Handlers.Commands
{
    public class ApproveHandler
    {
        public void Handle(IWorkflowContext wfContext, IOrganizationService service, Guid opportunityGuid)
        {
            //Get Approval security roles of the user
            var userRolesQuery = new QueryExpression("role")
            {
                ColumnSet = new ColumnSet("name"),
                LinkEntities =
                {
                    new LinkEntity("role", "systemuserroles", "roleid", "roleid", JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("systemuserid", ConditionOperator.Equal, wfContext.InitiatingUserId)
                            }
                        }
                    }
                }
            };

            var userRoles = service.RetrieveMultiple(userRolesQuery)
                                    .Entities
                                    .Select(r => r.GetAttributeValue<string>("name"))
                                    .ToList();



            //Retrieve Approval records related to the opportunity
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

            var approvals = service.RetrieveMultiple(approvalQuery).Entities;

            foreach(var approval in approvals )
            {
                var approvalTeam = approval.GetAttributeValue<string>("giulia_approvalteam");

                if(userRoles.Contains(approvalTeam))    //Match by name
                {
                    approval["giulia_status"] = new OptionSetValue(343530000);
                    service.Update(approval);
                }    
            }


            //Check if all are approved 
            bool allApproved = approvals.All(a =>
                    a.GetAttributeValue<OptionSetValue>("giulia_status").Value == 343530000);


            // Get the user's roles again (already done above)
            var userCanStillApprove = approvals.Any(a =>
                userRoles.Contains(a.GetAttributeValue<string>("giulia_approvalteam")) &&
                a.GetAttributeValue<OptionSetValue>("giulia_status").Value != 343530000
            );

            if (allApproved)
            {
                var opportunity = new Entity("giulia_opportunity", opportunityGuid);
                opportunity["giulia_offerapproved"] = true;
                service.Update(opportunity);
            }

        }
    }
}
