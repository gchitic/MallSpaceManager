using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Services
{
    public class OpportunityFieldService
    {
        private readonly IOrganizationService service;
        public OpportunityFieldService(IOrganizationService service) 
        {
            this.service = service;
        }

        //GET
        public Money getPricePerM2(Entity opportunity, Entity preImage)
        {
            return opportunity.Contains("giulia_priceperm2")
                ? opportunity.GetAttributeValue<Money>("giulia_priceperm2")
                : preImage.Contains("giulia_priceperm2")
                    ? preImage.GetAttributeValue<Money>("giulia_priceperm2")
                    : null;
        }

        public decimal? getOfferedSpace(Entity opportunity, Entity preImage) 
        { 
            return opportunity.Contains("giulia_offeredspace") 
                ? opportunity.GetAttributeValue<decimal?>("giulia_offeredspace")
                : preImage.Contains("giulia_offeredspace")
                    ? preImage.GetAttributeValue<decimal?>("giulia_offeredspace")
                    : null;
        }


        //SET
        public void setOpportunityStatusReason(Entity opportunity, OptionSetValue value)
        {
            opportunity["giulia_statusreason"] = value;
            service.Update(opportunity);
        }
    }
}
