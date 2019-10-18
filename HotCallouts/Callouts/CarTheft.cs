using LCPD_First_Response.LCPDFR.Callouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotCallouts.Callouts
{
    [ExperimentMode(typeof(CarTheft), ElementType.Callout), 
    CalloutInfo("CarTheft", ECalloutProbability.High)]
    public class CarTheft : Callout
    {
        
    }
}
