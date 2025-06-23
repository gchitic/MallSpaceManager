using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Floor.Services
{
    public class FloorDataService
    {
        private readonly IOrganizationService service;

        public FloorDataService(IOrganizationService service)
        {
            this.service = service;
        }

        public EntityReference getMallReference(Entity floor, Entity preImage)
        {
            if (floor.Attributes.Contains("giulia_mall"))
                return floor.GetAttributeValue<EntityReference>("giulia_mall");
            if (preImage != null && preImage.Contains("giulia_mall"))
                return preImage.GetAttributeValue<EntityReference>("giulia_mall");

            throw new InvalidPluginExecutionException("Select a mall record!");
        }

        public string getMallName(EntityReference mallRef)
        {
            Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_name"));
            return mall.GetAttributeValue<string>("giulia_name");
        }

        public int getMallFloors(EntityReference mallRef)
        {
            Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_floors"));
            return mall.GetAttributeValue<int>("giulia_floors");
        }

        public int getFloorNumber(Entity floor, Entity preImage)
        {
            if (floor.Attributes.Contains("giulia_floornumber"))
                return floor.GetAttributeValue<int>("giulia_floornumber");
            if (preImage != null && preImage.Contains("giulia_floornumber"))
                return preImage.GetAttributeValue<int>("giulia_floornumber");
            throw new InvalidPluginExecutionException("Insert the floor number!");
        }

        public List<int> getAllFloorsRelatedToMall(Guid mallGuid, Guid? currentFloorId)
        {
            List<int> allMallFloors = new List<int>();
            QueryExpression floorQuery = new QueryExpression("giulia_floor")
            {
                ColumnSet = new ColumnSet("giulia_floornumber"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("giulia_mall", ConditionOperator.Equal, mallGuid)
                    }
                }
            };

            EntityCollection allFloorsEntities = service.RetrieveMultiple(floorQuery);

            //Insert it in a list
            foreach (var floors in allFloorsEntities.Entities)
            {
                var floorId = floors.Id;
                if (currentFloorId.HasValue && floorId == currentFloorId.Value)
                    continue; // Skip the current floor on update

                allMallFloors.Add(floors.GetAttributeValue<int>("giulia_floornumber"));
            }

            return allMallFloors;
        }
    }
}
