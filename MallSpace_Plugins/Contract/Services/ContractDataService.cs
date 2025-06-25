using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Contract.Services
{
    public class ContractDataService
    {
        private readonly IOrganizationService service;
        public ContractDataService(IOrganizationService service)
        {
            this.service = service;
        }

        public Guid getOpportunityId(Entity contract)
        {
            return contract.GetAttributeValue<EntityReference>("giulia_opportunity").Id;
        }
    }
}
