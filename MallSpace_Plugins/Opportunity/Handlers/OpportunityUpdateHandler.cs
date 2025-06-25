using MallSpace_Plugins.Opportunity.Business_Logic;
using MallSpace_Plugins.Opportunity.Services;
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
        //Services
        OpportunityFieldService fieldService;
        RentCostService rentCostService;

        private readonly OpportunityRentCostCalculator calculator;
        private readonly ReadOnlyFieldRules readOnlyFieldRules;

        public OpportunityUpdateHandler(OpportunityRentCostCalculator calculator, ReadOnlyFieldRules readOnlyFieldRules, 
            IPluginExecutionContext context,OpportunityFieldService fieldService, RentCostService rentCostService)
        {
            this.calculator = calculator;
            this.readOnlyFieldRules = readOnlyFieldRules;

            this.context = context;
            //Services
            this.fieldService = fieldService;
            this.rentCostService = rentCostService;
        }

        public void Handle(Entity opportunity, Entity preImage, OpportunityRentCostCalculator calculator)
        {
            rentCostCalculation(opportunity, preImage);
        }

        public void rentCostCalculation(Entity opportunity, Entity preImage)
        {
            //Get field values
            var pricePerM2 = fieldService.getPricePerM2(opportunity, preImage);
            var offeredSpace = fieldService.getOfferedSpace(opportunity, preImage);

            //Check if read only fields are not filled
            EnforceReadOnlyFields(opportunity);

            //Calculate & set Rent Cost
            rentCostService.calculateAndSetRentCost(opportunity, pricePerM2, offeredSpace);
        }

        private void EnforceReadOnlyFields(Entity opportunity)
        {
            readOnlyFieldRules.Validate(opportunity, context);
        }
    }
}
