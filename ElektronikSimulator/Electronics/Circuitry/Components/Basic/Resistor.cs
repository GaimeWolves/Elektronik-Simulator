using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESim.Electronics.Circuitry.Wireing;

namespace ESim.Electronics.Circuitry.Components.Basic
{
    public class Resistor : Component
    {
        private double resistance;

        public double R
        {
            get => resistance;
            set
            {
                if (value > 0)
                    resistance = value;
            }
        }

        public double G
        {
            get => 1 / resistance;
            set
            {
                if (value > 0)
                    resistance = 1 / value;
            }
        }

        public Resistor(double resistance)
        {
            this.resistance = resistance;
            inputs = new Node[2];
        }
    }
}
