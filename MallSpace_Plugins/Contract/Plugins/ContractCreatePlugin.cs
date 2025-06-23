using MallSpace_Plugins.Contract.Handlers;
using MallSpace_Plugins.Floor.Business_Logic;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                try
                {
                    ContractCreateUpdateHandler handler = new ContractCreateUpdateHandler(occupiedSpaceCalculator);
                    handler.Handle(service, contract);
                }
                catch(InvalidPluginExecutionException ex)
                {
                    throw new InvalidPluginExecutionException("Error in ContrcatCreatePLugin: " + ex.Message);
                }
            }
        }
    }
}
