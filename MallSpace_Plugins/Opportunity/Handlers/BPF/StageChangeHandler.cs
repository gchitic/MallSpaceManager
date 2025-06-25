using MallSpace_Plugins.Opportunity.Business_Logic.BPF;
using MallSpace_Plugins.Opportunity.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Opportunity.Handlers.BPF
{
    public class StageChangeHandler
    {
        //Services
        private readonly BpfService bpfService;
        private readonly OpportunityFieldService opportunityFieldService;

        private readonly StageToStatusResonMapper mapper;

        public StageChangeHandler( BpfService bpfService, OpportunityFieldService opportunityFieldService, 
            StageToStatusResonMapper mapper) 
        {
            //Services
            this.bpfService = bpfService;
            this.opportunityFieldService = opportunityFieldService;

            this.mapper = mapper;
        }

        public void Handle(Entity opportunityBPF)
        {
            //Skip if BPF is finish
            var stageRef = bpfService.getActiveStageId(opportunityBPF);
            if (stageRef == null)
                return; 

            string stageName = bpfService.getStageName(opportunityBPF);
            var opportunityRef = bpfService.getOpportunityReference(opportunityBPF);

            //Set the value of status reason field from opportunity entity
            var statusReason = mapper.Map(stageName);
            var opportunity = new Entity("giulia_opportunity", opportunityRef.Id);
            opportunityFieldService.setOpportunityStatusReason(opportunity, statusReason);

        }
    }
}
