﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Pathing
{
    public class DijkstraShortestPath
    {
        private readonly double[] _distanceTo;
        private readonly DirectedEdge[] _edgeTo;
        private readonly IndexMinPriorityQueue<double> _priorityQueue;

        /// <summary>
        /// Computes a shortest paths tree from the specified sourceVertex to every other vertex in the edge-weighted directed graph
        /// </summary>
        /// <param name="graph">The edge-weighted directed graph</param>
        /// <param name="sourceVertex">The source vertex to compute the shortest paths tree from</param>
        public DijkstraShortestPath(EdgeWeightedDigraph graph, int sourceVertex) : this(graph, sourceVertex, null)
        {

        }

        private DijkstraShortestPath(EdgeWeightedDigraph graph, int sourceVertex, int? destinationVertex)
        {
            if (graph == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("EdgeWeightedDigraph cannot be null"); }
            }

            foreach (DirectedEdge edge in graph.Edges())
            {
                if (edge.Weight < 0)
                {
                    if (LogFilter.logError) { Debug.LogErrorFormat("Edge: '{0}' has negative weight", edge); }
                }
            }

            _distanceTo = new double[graph.NumberOfVertices];
            _edgeTo = new DirectedEdge[graph.NumberOfVertices];
            for (int v = 0; v < graph.NumberOfVertices; v++)
            {
                _distanceTo[v] = Double.PositiveInfinity;
            }
            _distanceTo[sourceVertex] = 0.0;

            _priorityQueue = new IndexMinPriorityQueue<double>(graph.NumberOfVertices);
            _priorityQueue.Insert(sourceVertex, _distanceTo[sourceVertex]);
            while (!_priorityQueue.IsEmpty())
            {
                int v = _priorityQueue.DeleteMin();

                if (destinationVertex.HasValue && v == destinationVertex.Value)
                {
                    return;
                }

                foreach (DirectedEdge edge in graph.Adjacent(v))
                {
                    Relax(edge);
                }
            }
            Check(graph, sourceVertex);
        }

        /// <summary>
        /// Returns an IEnumerable of DirectedEdges representing a shortest path from the specified sourceVertex to the specified destinationVertex
        /// This is more efficent than creating a new DijkstraShorestPath instance and calling PathTo( destinationVertex ) when we only
        /// want a single path from Source to Destination and don't want many paths from the source to multiple different destinations.
        /// </summary>
        /// <param name="graph">The edge-weighted directed graph</param>
        /// <param name="sourceVertex">The source vertext to find a shortest path from</param>
        /// <param name="destinationVertex">The destination vertex to find a shortest path to</param>
        public static IEnumerable<DirectedEdge> FindPath(EdgeWeightedDigraph graph, int sourceVertex, int destinationVertex)
        {
            var dijkstraShortestPath = new DijkstraShortestPath(graph, sourceVertex, destinationVertex);
            return dijkstraShortestPath.PathTo(destinationVertex);
        }

        private void Relax(DirectedEdge edge)
        {
            int v = edge.From;
            int w = edge.To;
            if (_distanceTo[w] > _distanceTo[v] + edge.Weight)
            {
                _distanceTo[w] = _distanceTo[v] + edge.Weight;
                _edgeTo[w] = edge;
                if (_priorityQueue.Contains(w))
                {
                    _priorityQueue.DecreaseKey(w, _distanceTo[w]);
                }
                else
                {
                    _priorityQueue.Insert(w, _distanceTo[w]);
                }
            }
        }

        /// <summary>
        /// Returns the length of a shortest path from the sourceVertex to the specified destinationVertex
        /// </summary>
        /// <param name="destinationVertex">The destination vertex to find a shortest path to</param>
        public double DistanceTo(int destinationVertex)
        {
            return _distanceTo[destinationVertex];
        }

        /// <summary>
        /// Is there a path from the sourceVertex to the specified destinationVertex?
        /// </summary>
        /// <param name="destinationVertex">The destination vertex to see if there is a path to</param>
        public bool HasPathTo(int destinationVertex)
        {
            return _distanceTo[destinationVertex] < double.PositiveInfinity;
        }

        /// <summary>
        /// Returns an IEnumerable of DirectedEdges representing a shortest path from the sourceVertex to the specified destinationVertex
        /// </summary>
        /// <param name="destinationVertex">The destination vertex to find a shortest path to</param>
        public IEnumerable<DirectedEdge> PathTo(int destinationVertex)
        {
            if (!HasPathTo(destinationVertex))
            {
                return null;
            }
            var path = new Stack<DirectedEdge>();
            for (DirectedEdge edge = _edgeTo[destinationVertex]; edge != null; edge = _edgeTo[edge.From])
            {
                path.Push(edge);
            }
            return path;
        }

        // TODO: This method should be private and should be called from the bottom of the constructor
        /// <summary>
        /// check optimality conditions:
        /// </summary>
        /// <param name="graph">The edge-weighted directed graph</param>
        /// <param name="sourceVertex">The source vertex to check optimality conditions from</param>
        private bool Check(EdgeWeightedDigraph graph, int sourceVertex)
        {
            if (graph == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("EdgeWeightedDigraph cannot be null"); }
            }

            if (_distanceTo[sourceVertex] != 0.0 || _edgeTo[sourceVertex] != null)
            {
                return false;
            }
            for (int v = 0; v < graph.NumberOfVertices; v++)
            {
                if (v == sourceVertex)
                {
                    continue;
                }
                if (_edgeTo[v] == null && _distanceTo[v] != double.PositiveInfinity)
                {
                    return false;
                }
            }
            for (int v = 0; v < graph.NumberOfVertices; v++)
            {
                foreach (DirectedEdge edge in graph.Adjacent(v))
                {
                    int w = edge.To;
                    if (_distanceTo[v] + edge.Weight < _distanceTo[w])
                    {
                        return false;
                    }
                }
            }
            for (int w = 0; w < graph.NumberOfVertices; w++)
            {
                if (_edgeTo[w] == null)
                {
                    continue;
                }
                DirectedEdge edge = _edgeTo[w];
                int v = edge.From;
                if (w != edge.To)
                {
                    return false;
                }
                if (_distanceTo[v] + edge.Weight != _distanceTo[w])
                {
                    return false;
                }
            }
            return true;
        }
    }
}