using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Floor.Business_Logic
{
    public class FloorValidator
    {
        public bool isInRange(int floorNumber, int maxMallFloors)
        {
            return floorNumber <= maxMallFloors;
        }

        public bool isUnique(int floorNumber, List<int> existingFloorNumbers)
        {
            return !existingFloorNumbers.Contains(floorNumber);
        }

        public void ensureSpaceIsAvailable(decimal? totalSpace, decimal? occupiedSpace, 
            decimal? offeredSpace)
        {
            if((totalSpace - occupiedSpace) < offeredSpace) {
                throw new InvalidPluginExecutionException("No more space on this floor");
            }
        }
    }
}
