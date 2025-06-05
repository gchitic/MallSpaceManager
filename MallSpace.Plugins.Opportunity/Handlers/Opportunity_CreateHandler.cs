using MallSpace.Plugins.Opportunity.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace.Plugins.Opportunity.Handlers
{
    public class Opportunity_CreateHandler
    {
        private readonly Opportunity_RentCostCalculator calculator;

        public Opportunity_CreateHandler()
        {
            calculator = new Opportunity_RentCostCalculator();
        }

        public void Handle(Entity opportunity)
        {
            try
            {
                var pricePerM2 = opportunity.GetAttributeValue<decimal>("giulia_priceperm2");
                var offeredSpace = opportunity.GetAttributeValue<decimal>("giulia_offeredspace");

                var rentCost = calculator.Calculator(pricePerM2, offeredSpace);
                if (rentCost != null)
                {
                    opportunity["giulia_rentcost"] = rentCost;
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("Error: " + ex.Message, ex);
            }
            

        }
    }
}
