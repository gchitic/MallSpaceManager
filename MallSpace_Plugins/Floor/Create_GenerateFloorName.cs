using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MallSpace_Plugins.Floor
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
                    //string mallName = "", floorNumber="";
                    //EntityReference mallRef;
                    //if (floor.Attributes.Contains("giulia_mall")) 
                    //{
                    //    mallRef = floor.GetAttributeValue<EntityReference>("giulia_mall");
                    //}
                    //else if(context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"].Contains("giulia_mall"))
                    //{
                    //    mallRef = context.PreEntityImages["PreImage"].GetAttributeValue<EntityReference>("giulia_mall");
                    //}
                    //else
                    //{
                    //    throw new InvalidPluginExecutionException("Select a mall record!");
                    //}
                    //Entity mall = service.Retrieve("giulia_malls", mallRef.Id, new ColumnSet("giulia_name"));
                    //mallName = mall.GetAttributeValue<string>("giulia_name");

                    //if (floor.Attributes.Contains("giulia_floornumber"))
                    //{
                    //    floorNumber = floor.GetAttributeValue<int>("giulia_floornumber").ToString();
                    //}
                    //else if(context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"].Contains("giulia_floornumber"))
                    //{
                    //    floorNumber = context.PreEntityImages["PreImage"].GetAttributeValue<int>("giulia_floornumber").ToString();
                    //}
                    //else
                    //{
                    //    throw new InvalidPluginExecutionException("Insert the floor number!");
                    //}

                    //floor["giulia_name"] = $"{mallName} Floor{floorNumber}";

                    var generator = new FloorNameGenerator(context, service);
                    generator.Generate(floor);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Error in Generate Floor Name plugin: " + ex.Message, ex);
                }
            }

        }
    }
}
