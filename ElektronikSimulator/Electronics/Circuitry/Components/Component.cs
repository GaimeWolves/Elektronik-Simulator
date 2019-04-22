using ESim.Electronics.Circuitry.Wireing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Circuitry.Components
{
    public abstract class Component
    {
        internal Node[] inputs;

        /// <summary>
        /// Connects the specified inputs of two components and combines both nodes if existent or creates one if none are connected.
        /// </summary>
        /// <param name="other">The other component to connect.</param>
        /// <param name="thisNodeIndex">The node to connect from this component.</param>
        /// <param name="otherNodeIndex">The node to connect from the other component.</param>
        /// <param name="difference">The removed / added node.</param>
        /// <returns>-1 if node destroyed; 1 if node created; 0 otherwise</returns>
        public int Connect(Component other, int thisNodeIndex, int otherNodeIndex)
        {
            if (inputs[thisNodeIndex] is null && other.inputs[otherNodeIndex] is null)
            {
                inputs[thisNodeIndex] = other.inputs[otherNodeIndex] = new Node(this, other);
                return 1;
            }
            else if (!(inputs[thisNodeIndex] is null) && !(other.inputs[otherNodeIndex] is null))
            {
                inputs[thisNodeIndex].ConnectComponents(other.inputs[otherNodeIndex].Components.ToArray());
                other.inputs[otherNodeIndex] = inputs[thisNodeIndex];
                return -1;
            }
            else
            {
                if (inputs[thisNodeIndex] is null)
                {
                    inputs[thisNodeIndex] = other.inputs[otherNodeIndex];
                    inputs[thisNodeIndex].ConnectComponents(this);
                }
                else
                {
                    other.inputs[otherNodeIndex] = inputs[thisNodeIndex];
                    inputs[thisNodeIndex].ConnectComponents(other);
                }
                return 0;
            }
        }

        public void Disconnect(Node node)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == node)
                {
                    inputs[i] = null;
                    return;
                }
            }
        }
    }
}
