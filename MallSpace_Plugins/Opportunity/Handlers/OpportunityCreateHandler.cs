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
        //Services
        private readonly OpportunityFieldService fieldService;
        private readonly RentCostService rentCostService;

        private readonly OpportunityRentCostCalculator calculator;
        private readonly OpportunityDefaultValues defaultValues;
        private readonly ReadOnlyFieldRules readOnlyFieldRules;
        

        public OpportunityCreateHandler(OpportunityRentCostCalculator calculator, OpportunityDefaultValues defaultValues,
            ReadOnlyFieldRules readOnlyFieldRules, IPluginExecutionContext context, OpportunityFieldService fieldService, 
            RentCostService rentCostService)
        {
            this.calculator = calculator;
            this.defaultValues = defaultValues;
            this.readOnlyFieldRules = readOnlyFieldRules;

            this.context = context;
            //services
            this.fieldService = fieldService;
            this.rentCostService = rentCostService;
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
            var pricePerM2 = fieldService.getPricePerM2(opportunity, null);
            var offeredSpace = fieldService.getOfferedSpace(opportunity, null);

            //Check if readonly fields are not filled
            EnforceReadOnlyFields(opportunity);

            //Calculate & set the value
            rentCostService.calculateAndSetRentCost(opportunity, pricePerM2, offeredSpace);
        }

        private void EnforceReadOnlyFields(Entity opportunity)
        {
            readOnlyFieldRules.Validate(opportunity, context);
        }
    }
}
