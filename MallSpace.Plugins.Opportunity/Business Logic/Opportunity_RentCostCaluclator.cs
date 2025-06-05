using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace.Plugins.Opportunity.Business_Logic
{
    public class Opportunity_RentCostCalculator
    {
        public decimal? Calculator(decimal? pricePerM2, decimal? offeredSpace)
        {
            if (!pricePerM2.HasValue && !offeredSpace.HasValue)
                return null;

            return pricePerM2 * offeredSpace;
        }
    }
}
