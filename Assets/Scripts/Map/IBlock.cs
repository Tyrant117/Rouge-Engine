using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Map
{
    public interface IBlock : IEquatable<IBlock>
    {
        /// <summary>
        /// Gets the X location of the Cell starting with 0 as the farthest left
        /// </summary>
        int X { get; }

        /// <summary>
        /// Y location of the Cell starting with 0 as the top
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Get the transparency of the Cell for line of sight calculations.
        /// </summary>
        bool IsTransparent { get; set; }

        /// <summary>
        /// Get the walkability of the Cell. 
        /// <para/>Only walkable cells are pathed.
        /// </summary>
        bool IsWalkable { get; set; }

        /// <summary>
        /// Check if the Cell is in the currently computed field-of-view.
        /// <para/>Field-of-view must first be calculated.
        /// </summary>
        bool IsInFov { get; }

        /// <summary>
        /// Check if the Cell is flagged as ever having been explored by the player. 
        /// <para/>This allows the player to render cells not currently in the FOV, differently from cells never seen before.
        /// </summary>
        bool IsExplored { get; set; }
    }
}