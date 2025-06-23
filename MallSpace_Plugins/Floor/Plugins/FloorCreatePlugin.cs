using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Handlers;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Floor.Plugins
{
    public  class FloorCreatePlugin : IPlugin
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
                var preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;

                //Dependencies
                FloorNameGenerator floorNameGenerator = new FloorNameGenerator(service);

                try
                {
                    var handler = new FloorCreateHandler(service);
                    handler.Handle(floor, preImage, floorNameGenerator);
                    if(context.MessageName == "Create")
                    {
                        floor["giulia_occupiedspace"] = floorNameGenerator.initializeOccupiedSpace();
                    }
                    
                } 
                catch(InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in FloorCreatePlugin: " + ex.Message);
                }
            }
        }
    }
}
