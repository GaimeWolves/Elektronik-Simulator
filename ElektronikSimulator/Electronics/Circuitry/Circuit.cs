using ESim.Electronics.Circuitry.Components;
using ESim.Electronics.Circuitry.Components.Basic;
using ESim.Electronics.Circuitry.Wireing;
using ESim.Electronics.Simulation;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry
{
    public class Circuit
    {
        private List<Component> components;

        public Circuit()
        {
            components = new List<Component>();
        }

        public void Draw(GraphicsDevice graphics)
        {

        }
    }
}
