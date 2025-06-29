﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Business_Logic
{
    public class ReadOnlyFieldRules
    {
        public void Validate(Entity opportunity, IPluginExecutionContext context)
        {
            //if its original opperation and is not a child plugin
            if (context.Depth != 1)
                return;

            //if field is present in Target
            if (opportunity.Attributes.Contains("giulia_rentcost"))
            {
                throw new InvalidPluginExecutionException("The Rent Cost field is read-only and cannot be set manually.");
            }
        }
    }
}
