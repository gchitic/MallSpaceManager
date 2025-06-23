using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Microsoft.Xrm.Sdk;

namespace MallSpace_Plugins.Floor.Business_Logic
{
    public class OccupiedSpaceCalculator
    {
        public decimal? calculateOccupiedSpace(decimal? offeredSpace, decimal? occupiedSpace)
        {
            return occupiedSpace + offeredSpace;
        }
    }
}
