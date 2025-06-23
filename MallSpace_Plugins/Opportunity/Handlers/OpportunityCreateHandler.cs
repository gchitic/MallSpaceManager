using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Handlers;
using MallSpace_Plugins.Opportunity.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Handlers
{
    public class OpportunityCreateHandler
    {
        private readonly OpportunityRentCostCalculator calculator;
        private readonly OpportunityDefaultValues defaultValues;
        private readonly OccupiedSpaceCalculator occupiedSpaceCalculator;

        public OpportunityCreateHandler(OpportunityRentCostCalculator calculator, OpportunityDefaultValues defaultValues )
        {
            this.calculator = calculator;
            this.defaultValues = defaultValues;
        }

        public void Handle(Entity opportunity)
        {
            rentCostCalculation( opportunity );

            //Set default values
            opportunity["giulia_contractpreparationstatus"] = defaultValues.contractPreparationStatusSetDefaultValue();
        }

        public void rentCostCalculation(Entity opportunity)
        {
            //Rent cost calculation
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
