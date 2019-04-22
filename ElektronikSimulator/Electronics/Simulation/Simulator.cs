using ESim.Electronics.Circuitry.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESim.Electronics.Simulation.MNA;
using ESim.Electronics.Circuitry.Wireing;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ESim.Electronics.Simulation
{
    public class Simulator
    {
        private List<Component> components;
        private double time;

        public Simulator(List<Component> circuit)
        {
            components = circuit;
        }

        public void Simulate()
        {
            double[] result = MNAHelper.Simulate(ref components);
        }
    }
}
