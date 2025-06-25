using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MallSpace_Plugins.Opportunity.Services
{
    public class UserAccesService
    {
        private readonly IOrganizationService service;

        public UserAccesService(IOrganizationService service)
        {
            this.service = service;
        }

        public List<string> getUserSecurityRoles(IWorkflowContext wfContext)
        {
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

            return service.RetrieveMultiple(userRolesQuery)
                                    .Entities
                                    .Select(r => r.GetAttributeValue<string>("name"))
                                    .ToList();
        }

    }
}
