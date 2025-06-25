using MallSpace_Plugins.Opportunity.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Services
{
    public class RentCostService
    {
        private readonly OpportunityRentCostCalculator rentCostCalculator;

        public RentCostService(OpportunityRentCostCalculator rentCostCalculator)
        {
            this.rentCostCalculator = rentCostCalculator;
        }

        public void calculateAndSetRentCost(Entity opportunity, Money pricePerM2, decimal? offeredSpace)
        {
            var rentCost = rentCostCalculator.Calculator(pricePerM2, offeredSpace);
            if (rentCost != null) 
            {
                opportunity["giulia_rentcost"] = rentCost;
            }
        }
    }
}
