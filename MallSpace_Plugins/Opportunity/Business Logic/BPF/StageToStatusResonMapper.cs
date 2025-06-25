using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Business_Logic.BPF
{
    public class StageToStatusResonMapper
    {
        public OptionSetValue Map(string stageName)
        {
            switch (stageName)
            {
                case "Draft":
                    return new OptionSetValue(343530000);
                case "Negotiation":
                    return new OptionSetValue(343530001);
                case "Offer":
                    return new OptionSetValue(343530002);
                case "Contract":
                    return new OptionSetValue(343530003);
                default:
                    throw new InvalidPluginExecutionException($"Unknown stage: {stageName}");
            }
        }
    }
}
