using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Services
{
    public class BpfService
    {
        private readonly IOrganizationService service;

        public BpfService(IOrganizationService service)
        {
            this.service = service;
        }

        public EntityReference getActiveStageId(Entity opportunityBPF)
        {
            return opportunityBPF.GetAttributeValue<EntityReference>("activestageid");
        }

        public Guid getStageId(Entity opportunityBPF)
        {
            var activeStage = getActiveStageId(opportunityBPF);
            return activeStage?.Id ?? Guid.Empty;
        }

        public string getStageName(Entity opportunityBPF)
        {
            var stageId = getStageId(opportunityBPF);
            var stage = service.Retrieve("processstage", stageId, new ColumnSet("stagename"));
            return stage.GetAttributeValue<string>("stagename");
        }

        public EntityReference getOpportunityReference(Entity opportunityBPF)
        {
            var opportunityRef = opportunityBPF.GetAttributeValue<EntityReference>("bpf_giulia_opportunityid");
            if (opportunityRef == null)
            {
                var fullBpf = service.Retrieve(opportunityBPF.LogicalName, opportunityBPF.Id, new ColumnSet(true));
                opportunityRef = fullBpf.GetAttributeValue<EntityReference>("bpf_giulia_opportunityid");

                if (opportunityRef == null)
                    throw new InvalidPluginExecutionException("Opportunity reference is still null after full BPF retrieval.");
            }

            return opportunityRef;
        }
    }
}
