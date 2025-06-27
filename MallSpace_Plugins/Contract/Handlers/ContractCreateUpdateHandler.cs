using MallSpace_Plugins.Contract.Services;
using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Services;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Contract.Handlers
{
    public class ContractCreateUpdateHandler
    {
        private readonly OccupiedSpaceCalculator occupiedSpaceCalculator;
        private readonly FloorValidator floorValidator;

        //Services
        private readonly FloorDataService floorDataService;
        private readonly OpportunityFieldService opportunityFieldService;
        private readonly ContractDataService contractDataService;

        public ContractCreateUpdateHandler(OccupiedSpaceCalculator occupiedSpaceCalculator, FloorDataService floorDataService, OpportunityFieldService opportunityFieldService, 
            FloorValidator floorValidator, ContractDataService contractDataService)
        {
            this.occupiedSpaceCalculator = occupiedSpaceCalculator;

            //Services
            this.floorDataService = floorDataService;
            this.opportunityFieldService = opportunityFieldService;
            this.floorValidator = floorValidator;
            this.contractDataService = contractDataService;
        }

        public void Handle(IOrganizationService service, Entity contract)
        {
            Guid opportunityGuid = contractDataService.getOpportunityId(contract);

            //extract from opportunity offeredspace, floorId
            Entity opportunity = opportunityFieldService.getOpportunityWithFields(opportunityGuid);

            //extract floor id from opportunity
            Guid floorGuid = opportunityFieldService.getFloorGuid(opportunity);
            Entity floor = floorDataService.getFloorWithFields(floorGuid);

            //set values
            decimal? totalSpace = floorDataService.getTotalSpace(floor);
            decimal? occupiedSpace = floorDataService.getOccupiedSpace(floor);
            decimal? offeredSpace = opportunityFieldService.getOfferedSpace(opportunity, null);

            //Calculate offeredSpace
            floorValidator.ensureSpaceIsAvailable(totalSpace, occupiedSpace, offeredSpace);
            var space = occupiedSpaceCalculator.calculateOccupiedSpace(offeredSpace, occupiedSpace);
            floorDataService.setOccupiedSpace(floor, space);
        }
    }
}
