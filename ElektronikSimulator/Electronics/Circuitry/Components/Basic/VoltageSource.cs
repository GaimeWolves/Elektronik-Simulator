using ESim.Electronics.Circuitry.Wireing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry.Components.Basic
{
    public class VoltageSource : Component
    {
        public double Voltage { get; set; }

        public VoltageSource(double voltage)
        {
            Voltage = voltage;
            inputs = new Node[2];
        }
    }
}
