using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Handlers.BPF
{
    public class StageChangeHandler
    {
        public void Handle(IOrganizationService service, Entity opportunityBPF)
        {
            // If activestageid is missing, it might be because the BPF is finished
            var stageRef = opportunityBPF.GetAttributeValue<EntityReference>("activestageid");
            if (stageRef == null)
            {
                // Optionally: You can skip logic or handle this final state separately
                return; // Do nothing when BPF is completed
            }

            // Get stage ID and resolve name
            var stageId = opportunityBPF.GetAttributeValue<EntityReference>("activestageid")?.Id ?? Guid.Empty;
            if (stageId == Guid.Empty)
                throw new InvalidPluginExecutionException("Active stage ID is missing.");

            string stageName = getStageName(service, stageId);

            //Retrive all attributes from bpf and get the bpf_giulia_opportunityid
            var opportunityRef = opportunityBPF.GetAttributeValue<EntityReference>("bpf_giulia_opportunityid");
            if (opportunityRef == null)
            {
                var fullBpf = service.Retrieve(opportunityBPF.LogicalName, opportunityBPF.Id, new ColumnSet(true));
                opportunityRef = fullBpf.GetAttributeValue<EntityReference>("bpf_giulia_opportunityid");

                if (opportunityRef == null)
                    throw new InvalidPluginExecutionException("Opportunity reference is still null after full BPF retrieval.");
            }


            //Set the value of status reason field from opportunity etity
            if (stageName == "Draft")
                updateOpportunityStatusReason(service, opportunityRef.Id, new OptionSetValue(343530000));
            else if (stageName == "Negotiation")
                updateOpportunityStatusReason(service, opportunityRef.Id, new OptionSetValue(343530001));
            else if (stageName == "Offer")
                updateOpportunityStatusReason(service, opportunityRef.Id, new OptionSetValue(343530002));
            else if (stageName == "Contract")
            {
                updateOpportunityStatusReason(service, opportunityRef.Id, new OptionSetValue(343530003));
            }
                
        }

        public string getStageName(IOrganizationService service, Guid stageId)
        {
            //Retrieve syntax: Retrieve(string entityName, Guid id, ColumnSet columnSet)
            var stage = service.Retrieve("processstage", stageId, new ColumnSet("stagename"));
            return stage.GetAttributeValue<string>("stagename");
        }

        public void updateOpportunityStatusReason(IOrganizationService service, Guid id, OptionSetValue value)
        {
            var opportunity = new Entity("giulia_opportunity", id);
            opportunity["giulia_statusreason"] = value;
            service.Update(opportunity);
        }

    }
}
