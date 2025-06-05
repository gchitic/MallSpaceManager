using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace.Plugins.Opportunity.Handlers
{
    public class ReadOnlyFieldsHandler
    {
        public void protectReadOnlyFields(IPluginExecutionContext context, Entity opportunity)
        {
            try 
            {
                if (opportunity.Contains("giulia_rentcost"))
                {
                    throw new InvalidPluginExecutionException("The Rent Cost field is read-only and cannot be set manually.");
                }             
            }
            catch(InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("Error: " + ex.Message, ex);
            }
            
        }
    }
}
