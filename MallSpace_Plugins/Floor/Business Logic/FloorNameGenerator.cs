using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace MallSpace_Plugins.Floor.Business_Logic
{
    internal class FloorNameGenerator
    {
        private readonly IPluginExecutionContext context;
        private readonly IOrganizationService service;

        public FloorNameGenerator(IPluginExecutionContext context, IOrganizationService service)
        {
            this.context = context;
            this.service = service;
        }

        public void Generate(Entity floor)
        {
            try
            {
                var preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;
                EntityReference mallRef = getMallReference(floor, preImage);

                string mallName = getMallName(mallRef);
                int floorNumber = getFloorNumber(floor, preImage);
                floor["giulia_name"] = $"{mallName} Floor{floorNumber}";
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error: " + ex.Message, ex);
            }
        }

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

        private int getFloorNumber(Entity floor, Entity preImage)
        {
            if (floor.Attributes.Contains("giulia_floornumber"))
                return floor.GetAttributeValue<int>("giulia_floornumber");
            if (preImage != null && preImage.Contains("giulia_floornumber"))
                return preImage.GetAttributeValue<int>("giulia_floornumber");
            throw new InvalidPluginExecutionException("Insert the floor number!");
        }
    }
}
