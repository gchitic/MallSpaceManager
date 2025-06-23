using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Workflow.ComponentModel.Compiler;

namespace MallSpace_Plugins.Floor.Handlers
{
    public class FloorCreateHandler
    {
        private readonly IOrganizationService service;
        //private readonly FloorNameGenerator floorNameGenerator;
        private readonly FloorNameGenerate floorNameGenerate;
        private readonly FloorValidator floorValidator;
        private readonly OccupiedSpaceCalculator occupiedSpaceCalculator;
        private readonly FloorDataService floorDataService;
        public FloorCreateHandler(IOrganizationService service, FloorNameGenerate floorNameGenerate,
            FloorValidator floorValidator, OccupiedSpaceCalculator occupiedSpaceCalculator,
            FloorDataService floorDataService)
        {
            this.service = service;
            this.floorNameGenerate = floorNameGenerate;
            this.floorValidator = floorValidator;
            this.occupiedSpaceCalculator = occupiedSpaceCalculator;
            this.floorDataService = floorDataService;
        }

        public void HandleCreate(Entity floor)
        {
            var name = generateAndValidateFloorName(floor, null);
            floor["giulia_name"] = name;
            floor["giulia_occupiedspace"] = occupiedSpaceCalculator.InitializeOccupiedSpace();
        }

        public void HandleUpdate(Entity floor, Entity preImage)
        {
            var name = generateAndValidateFloorName(floor, preImage);
            floor["giulia_name"] = name;
        }

        public string generateAndValidateFloorName(Entity floor, Entity preImage)
        {
            //get all field values
            var mallRef = floorDataService.getMallReference(floor, preImage);
            var mallName = floorDataService.getMallName(mallRef);
            var mallMaxFloors = floorDataService.getMallFloors(mallRef);
            var floorNumber = floorDataService.getFloorNumber(floor, preImage);
            var allFloors = floorDataService.getAllFloorsRelatedToMall(mallRef.Id, floor.Id);

            //validate
            if (!floorValidator.isInRange(floorNumber, mallMaxFloors))
                throw new InvalidPluginExecutionException("Floor number exceeds mall floor count.");
            if(!floorValidator.isUnique(floorNumber, allFloors))
                throw new InvalidPluginExecutionException("Floor number must be unique within the mall.");

            return floorNameGenerate.generateFloorName(mallName, floorNumber);
        }
    }
}
