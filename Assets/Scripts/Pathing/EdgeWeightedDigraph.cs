using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rougelikeberry.Pathing
{
    public class EdgeWeightedDigraph
    {
        private readonly LinkedList<DirectedEdge>[] m_Adjacent;

        /// <summary>
        /// The number of vertices in the edge-weighted digraph
        /// </summary>
        public int NumberOfVertices { get; private set; }
        /// <summary>
        /// The number of edges in the edge-weighted digraph
        /// </summary>
        public int NumberOfEdges { get; private set; }

        public EdgeWeightedDigraph(int vertices)
        {
            NumberOfVertices = vertices;
            NumberOfEdges = 0;
            m_Adjacent = new LinkedList<DirectedEdge>[NumberOfVertices];
            for (int v = 0; v < NumberOfVertices; v++)
            {
                m_Adjacent[v] = new LinkedList<DirectedEdge>();
            }
        }

        /// <summary>
        /// Adds the specified directed edge to the edge-weighted digraph
        /// </summary>
        /// <param name="edge">The DirectedEdge to add</param>
        public void AddEdge(DirectedEdge edge)
        {
            if(edge == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Directed Edge cannot be null"); }
            }
            m_Adjacent[edge.From].AddLast(edge);
        }

        /// <summary>
        /// Returns an IEnumerable of the DirectedEdges incident from the specified vertex
        /// </summary>
        /// <param name="vertex">The vertex to find incident DirectedEdges from</param>
        public IEnumerable<DirectedEdge> Adjacent(int vertex)
        {
            return m_Adjacent[vertex];
        }

        /// <summary>
        /// Returns an IEnumerable of all directed edges in the edge-weighted digraph
        /// </summary>
        public IEnumerable<DirectedEdge> Edges()
        {
            for (int v = 0; v < NumberOfVertices; v++)
            {
                foreach (DirectedEdge edge in m_Adjacent[v])
                {
                    yield return edge;
                }
            }
        }

        /// <summary>
        /// Returns the number of directed edges incident from the specified vertex
        /// This is known as the outdegree of the vertex
        /// </summary>
        /// <param name="vertex">The vertex to find find the outdegree of</param>
        public int OutDegree(int vertex)
        {
            return m_Adjacent[vertex].Count;
        }

        /// <summary>
        /// Returns a string that represents the current edge-weighted digraph
        /// </summary>
        public override string ToString()
        {
            var formattedString = new StringBuilder();
            formattedString.AppendFormat("{0} vertices, {1} edges {2}", NumberOfVertices, NumberOfEdges, Environment.NewLine);
            for (int v = 0; v < NumberOfVertices; v++)
            {
                formattedString.AppendFormat("{0}: ", v);
                foreach (DirectedEdge edge in m_Adjacent[v])
                {
                    formattedString.AppendFormat("{0} ", edge.To);
                }
                formattedString.AppendLine();
            }
            return formattedString.ToString();
        }
    }
}