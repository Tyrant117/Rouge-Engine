using Rougelikeberry.FoV;
using Rougelikeberry.Map.Creation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

namespace Rougelikeberry.Map
{
    public class Map : IMap
    {
        /// <summary>
        /// Width of Map, 0 through Final Value.
        /// </summary>
        public int Width
        {
            get; private set;
        }
        /// <summary>
        /// Height of Map, 0 through Final Value.
        /// </summary>
        public int Height
        {
            get; private set;
        }

        private FieldOfView _fieldOfView;

        private Block[,] _Blocks;

        public Map()
        {

        }

        public Map(int width, int height)
        {
            Initialize(width, height);
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            _Blocks = new Block[width, height];
            _fieldOfView = new FieldOfView(this);
        }

        public bool IsExplored(int x, int y)
        {
            return _Blocks[x, y].IsExplored;
        }
        public bool IsTransparent(int x, int y)
        {
            return _Blocks[x, y].IsTransparent;
        }
        public bool IsWalkable(int x, int y)
        {
            return _Blocks[x, y].IsWalkable;
        }
        public bool IsInFoV(int x, int y)
        {
            return _fieldOfView.IsInFoV(x, y);
        }

        public void SetBlockProperties(int x, int y, bool isTransparent, bool isWalkable, bool isExplored)
        {
            _Blocks[x, y].IsTransparent = isTransparent;
            _Blocks[x, y].IsWalkable = isWalkable;
            _Blocks[x, y].IsExplored = isExplored;
        }

        /// <summary>
        /// Sets the properties of all Blocks in the Map to be transparent and walkable
        /// </summary>
        public void Clear()
        {
            Clear(true, true);
        }
        /// <summary>
        /// Sets the properties of all Blocks in the Map to the specified values
        /// </summary>
        /// <param name="isTransparent">Optional parameter defaults to true if not provided. True if line-of-sight is not blocked by this Block. False otherwise</param>
        /// <param name="isWalkable">Optional parameter defaults to true if not provided. True if a character could walk across the Block normally. False otherwise</param>
        public void Clear(bool isTransparent, bool isWalkable)
        {
            foreach (IBlock block in GetAllBlocks())
            {
                SetBlockProperties(block.X, block.Y, isTransparent, isWalkable, false);
            }
        }

        /// <summary>
        /// Create and return a deep copy of an existing Map
        /// </summary>
        /// <returns>IMap deep copy of the original Map</returns>
        public IMap Clone()
        {
            var map = new Map(Width, Height);
            foreach (IBlock Block in GetAllBlocks())
            {
                map.SetBlockProperties(Block.X, Block.Y, Block.IsTransparent, Block.IsWalkable, Block.IsExplored);
            }
            return map;
        }

        /// <summary>
        /// Copies the Block properties of a smaller source Map into this destination Map at location (0,0)
        /// </summary>
        /// <param name="sourceMap">An IMap which must be of smaller size and able to fit in this destination Map at the specified location</param>
        public void Copy(IMap source)
        {
            Copy(source, 0, 0);
        }
        /// <summary>
        /// Copies the Block properties of a smaller source Map into this destination Map at the specified location
        /// </summary>
        /// <param name="sourceMap">An IMap which must be of smaller size and able to fit in this destination Map at the specified location</param>
        /// <param name="left">Optional parameter defaults to 0 if not provided. X location of the Block to start copying parameters to, starting with 0 as the farthest left</param>
        /// <param name="top">Optional parameter defaults to 0 if not provided. Y location of the Block to start copying parameters to, starting with 0 as the top</param>
        /// <exception cref="ArgumentNullException">Thrown on null source map</exception>
        /// <exception cref="ArgumentException">Thrown on invalid source map dimensions</exception>
        public void Copy(IMap source, int left, int top)
        {
            if (source == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Source map cannot be null"); }
            }

            if (source.Width + left > Width)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Source map 'width' + 'left' cannot be larger than the destination map width"); }
            }
            if (source.Height + top > Height)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Source map 'height' + 'top' cannot be larger than the destination map height"); }
            }
            foreach (IBlock block in source.GetAllBlocks())
            {
                SetBlockProperties(block.X + left, block.Y + top, block.IsTransparent, block.IsWalkable, block.IsExplored);
            }
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Block with a given light radius.
        /// Any existing field-of-view calculations will be overwritten when calling this method.
        /// </summary>
        /// <param name="xOrigin">X location of the Block to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Block to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Blocks in which the field-of-view extends from the origin Block. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        /// <returns>List of Blocks representing what is observable in the Map based on the specified parameters</returns>
        public ReadOnlyCollection<IBlock> ComputeFoV(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            return _fieldOfView.ComputeFov(xOrigin, yOrigin, radius, lightWalls);
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters and appends it any existing field-of-view calculations.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Block with a given light radius.
        /// </summary>
        /// <example>
        /// When a character is holding a light source in a large area that also has several other sources of light such as torches along the walls
        /// ComputeFov could first be called for the character and then AppendFov could be called for each torch to give us the final combined FOV given all the light sources
        /// </example>
        /// <param name="xOrigin">X location of the Block to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Block to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Blocks in which the field-of-view extends from the origin Block. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        /// <returns>List of Blocks representing what is observable in the Map based on the specified parameters</returns>
        public ReadOnlyCollection<IBlock> AppendFoV(int xOrigin, int yOrigin, int radius, bool lightWalls)
        {
            return _fieldOfView.AppendFov(xOrigin, yOrigin, radius, lightWalls);
        }

        /// <summary>
        /// Get an IEnumerable of all blocks in the map.
        /// </summary>
        public IEnumerable<IBlock> GetAllBlocks()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return GetBlock(x, y);
                }
            }
        }

        /// <summary>
        /// Get an IEnumerable of Blocks in a line from the Origin Block to the Destination Block
        /// The resulting IEnumerable includes the Origin and Destination Blocks
        /// Uses Bresenham's line algorithm to determine which Blocks are in the closest approximation to a straight line between the two Blocks
        /// </summary>
        /// <param name="xOrigin">X location of the Origin Block at the start of the line with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Origin Block at the start of the line with 0 as the top</param>
        /// <param name="xDestination">X location of the Destination Block at the end of the line with 0 as the farthest left</param>
        /// <param name="yDestination">Y location of the Destination Block at the end of the line with 0 as the top</param>
        public IEnumerable<IBlock> GetBlocksAlongLine(int xOrigin, int yOrigin, int xEnd, int yEnd)
        {
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xEnd = ClampX(xEnd);
            yEnd = ClampY(yEnd);

            int dx = Math.Abs(xEnd - xOrigin);
            int dy = Math.Abs(yEnd - yOrigin);

            int sx = xOrigin < xEnd ? 1 : -1;
            int sy = yOrigin < yEnd ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return GetBlock(xOrigin, yOrigin);
                if (xOrigin == xEnd && yOrigin == yEnd)
                {
                    break;
                }
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        private int ClampX(int x)
        {
            return (x < 0) ? 0 : (x > Width - 1) ? Width - 1 : x;
        }

        private int ClampY(int y)
        {
            return (y < 0) ? 0 : (y > Height - 1) ? Height - 1 : y;
        }

        public IEnumerable<IBlock> GetBlocksInCircle(int xCenter, int yCenter, int radius)
        {
            var discovered = new HashSet<int>();

            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                foreach (IBlock Block in GetBlocksAlongLine(xCenter + x, yCenter + y, xCenter - x, yCenter + y))
                {
                    if (AddToHashSet(discovered, Block))
                    {
                        yield return Block;
                    }
                }
                foreach (IBlock Block in GetBlocksAlongLine(xCenter - x, yCenter - y, xCenter + x, yCenter - y))
                {
                    if (AddToHashSet(discovered, Block))
                    {
                        yield return Block;
                    }
                }
                foreach (IBlock Block in GetBlocksAlongLine(xCenter + y, yCenter + x, xCenter - y, yCenter + x))
                {
                    if (AddToHashSet(discovered, Block))
                    {
                        yield return Block;
                    }
                }
                foreach (IBlock Block in GetBlocksAlongLine(xCenter + y, yCenter - x, xCenter - y, yCenter - x))
                {
                    if (AddToHashSet(discovered, Block))
                    {
                        yield return Block;
                    }
                }

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);
        }


        /// <summary>
        /// Get an IEnumerable of Blocks in a diamond (Rhombus) shape around the center Block up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Block with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Block with 0 as the top</param>
        /// <param name="distance">The number of Blocks to get in a distance from the center Block</param>
        /// <returns>IEnumerable of Blocks in a diamond (Rhombus) shape around the center Block</returns>
        public IEnumerable<IBlock> GetBlocksInDiamond(int xCenter, int yCenter, int distance)
        {
            var discovered = new HashSet<int>();

            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(Width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(Height - 1, yCenter + distance);

            for (int i = 0; i <= distance; i++)
            {
                for (int j = distance; j >= 0 + i; j--)
                {
                    IBlock Block;
                    if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - j), out Block))
                    {
                        yield return Block;
                    }
                    if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + j), out Block))
                    {
                        yield return Block;
                    }
                    if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - j), out Block))
                    {
                        yield return Block;
                    }
                    if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + j), out Block))
                    {
                        yield return Block;
                    }
                }
            }
        }

        /// <summary>
        /// Get an IEnumerable of Blocks in a square area around the center Block up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Block with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Block with 0 as the top</param>
        /// <param name="distance">The number of Blocks to get in each direction from the center Block</param>
        /// <returns>IEnumerable of Blocks in a square area around the center Block</returns>
        public IEnumerable<IBlock> GetBlocksInSquare(int xCenter, int yCenter, int distance)
        {
            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(Width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(Height - 1, yCenter + distance);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    yield return GetBlock(x, y);
                }
            }
        }

        /// <summary>
        /// Get an IEnumerable of outermost border Blocks in a circle around the center Block up to the specified radius using Bresenham's midpoint circle algorithm
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Midpoint_circle_algorithm">Based on Bresenham's midpoint circle algorithm</seealso>
        /// <param name="xCenter">X location of the center Block with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Block with 0 as the top</param>
        /// <param name="radius">The number of Blocks to get in a radius from the center Block</param>
        /// <returns>IEnumerable of outermost border Blocks in a circle around the center Block</returns>
        public IEnumerable<IBlock> GetBorderBlocksInCircle(int xCenter, int yCenter, int radius)
        {
            var discovered = new HashSet<int>();

            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                IBlock Block;
                if (AddToHashSet(discovered, ClampX(xCenter + x), ClampY(yCenter + y), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter + x), ClampY(yCenter - y), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter - x), ClampY(yCenter + y), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter - x), ClampY(yCenter - y), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter + y), ClampY(yCenter + x), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter + y), ClampY(yCenter - x), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter - y), ClampY(yCenter + x), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, ClampX(xCenter - y), ClampY(yCenter - x), out Block))
                {
                    yield return Block;
                }

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);
        }

        /// <summary>
        /// Get an IEnumerable of outermost border Blocks in a diamond (Rhombus) shape around the center Block up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Block with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Block with 0 as the top</param>
        /// <param name="distance">The number of Blocks to get in a distance from the center Block</param>
        /// <returns>IEnumerable of outermost border Blocks in a diamond (Rhombus) shape around the center Block</returns>
        public IEnumerable<IBlock> GetBorderBlocksInDiamond(int xCenter, int yCenter, int distance)
        {
            var discovered = new HashSet<int>();

            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(Width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(Height - 1, yCenter + distance);

            IBlock Block;
            if (AddToHashSet(discovered, xCenter, yMin, out Block))
            {
                yield return Block;
            }
            if (AddToHashSet(discovered, xCenter, yMax, out Block))
            {
                yield return Block;
            }
            for (int i = 1; i <= distance; i++)
            {
                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - i), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + i), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - i), out Block))
                {
                    yield return Block;
                }
                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + i), out Block))
                {
                    yield return Block;
                }
            }
        }

        /// <summary>
        /// Get an IEnumerable of outermost border Blocks in a square area around the center Block up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Block with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Block with 0 as the top</param>
        /// <param name="distance">The number of Blocks to get in each direction from the center Block</param>
        /// <returns>IEnumerable of outermost border Blocks in a square area around the center Block</returns>
        public IEnumerable<IBlock> GetBorderBlocksInSquare(int xCenter, int yCenter, int distance)
        {
            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(Width - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(Height - 1, yCenter + distance);

            for (int x = xMin; x <= xMax; x++)
            {
                yield return GetBlock(x, yMin);
                yield return GetBlock(x, yMax);
            }
            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                yield return GetBlock(xMin, y);
                yield return GetBlock(xMax, y);
            }
        }

        /// <summary>
        /// Get an IEnumerable of all the Blocks in the specified row numbers
        /// </summary>
        /// <param name="rowNumbers">Integer array of row numbers with 0 as the top</param>
        /// <returns>IEnumerable of all the Blocks in the specified row numbers</returns>
        public IEnumerable<IBlock> GetBlocksInRows(params int[] rowNumbers)
        {
            foreach (int y in rowNumbers)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return GetBlock(x, y);
                }
            }
        }

        /// <summary>
        /// Get an IEnumerable of all the Blocks in the specified column numbers
        /// </summary>
        /// <param name="columnNumbers">Integer array of column numbers with 0 as the farthest left</param>
        /// <returns>IEnumerable of all the Blocks in the specified column numbers</returns>
        public IEnumerable<IBlock> GetBlocksInColumns(params int[] columnNumbers)
        {
            foreach (int x in columnNumbers)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return GetBlock(x, y);
                }
            }
        }

        /// <summary>
        /// Get a Block at the specified location
        /// </summary>
        /// <param name="x">X location of the Block to get starting with 0 as the farthest left</param>
        /// <param name="y">Y location of the Block to get, starting with 0 as the top</param>
        /// <returns>Block at the specified location</returns>
        public IBlock GetBlock(int x, int y)
        {
            if(_Blocks[x,y] == null) { _Blocks[x, y] = new Block(x, y, false, false, false); }
            return _Blocks[x, y];
        }

        /// <summary>
        /// Provides a simple visual representation of the map using the following symbols:
        /// - `%`: `Block` is not in field-of-view
        /// - `.`: `Block` is transparent, walkable, and in field-of-view
        /// - `s`: `Block` is walkable and in field-of-view (but not transparent)
        /// - `o`: `Block` is transparent and in field-of-view (but not walkable)
        /// - `#`: `Block` is in field-of-view (but not transparent or walkable)
        /// </summary>
        /// <param name="useFov">True if field-of-view calculations will be used when creating the string represenation of the Map. False otherwise</param>
        /// <returns>A string representation of the map using special symbols to denote Block properties</returns>
        public string ToString(bool useFov)
        {
            var mapRepresentation = new StringBuilder();
            int lastY = 0;
            foreach (IBlock iBlock in GetAllBlocks())
            {
                Block Block = (Block)iBlock;
                if (Block.Y != lastY)
                {
                    lastY = Block.Y;
                    mapRepresentation.Append(Environment.NewLine);
                }
                mapRepresentation.Append(Block.ToString(useFov));
            }
            return mapRepresentation.ToString().TrimEnd('\r', '\n');
        }

        /// <summary>
        /// Get a MapState POCO which represents this Map and can be easily serialized
        /// Use Restore with the MapState to get back a full Map
        /// </summary>
        /// <returns>Mapstate POCO (Plain Old C# Object) which represents this Map and can be easily serialized</returns>
        public MapState Save()
        {
            var mapState = new MapState();
            mapState.Width = Width;
            mapState.Height = Height;
            mapState.Blocks = new MapState.BlockProperties[Width * Height];
            foreach (IBlock Block in GetAllBlocks())
            {
                var BlockProperties = MapState.BlockProperties.None;
                if (Block.IsInFov)
                {
                    BlockProperties |= MapState.BlockProperties.Visible;
                }
                if (Block.IsTransparent)
                {
                    BlockProperties |= MapState.BlockProperties.Transparent;
                }
                if (Block.IsWalkable)
                {
                    BlockProperties |= MapState.BlockProperties.Walkable;
                }
                mapState.Blocks[Block.Y * Width + Block.X] = BlockProperties;
            }
            return mapState;
        }

        /// <summary>
        /// Restore the state of this Map from the specified MapState
        /// </summary>
        /// <param name="state">Mapstate POCO (Plain Old C# Object) which represents this Map and can be easily serialized and deserialized</param>
        /// <exception cref="ArgumentNullException">Thrown on null map state</exception>
        public void Load(MapState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state", "Map state cannot be null");
            }

            var inFov = new HashSet<int>();

            Initialize(state.Width, state.Height);
            foreach (IBlock Block in GetAllBlocks())
            {
                MapState.BlockProperties BlockProperties = state.Blocks[Block.Y * Width + Block.X];
                if (EnumExtensions.HasFlag(BlockProperties, MapState.BlockProperties.Visible))
                {
                    inFov.Add(IndexFor(Block.X, Block.Y));
                }
                Block.IsTransparent = EnumExtensions.HasFlag(BlockProperties, MapState.BlockProperties.Transparent);
                Block.IsWalkable = EnumExtensions.HasFlag(BlockProperties, MapState.BlockProperties.Walkable);
            }

            _fieldOfView = new FieldOfView(this, inFov);
        }

        /// <summary>
        /// Static factory method which creates a new Map using the specified IMapCreationStrategy
        /// </summary>
        /// <param name="mapCreationStrategy">A class that implements IMapCreationStrategy and has CreateMap method which defines algorithms for creating interesting Maps</param>
        public static Map Create(IMapCreationStrategy<Map> mapCreationStrategy)
        {
            if (mapCreationStrategy == null)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Map creation strategy cannot be null"); }
            }

            return mapCreationStrategy.CreateMap();
        }

        /// <summary>
        /// Get the Block at the specified single dimensional array index using the formulas: x = index % Width; y = index / Width;
        /// </summary>
        /// <param name="index">The single dimensional array index for the Block that we want to get</param>
        /// <returns>Block at the specified single dimensional array index</returns>
        public IBlock BlockFor(int index)
        {
            int x = index % Width;
            int y = index / Width;

            return GetBlock(x, y);
        }

        /// <summary>
        /// Get the single dimensional array index for a Block at the specified location using the formula: index = ( y * Width ) + x
        /// </summary>
        /// <param name="x">X location of the Block index to get starting with 0 as the farthest left</param>
        /// <param name="y">Y location of the Block index to get, starting with 0 as the top</param>
        /// <returns>An index for the Block at the specified location useful if storing Blocks in a single dimensional array</returns>
        public int IndexFor(int x, int y)
        {
            return (y * Width) + x;
        }

        /// <summary>
        /// Get the single dimensional array index for the specified Block
        /// </summary>
        /// <param name="Block">The Block to get the index for</param>
        /// <returns>An index for the Block which is useful if storing Blocks in a single dimensional array</returns>
        /// <exception cref="ArgumentNullException">Thrown on null Block</exception>
        public int IndexFor(IBlock Block)
        {
            if (Block == null)
            {
                throw new ArgumentNullException("Block", "Block cannot be null");
            }

            return (Block.Y * Width) + Block.X;
        }

        private bool AddToHashSet(HashSet<int> hashSet, int x, int y, out IBlock Block)
        {
            Block = GetBlock(x, y);
            return hashSet.Add(IndexFor(Block));
        }

        private bool AddToHashSet(HashSet<int> hashSet, IBlock Block)
        {
            return hashSet.Add(IndexFor(Block));
        }

        /// <summary>
        /// Provides a simple visual representation of the map using the following symbols:
        /// - `.`: `Block` is transparent and walkable
        /// - `s`: `Block` is walkable (but not transparent)
        /// - `o`: `Block` is transparent (but not walkable)
        /// - `#`: `Block` is not transparent or walkable
        /// </summary>
        /// <remarks>
        /// This call ignores field-of-view. If field-of-view is important use the ToString overload with a "true" parameter
        /// </remarks>
        /// <returns>A string representation of the map using special symbols to denote Block properties</returns>
        public override string ToString()
        {
            return ToString(false);
        }
    }
}