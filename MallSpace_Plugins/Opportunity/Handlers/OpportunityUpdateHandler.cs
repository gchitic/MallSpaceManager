using MallSpace_Plugins.Opportunity.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Handlers
{
    public class OpportunityUpdateHandler
    {
        private readonly IPluginExecutionContext context;
        private readonly OpportunityRentCostCalculator calculator;
        private readonly ReadOnlyFieldRules readOnlyFieldRules;

        public OpportunityUpdateHandler(OpportunityRentCostCalculator calculator, ReadOnlyFieldRules readOnlyFieldRules, IPluginExecutionContext context)
        {
            this.calculator = calculator;
            this.readOnlyFieldRules = readOnlyFieldRules;
            this.context = context;
        }

        public void Handle(Entity opportunity, Entity preImage, OpportunityRentCostCalculator calculator)
        {
            var pricePerM2 = opportunity.Contains("giulia_priceperm2") ?
                opportunity.GetAttributeValue<Money>("giulia_priceperm2") : 
                preImage?.GetAttributeValue<Money>("giulia_priceperm2");
            var offeredSpace = opportunity.Contains("giulia_offeredspace") ?
                opportunity.GetAttributeValue<decimal?>("giulia_offeredspace") : 
                preImage?.GetAttributeValue<decimal?>("giulia_offeredspace");

            EnforceReadOnlyFields(opportunity);
            var rentCost = calculator.Calculator(pricePerM2, offeredSpace);

            if (rentCost != null)
            {
                opportunity["giulia_rentcost"] = rentCost;
            }
        }

        private void EnforceReadOnlyFields(Entity opportunity)
        {
            readOnlyFieldRules.Validate(opportunity, context);
        }
    }
}
