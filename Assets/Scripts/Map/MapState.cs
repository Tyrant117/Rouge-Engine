using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Map
{
    public class MapState
    {
        [Flags]
        public enum BlockProperties
        {
            /// <summary>
            /// Not set
            /// </summary>
            None = 0,
            /// <summary>
            /// A character could normally walk across the Cell without difficulty
            /// </summary>
            Walkable = 1,
            /// <summary>
            /// There is a clear line-of-sight through this Cell
            /// </summary>
            Transparent = 2,
            /// <summary>
            /// The Cell is in the currently observable field-of-view
            /// </summary>
            Visible = 4,
            /// <summary>
            /// The Cell has been in the field-of-view in the player at some point during the game
            /// </summary>
            Explored = 8
        }

        /// <summary>
        /// How many Cells wide the Map is
        /// </summary>
        public int Width
        {
            get; set;
        }

        /// <summary>
        /// How many Cells tall the Map is
        /// </summary>
        public int Height
        {
            get; set;
        }

        /// <summary>
        /// An array of the Flags Enumeration of CellProperties for each Cell in the Map.
        /// The index of the array corresponds to the location of the Cell within the Map using the forumla: index = ( y * Width ) + x
        /// </summary>
        public BlockProperties[] Blocks
        {
            get; set;
        }

    }
}