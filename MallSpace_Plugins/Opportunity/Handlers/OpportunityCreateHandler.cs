using MallSpace_Plugins.Opportunity.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Handlers
{
    public class OpportunityCreateHandler
    {
        private readonly OpportunityRentCostCalculator calculator;

        public OpportunityCreateHandler()
        {
            calculator = new OpportunityRentCostCalculator();
        }

        public void Handle(Entity opportunity, OpportunityRentCostCalculator calculator)
        {
            var pricePerM2 = opportunity.Contains("giulia_priceperm2") ?
                opportunity.GetAttributeValue<Money>("giulia_priceperm2") : null;
            var offeredSpace = opportunity.Contains("giulia_offeredspace") ?
                opportunity.GetAttributeValue<decimal?>("giulia_offeredspace") : null;

            var rentCost = calculator.Calculator(pricePerM2, offeredSpace);

            if (rentCost != null)
            {
                opportunity["giulia_rentcost"] = rentCost;
            }
        }
    }
}
