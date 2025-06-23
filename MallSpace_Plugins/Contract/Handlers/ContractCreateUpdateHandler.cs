using MallSpace_Plugins.Floor.Business_Logic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Contract.Handlers
{
    public class ContractCreateUpdateHandler
    {
        private readonly OccupiedSpaceCalculator occupiedSpaceCalculator;

        public ContractCreateUpdateHandler(OccupiedSpaceCalculator occupiedSpaceCalculator)
        {
            this.occupiedSpaceCalculator = occupiedSpaceCalculator;
        }

        public void Handle(IOrganizationService service, Entity contract)
        {
            //1.get opportunity entity by id
            Guid opportunityGuid = contract.GetAttributeValue<EntityReference>("giulia_opportunity").Id;

            //2.extract from opportunity status reason, offeredspace, floorId
            Entity opportunity = service.Retrieve("giulia_opportunity", opportunityGuid, new ColumnSet("giulia_offeredspace", "giulia_floor"));
            
            //extract floor id from opportunity
            Guid floorGuid = opportunity.GetAttributeValue<EntityReference>("giulia_floor").Id;
            Entity floor = service.Retrieve("giulia_floor", floorGuid, new ColumnSet("giulia_totalspace", "giulia_occupiedspace"));

            //3.set occupiedSpace from floor
                decimal? totalSpace = floor.GetAttributeValue<decimal?>("giulia_totalspace");
                decimal? occupiedSpace = floor.GetAttributeValue<decimal?>("giulia_occupiedspace");
                decimal? offeredSpace = opportunity.GetAttributeValue<decimal?>("giulia_offeredspace");

                if (totalSpace - occupiedSpace > offeredSpace) 
                {
                    floor["giulia_occupiedspace"] = occupiedSpaceCalculator.calculateOccupiedSpace(offeredSpace, occupiedSpace);
                    service.Update(floor);
    
                }
                else
                {
                    throw new InvalidPluginExecutionException("No more space on this floor");
                }
        }
    }
}
