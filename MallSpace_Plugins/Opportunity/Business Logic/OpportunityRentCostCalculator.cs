using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Business_Logic
{
    public class OpportunityRentCostCalculator
    {
        public Money Calculator(Money pricePerM2, decimal? offeredSpace)
        {
            if (pricePerM2 == null && !offeredSpace.HasValue)
                return null;

            decimal result = (decimal)(pricePerM2.Value * offeredSpace);

            return new Money(result);
        }
    }
}
