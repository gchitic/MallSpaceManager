using MallSpace_Plugins.Contract.Handlers;
using MallSpace_Plugins.Contract.Services;
using MallSpace_Plugins.Floor.Business_Logic;
using MallSpace_Plugins.Floor.Services;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Contract.Plugins
{
    public class ContractCreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)
                serviceProvider.GetService(typeof (IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity contract = (Entity)context.InputParameters["Target"];

                //Dependencies
                OccupiedSpaceCalculator occupiedSpaceCalculator = new OccupiedSpaceCalculator();
                FloorValidator floorValidator = new FloorValidator();

                //Services
                FloorDataService floorDataService = new FloorDataService(service);
                OpportunityFieldService opportunityFieldService = new OpportunityFieldService(service);
                ContractDataService contractDataService = new ContractDataService(service);

                try
                {
                    if(context.MessageName == "Create")
                    {
                        ContractCreateUpdateHandler handler = new ContractCreateUpdateHandler(occupiedSpaceCalculator, floorDataService, opportunityFieldService,
                            floorValidator, contractDataService);
                        handler.Handle(service, contract);
                    } 
                }
                catch(InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in ContrcatCreatePLugin: " + ex.Message);
                }
            }
        }
    }
}
