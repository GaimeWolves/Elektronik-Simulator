using ESim.Electronics.Circuitry.Wireing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry.Components.Basic
{
    public class Ground : Component
    {
        public Ground()
        {
            inputs = new Node[1];
        }
    }
}
