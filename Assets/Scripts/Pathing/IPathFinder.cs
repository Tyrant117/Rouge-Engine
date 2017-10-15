using Rougelikeberry.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Pathing
{
    public interface IPathFinder
    {
        Path ShortestPath(IBlock start, IBlock end);
        Path TryFindShortestPath(IBlock start, IBlock end);
    }
}