using ESim.Electronics.Circuitry.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry.Wireing
{
    public class Node
    {
        public double Potential { get; set; }
        public List<Component> Components { get; private set; }

        public Node(params Component[] connected)
        {
            Components = new List<Component>();
            Components.AddRange(connected);
        }

        public void ConnectComponents(params Component[] connected) => Components.AddRange(connected);
        public void DisconnectComponents(params Component[] connected)
        {
            foreach (Component c in connected)
            {
                Components.Remove(c);
                c.Disconnect(this);
            }
        }

    }
}
