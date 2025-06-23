using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Windows.Documents;

namespace MallSpace_Plugins.Floor.Business_Logic
{
    public class FloorNameGenerator
    {
        private readonly IPluginExecutionContext context;
        private readonly IOrganizationService service;

        public FloorNameGenerator(IOrganizationService service)
        {
            this.service = service;
        }

        public string generateFloorName(Entity floor, Entity preimage)
        {
            var mallRef = getMallReference(floor, preimage);
            var mallName = getMallName(mallRef);
            int mallFloor = getMallFloors(mallRef);

            List<int> allMallFloors = new List<int>();
            allMallFloors = getAllFloorsRelatedToMall(service, mallRef.Id);

            bool ifFloorNumberValid = isFloorNumberValid(floor, preimage, mallFloor);
            bool ifMallNumberUnique = isMallNumberUnique(floor, preimage, allMallFloors);
            if (ifFloorNumberValid && ifMallNumberUnique)
            {       
                var floorNumber = getFloorNumber(floor, preimage);
                return $"{mallName} Floor{floorNumber}";
            }
            throw new InvalidPluginExecutionException("This floor number is invalid.");
            return null;
        }

        public decimal initializeOccupiedSpace() => 0.0m; 

        private EntityReference getMallReference(Entity floor, Entity preImage)
        {
            if (floor.Attributes.Contains("giulia_mall"))
                return floor.GetAttributeValue<EntityReference>("giulia_mall");
            if (preImage != null && preImage.Contains("giulia_mall"))
                return preImage.GetAttributeValue<EntityReference>("giulia_mall");

            throw new InvalidPluginExecutionException("Select a mall record!");
        }

        private string getMallName(EntityReference mallRef)
        {
            Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_name"));
            return mall.GetAttributeValue<string>("giulia_name");
        }

        private int getMallFloors(EntityReference mallRef)
        {
            Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_floors"));
            return mall.GetAttributeValue<int>("giulia_floors");
        }

        private int getFloorNumber(Entity floor, Entity preImage)
        {
            if (floor.Attributes.Contains("giulia_floornumber"))
                return floor.GetAttributeValue<int>("giulia_floornumber");
            if (preImage != null && preImage.Contains("giulia_floornumber"))
                return preImage.GetAttributeValue<int>("giulia_floornumber");
            throw new InvalidPluginExecutionException("Insert the floor number!");
        }

        private List<int> getAllFloorsRelatedToMall(IOrganizationService service, Guid mallGuid)
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
            foreach(var floors in allFloorsEntities.Entities)
            {
                allMallFloors.Add(floors.GetAttributeValue<int>("giulia_floornumber"));
            }

            return allMallFloors;
        }

        private bool isFloorNumberValid(Entity floor, Entity preImage, int mallFloors)
        {
            if(floor.Attributes.Contains("giulia_floornumber"))
            {
                if (floor.GetAttributeValue<int>("giulia_floornumber") <= mallFloors)
                    return true;
            }
            else if (preImage.Attributes.Contains("giulia_floornumber"))
            {
                if (preImage.GetAttributeValue<int>("giulia_floornumber") <= mallFloors)
                    return true;
            }
                    
            return false;
        }

        private bool isMallNumberUnique(Entity floor, Entity preImage, List<int> allMallFloors)
        {
            int floorNumber = 0;
            if (floor.Attributes.Contains("giulia_floornumber"))
                floorNumber = floor.GetAttributeValue<int>("giulia_floornumber");
            else if (preImage.Attributes.Contains("giulia_floornumber"))
                floorNumber = preImage.GetAttributeValue<int>("giulia_floornumber");

            foreach (var floors in allMallFloors)
                {
                    if (floorNumber == floors)
                        return false;
                }
            return true;
        }
    }
}
