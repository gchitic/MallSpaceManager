using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Opportunity.Handlers
{
    internal class ReadOnlyFieldsHandler
    {
        public void protectReadOnlyFields(IPluginExecutionContext context, Entity opportunity)
        {
            if (opportunity.Contains("giulia_rentcost") && opportunity["giulia_rentcost"] != null)
            {
                throw new InvalidPluginExecutionException("The Rent Cost field is read-only and cannot be set manually.");
            }
        }
    }
}
