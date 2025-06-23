using MallSpace_Plugins.Floor.Business_Logic;
using Microsoft.Xrm.Sdk;
using System;

namespace MallSpace_Plugins.Floor.Handlers
{
    public class FloorCreateHandler
    {
        private readonly IOrganizationService service;
        public FloorCreateHandler(IOrganizationService service)
        {
            this.service = service;
        }

        public void Handle(Entity floor, Entity preimage, FloorNameGenerator floorNameGenerator)
        {
            string name = floorNameGenerator.generateFloorName(floor, preimage);
            floor["giulia_name"] = name;
        }
    }
}
