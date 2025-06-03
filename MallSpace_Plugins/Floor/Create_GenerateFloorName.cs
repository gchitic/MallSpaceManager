using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MallSpace_Plugins
{
    public class Create_GenerateFloorName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity floor = (Entity)context.InputParameters["Target"];

                try
                {
                    string mallName="", floorNumber;

                    EntityReference mallRef = floor.GetAttributeValue<EntityReference>("giulia_mall");
                    Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_name"));
                    mallName = mall.GetAttributeValue<string>("giulia_name");

                    floorNumber = floor.GetAttributeValue<int>("giulia_floornumber").ToString();
                    floor["giulia_name"] = $"{mallName} Floor{floorNumber}";
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Error in Generate Floor Name plugin: " + ex.Message, ex);
                }
            }

        }
    }
}
