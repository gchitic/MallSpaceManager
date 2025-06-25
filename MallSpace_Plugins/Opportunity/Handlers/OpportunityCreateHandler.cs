using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Handlers;
using MallSpace_Plugins.Opportunity.Business_Logic;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Handlers
{
    public class OpportunityCreateHandler
    {
        private readonly IPluginExecutionContext context;
        private readonly OpportunityRentCostCalculator calculator;
        private readonly OpportunityDefaultValues defaultValues;
        private readonly ReadOnlyFieldRules readOnlyFieldRules;
        private readonly OccupiedSpaceCalculator occupiedSpaceCalculator;
        private readonly OpportunityFieldService fieldService;

        public OpportunityCreateHandler(OpportunityRentCostCalculator calculator, OpportunityDefaultValues defaultValues,
            ReadOnlyFieldRules readOnlyFieldRules, IPluginExecutionContext context, OpportunityFieldService fieldService)
        {
            this.calculator = calculator;
            this.defaultValues = defaultValues;
            this.readOnlyFieldRules = readOnlyFieldRules;

            this.context = context;
            this.fieldService = fieldService;
        }

        public void Handle(Entity opportunity)
        {
            rentCostCalculation( opportunity );

            //Set default values
            opportunity["giulia_contractpreparationstatus"] = defaultValues.contractPreparationStatusSetDefaultValue();
        }

        public void rentCostCalculation(Entity opportunity)
        {
            //Extract needed fields values : priceperm2,offeredspace
            var pricePerM2 = fieldService.getPricePerM2(opportunity);
            var offeredSpace = fieldService.getOfferedSpace(opportunity);

            //Check if readonly fields are not filled
            EnforceReadOnlyFields(opportunity);

            //Calculate & set the value
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
