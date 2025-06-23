using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MallSpace_Plugins.Floor.Business_Logic
{
    public class FloorNameGenerate
    {
        public string generateFloorName(string mallName, int floorNumber) 
        {
            return $"{mallName} Floor{floorNumber}";
        }

    }
}
