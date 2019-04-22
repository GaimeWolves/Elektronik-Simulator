using ESim.Electronics.Circuitry.Wireing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry.Components.Basic
{
    public class CurrentSource : Component
    {
        public double Current { get; set; }

        public CurrentSource(double current)
        {
            Current = current;
            inputs = new Node[2];
        }
    }
}
