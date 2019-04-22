using ESim.Electronics.Circuitry.Components;
using ESim.Electronics.Circuitry.Components.Basic;
using ESim.Electronics.Circuitry.Wireing;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.Simulation.MNA
{
    /// <summary>
    /// A class containing all functions to do Modified Nodal Analysis.
    /// </summary>
    public static class MNAHelper
    {
        /// <summary>
        /// The types of components allowed in a linearized circuit.
        /// </summary>
        private static readonly Type[] LegalTypes = {
            typeof(Resistor),
            typeof(CurrentSource),
            typeof(Ground),
            typeof(VoltageSource)
        };

        /// <summary>
        /// Simulates the current linearized circuit state.
        /// </summary>
        /// <param name="components">The linearized circuit.</param>
        /// <returns>The state of each node (eg. voltage and current).</returns>
        public static double[] Simulate(ref List<Component> components)
        {
            List<Node> nodes = DetectGroundNodes(ref components);
            FilterComponents(components, out List<Resistor> resistors, out List<CurrentSource> iSources, out List<VoltageSource> vSources);

            DenseMatrix G = CreateConductanceMatrix(nodes, resistors);
            DenseMatrix V = CreateVoltageMatrix(nodes, vSources);
            DenseMatrix A = CreateComponentMatrix(V, G);

            DenseVector I = CreateCurrentVector(nodes, iSources);
            DenseVector E = CreateVoltageVector(vSources);
            DenseVector z = CreateSourceVector(I, E);

            //Solves for x: Ax = z
            return Solve(A, z);
        }

        /// <summary>
        /// Solves the MNA equation for the unknown currents and node potentials. (x): Ax = z
        /// </summary>
        /// <param name="A">The component matrix containing all resistances and voltage source potentials.</param>
        /// <param name="z">The source matrix containing the values for voltage/current sources.</param>
        /// <returns>A vector containing all unknown currents and node potentials.</returns>
        private static double[] Solve(DenseMatrix A, DenseVector z) => (z * A.Inverse()).AsArray();

        /// <summary>
        /// Creates the component matrix containing all resistances and voltage source connections.
        /// </summary>
        /// <param name="V">The matrix describing all voltage source connections.</param>
        /// <param name="G">The matrix describing all restistance values and connections.</param>
        /// <returns>The component matrix.</returns>
        private static DenseMatrix CreateComponentMatrix(DenseMatrix V, DenseMatrix G)
        {
            DenseMatrix A = new DenseMatrix(G.RowCount + V.RowCount);

            DenseMatrix Vt = V.Transpose() as DenseMatrix;

            for (int i = 0; i < G.Values.Length; i++)
                A[i % G.RowCount, i / G.RowCount] = G.Values[i];

            for (int i = 0; i < V.Values.Length; i++)
                A[i % V.RowCount + G.RowCount, i / V.RowCount] = V.Values[i];

            for (int i = 0; i < Vt.Values.Length; i++)
                A[i % Vt.RowCount, i / Vt.RowCount + G.ColumnCount] = Vt.Values[i];

            return A;
        }

        /// <summary>
        /// Creates a matrix describing all voltage source connections.
        /// </summary>
        /// <param name="nodes">A list of all nodes. (excluding ground nodes)</param>
        /// <param name="vSources">A list containing all voltage sources.</param>
        /// <returns>The matrix describing all voltage source connections.</returns>
        private static DenseMatrix CreateVoltageMatrix(List<Node> nodes, List<VoltageSource> vSources)
        {
            DenseMatrix V = new DenseMatrix(vSources.Count, nodes.Count);

            for(int i = 0; i < vSources.Count; i++)
            {
                if (nodes.Contains(vSources[i].inputs[0]))
                    V[i, nodes.IndexOf(vSources[i].inputs[0])] = -1;
                if (nodes.Contains(vSources[i].inputs[1]))
                    V[i, nodes.IndexOf(vSources[i].inputs[1])] = 1;
            }

            return V;
        }

        /// <summary>
        /// Creates a matrix describing all resistor values and connections.
        /// </summary>
        /// <param name="nodes">The list of nodes. (excluding ground nodes)</param>
        /// <param name="resistors">The list of resistors.</param>
        /// <returns>The matrix describing all resistor values and connections.</returns>
        private static DenseMatrix CreateConductanceMatrix(List<Node> nodes, List<Resistor> resistors)
        {
            DenseMatrix G = resistors is null ? null : new DenseMatrix(nodes.Count);
            
            for(int i = 0; i < nodes.Count; i++)
            {
                double val = 0;
                foreach (Component c in nodes[i].Components)
                    if (c.GetType() == typeof(Resistor))
                        val += (c as Resistor).G;
                G[i, i] = val;
            }

            foreach(Resistor r in resistors)
            {
                if (!r.inputs.Any(n => n.Components.Any(c => c.GetType() == typeof(Ground))))
                {
                    G[nodes.IndexOf(r.inputs[0]), nodes.IndexOf(r.inputs[1])] = -r.G;
                    G[nodes.IndexOf(r.inputs[1]), nodes.IndexOf(r.inputs[0])] = -r.G;
                }
            }

            return G;
        }

        /// <summary>
        /// Creates a source vector containing all current/voltage source values.
        /// </summary>
        /// <param name="I">The vector containing all current source values.</param>
        /// <param name="E">The cevtor containing all voltage source values.</param>
        /// <returns>The vector containing all current/voltage source values.</returns>
        private static DenseVector CreateSourceVector(DenseVector I, DenseVector E)
        {
            List<double> vals = new List<double>();
            if (!(I is null)) vals.AddRange(I.Values);
            if (!(E is null)) vals.AddRange(E.Values);
            return vals.Count == 0 ? null : new DenseVector(vals.ToArray());
        }

        /// <summary>
        /// Create a vector containing all current source values.
        /// </summary>
        /// <param name="nodes">The list of nodes.</param>
        /// <param name="iSources">The list of current sources.</param>
        /// <returns>The vector containing all current source values.</returns>
        private static DenseVector CreateCurrentVector(List<Node> nodes, List<CurrentSource> iSources)
        {
            DenseVector I = new DenseVector(nodes.Count);
            if (!(iSources is null))
            {
                foreach (CurrentSource c in iSources)
                {
                    if (nodes.Contains(c.inputs[1]))
                        I[nodes.IndexOf(c.inputs[1])] = c.Current;
                }
            }
            return I;
        }

        /// <summary>
        /// Creates a vector containing all voltage source values.
        /// </summary>
        /// <param name="vSources">The list of voltage sources.</param>
        /// <returns>The vector containing all voltage source values.</returns>
        private static DenseVector CreateVoltageVector(List<VoltageSource> vSources) 
            => vSources is null ? null : new DenseVector(vSources.Select(c => c.Voltage).ToArray());

        /// <summary>
        /// Creates the node array (omitting ground nodes) and removes all grounds.
        /// </summary>
        /// <param name="circuit">The current circuit.</param>
        /// <returns>A list of nodes.</returns>
        private static List<Node> DetectGroundNodes(ref List<Component> circuit)
        {
            List<Node> nodes = new List<Node>();

            foreach (Component c in circuit)
            {
                if (!LegalTypes.Contains(c.GetType()))
                    throw new ArgumentException("Cannot complete MNA, circuit is non-linear.");

                foreach (Node n in c.inputs)
                    //Omit Ground nodes
                    if (!nodes.Contains(n) && !n.Components.Any(i => i.GetType() == typeof(Ground)))
                        nodes.Add(n);
            }

            //Remove Grounds
            circuit.RemoveAll(i => i.GetType() == typeof(Ground));

            return nodes;
        }

        /// <summary>
        /// Splits the list of components into lists of resistors, current- and voltage sources.
        /// </summary>
        /// <param name="circuit">The list of components.</param>
        /// <param name="resistors">A list of resistors.</param>
        /// <param name="iSources">A list of current sources.</param>
        /// <param name="vSources">A list of voltage sources.</param>
        private static void FilterComponents(List<Component> circuit, out List<Resistor> resistors, out List<CurrentSource> iSources, out List<VoltageSource> vSources)
        {
            resistors = circuit.Any(c => c.GetType() == typeof(Resistor)) ? circuit.Where(c => c.GetType() == typeof(Resistor)).Cast<Resistor>().ToList() : null;
            vSources = circuit.Any(c => c.GetType() == typeof(VoltageSource)) ? circuit.Where(c => c.GetType() == typeof(VoltageSource)).Cast<VoltageSource>().ToList() : null;
            iSources = circuit.Any(c => c.GetType() == typeof(CurrentSource)) ? circuit.Where(c => c.GetType() == typeof(CurrentSource)).Cast<CurrentSource>().ToList() : null;
        } 
    }
}
