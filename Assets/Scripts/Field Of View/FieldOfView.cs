using Rougelikeberry.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Rougelikeberry.FoV
{
    public class FieldOfView
    {
        private enum Quadrant
        {
            NE = 1,
            SE = 2,
            SW = 3,
            NW = 4
        }

        private readonly IMap m_Map;
        private readonly HashSet<int> m_InFoV;

        public FieldOfView(IMap map)
        {
            m_Map = map;
            m_InFoV = new HashSet<int>();
        }

        internal FieldOfView(IMap map, HashSet<int> inFov)
        {
            m_Map = map;
            m_InFoV = inFov;
        }


        public FieldOfView Clone()
        {
            var infovCopy = new HashSet<int>();
            foreach (int i in m_InFoV)
            {
                infovCopy.Add(i);
            }
            return new FieldOfView(m_Map, infovCopy);
        }

        /// <summary>
        /// Check if the Cell is in the currently computed field-of-view
        /// Field-of-view must first be calculated by calling ComputeFov and/or AppendFov
        /// </summary>>
        /// <param name="x">X location of the Cell to check starting with 0 as the farthest left</param>
        /// <param name="y">Y location of the Cell to check, starting with 0 as the top</param>
        public bool IsInFoV(int x, int y)
        {
            return m_InFoV.Contains(m_Map.IndexFor(x, y));
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius.
        /// Any existing field-of-view calculations will be overwritten when calling this method.
        /// </summary>
        /// <param name="xOrigin">X location of the Cell to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Cell to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Cells in which the field-of-view extends from the origin Cell. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        public ReadOnlyCollection<IBlock> ComputeFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            ClearFov();
            return AppendFov(xOrigin, yOrigin, radius, lightWalls);
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters and appends it any existing field-of-view calculations.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius.
        /// </summary>
        /// <example>
        /// When a character is holding a light source in a large area that also has several other sources of light such as torches along the walls
        /// ComputeFov could first be called for the character and then AppendFov could be called for each torch to give us the final combined FOV given all the light sources
        /// </example>
        /// <param name="xOrigin">X location of the Cell to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Cell to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Cells in which the field-of-view extends from the origin Cell. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        public ReadOnlyCollection<IBlock> AppendFov(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            foreach (IBlock borderBlock in m_Map.GetBorderBlocksInSquare(xOrigin, yOrigin, radius))
            {
                foreach (IBlock block in m_Map.GetBlocksAlongLine(xOrigin, yOrigin, borderBlock.X, borderBlock.Y))
                {
                    if ((Math.Abs(block.X - xOrigin) + Math.Abs(block.Y - yOrigin)) > radius)
                    {
                        break;
                    }
                    if (block.IsTransparent)
                    {
                        m_InFoV.Add(m_Map.IndexFor(block));
                    }
                    else
                    {
                        if (lightWalls)
                        {
                            m_InFoV.Add(m_Map.IndexFor(block));
                        }
                        break;
                    }
                }
            }

            if (lightWalls)
            {
                // Post processing step created based on the algorithm at this website:
                // https://sites.google.com/site/jicenospam/visibilitydetermination
                foreach (IBlock block in m_Map.GetBlocksInSquare(xOrigin, yOrigin, radius))
                {
                    if (block.X > xOrigin)
                    {
                        if (block.Y > yOrigin)
                        {
                            PostProcessFovQuadrant(block.X, block.Y, Quadrant.SE);
                        }
                        else if (block.Y < yOrigin)
                        {
                            PostProcessFovQuadrant(block.X, block.Y, Quadrant.NE);
                        }
                    }
                    else if (block.X < xOrigin)
                    {
                        if (block.Y > yOrigin)
                        {
                            PostProcessFovQuadrant(block.X, block.Y, Quadrant.SW);
                        }
                        else if (block.Y < yOrigin)
                        {
                            PostProcessFovQuadrant(block.X, block.Y, Quadrant.NW);
                        }
                    }
                }
            }

            return BlocksInFov();
        }

        private ReadOnlyCollection<IBlock> BlocksInFov()
        {
            var blocks = new List<IBlock>();
            foreach (int index in m_InFoV)
            {
                blocks.Add(m_Map.BlockFor(index));
            }
            return new ReadOnlyCollection<IBlock>(blocks);
        }

        private void ClearFov()
        {
            m_InFoV.Clear();
        }

        private void PostProcessFovQuadrant(int x, int y, Quadrant quadrant)
        {
            int x1 = x;
            int y1 = y;
            int x2 = x;
            int y2 = y;
            switch (quadrant)
            {
                case Quadrant.NE:
                    {
                        y1 = y + 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SE:
                    {
                        y1 = y - 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SW:
                    {
                        y1 = y - 1;
                        x2 = x + 1;
                        break;
                    }
                case Quadrant.NW:
                    {
                        y1 = y + 1;
                        x2 = x + 1;
                        break;
                    }
            }
            if (!IsInFoV(x, y) && !m_Map.IsTransparent(x, y))
            {
                if ((m_Map.IsTransparent(x1, y1) && IsInFoV(x1, y1)) || (m_Map.IsTransparent(x2, y2) && IsInFoV(x2, y2))
                     || (m_Map.IsTransparent(x2, y1) && IsInFoV(x2, y1)))
                {
                    m_InFoV.Add(m_Map.IndexFor(x, y));
                }
            }
        }

    }
}