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
        public Money getPricePerM2(Entity opportunity)
        {
            return opportunity.Contains("giulia_priceperm2") ? 
                opportunity.GetAttributeValue<Money>("giulia_priceperm2") : null;
        }

        public decimal? getOfferedSpace(Entity opportunity) 
        { 
            return opportunity.Contains("giulia_offeredspace") ? 
                opportunity.GetAttributeValue<decimal?>("giulia_offeredspace") : null;
        }
    }
}
