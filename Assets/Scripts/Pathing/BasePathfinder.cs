using Rougelikeberry.Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rougelikeberry.Pathing
{
    public class BasePathfinder : IPathFinder
    {
        private readonly EdgeWeightedDigraph _graph;
        private readonly IMap _map;

        /// <summary>
        /// Constructs a new PathFinder instance for the specified Map that will not consider diagonal movements to be valid.
        /// </summary>
        /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
        public BasePathfinder(IMap map)
        {
            if(map == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Map is null"); }
            }

            _map = map;
            _graph = new EdgeWeightedDigraph(_map.Width * _map.Height);

            foreach (IBlock block in _map.GetAllBlocks())
            {
                if (block.IsWalkable)
                {
                    int v = IndexFor(block);
                    foreach (IBlock neighbor in _map.GetBorderBlocksInDiamond(block.X, block.Y, 1))
                    {
                        if (neighbor.IsWalkable)
                        {
                            int w = IndexFor(neighbor);
                            _graph.AddEdge(new DirectedEdge(v, w, 1.0));
                            _graph.AddEdge(new DirectedEdge(w, v, 1.0));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a new PathFinder instance for the specified Map that will consider diagonal movement by using the specified diagonalCost
        /// </summary>
        /// <param name="map">The Map that this PathFinder instance will run shortest path algorithms on</param>
        /// <param name="diagonalCost">
        /// The cost of diagonal movement compared to horizontal or vertical movement. 
        /// Use 1.0 if you want the same cost for all movements.
        /// On a standard cartesian map, it should be sqrt(2) (1.41)
        /// </param>
        public BasePathfinder(IMap map, double diagonalCost)
        {
            if (map == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Map is null"); }
            }

            _map = map;
            _graph = new EdgeWeightedDigraph(_map.Width * _map.Height);
            foreach (IBlock block in _map.GetAllBlocks())
            {
                if (block.IsWalkable)
                {
                    int v = IndexFor(block);
                    foreach (IBlock neighbor in _map.GetBorderBlocksInSquare(block.X, block.Y, 1))
                    {
                        if (neighbor.IsWalkable)
                        {
                            int w = IndexFor(neighbor);
                            if (neighbor.X != block.X && neighbor.Y != block.Y)
                            {
                                _graph.AddEdge(new DirectedEdge(v, w, diagonalCost));
                                _graph.AddEdge(new DirectedEdge(w, v, diagonalCost));
                            }
                            else
                            {
                                _graph.AddEdge(new DirectedEdge(v, w, 1.0));
                                _graph.AddEdge(new DirectedEdge(w, v, 1.0));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a shortest Path containing a list of Blocks from a specified source Block to a destination Block
        /// </summary>
        /// <param name="start">The Block which is at the start of the path</param>
        /// <param name="end">The Block which is at the end of the path</param>
        public Path ShortestPath(IBlock start, IBlock end)
        {
            Path shortestPath = TryFindShortestPath(start, end);

            if (shortestPath == null)
            {
                if (LogFilter.logWarn) { Debug.LogWarningFormat("Path from({ 0}, {1}) to({2}, {3}) not found", start.X, start.Y, end.X, end.Y); }
            }

            return shortestPath;
        }

        /// <summary>
        /// Returns a shortest Path containing a list of Blocks from a specified source Block to a destination Block
        /// </summary>
        /// <param name="start">The Block which is at the start of the path</param>
        /// <param name="end">The Block which is at the end of the path</param>
        public Path TryFindShortestPath(IBlock start, IBlock end)
        {
            if (start == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Start is null"); }
            }

            if (end == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("End is null"); }
            }

            var blocks = ShortestPathBlocks(start, end).ToList();
            if (blocks[0] == null)
            {
                return null;
            }
            return new Path(blocks);
        }

        private IEnumerable<IBlock> ShortestPathBlocks(IBlock start, IBlock end)
        {
            yield return null;
            IEnumerable<DirectedEdge> path = DijkstraShortestPath.FindPath(_graph, IndexFor(start), IndexFor(end));
            if (path == null)
            {
                yield return null;
            }
            else
            {
                yield return start;
                foreach (DirectedEdge edge in path)
                {
                    yield return BlockFor(edge.To);
                }
            }
        }

        private int IndexFor(IBlock block)
        {
            return (block.Y * _map.Width) + block.X;
        }

        private IBlock BlockFor(int index)
        {
            int x = index % _map.Width;
            int y = index / _map.Width;

            return _map.GetBlock(x, y);
        }
    }
}