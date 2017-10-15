using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Rougelikeberry.Map
{
    public interface IMap
    {
        int Width { get; }
        int Height { get; }

        void Initialize(int width, int height);

        bool IsTransparent(int x, int y);
        bool IsWalkable(int x, int y);
        bool IsInFoV(int x, int y);
        bool IsExplored(int x, int y);

        void SetBlockProperties(int x, int y, bool isTransparent, bool isWalkable, bool isExplored);

        void Clear();
        void Clear(bool isTransparent, bool isWalkable);

        IMap Clone();

        void Copy(IMap source);
        void Copy(IMap source, int left, int top);

        ReadOnlyCollection<IBlock> ComputeFoV(int xOrigin, int yOrigin, int radius, bool lightWalls);

        ReadOnlyCollection<IBlock> AppendFoV(int xOrigin, int yOrigin, int radius, bool lightWalls);

        IEnumerable<IBlock> GetAllBlocks();

        IEnumerable<IBlock> GetBlocksAlongLine(int xOrigin, int yOrigin, int xEnd, int yEnd);

        IEnumerable<IBlock> GetBlocksInCircle(int xCenter, int yCenter, int radius);

        IEnumerable<IBlock> GetBlocksInDiamond(int xCenter, int yCenter, int distance);

        IEnumerable<IBlock> GetBlocksInSquare(int xCenter, int yCenter, int distance);

        IEnumerable<IBlock> GetBlocksInRows(params int[] rowNumbers);

        IEnumerable<IBlock> GetBlocksInColumns(params int[] columnNumbers);

        IBlock GetBlock(int x, int y);

        IEnumerable<IBlock> GetBorderBlocksInCircle(int xCenter, int yCenter, int radius);

        IEnumerable<IBlock> GetBorderBlocksInDiamond(int xCenter, int yCenter, int distance);

        IEnumerable<IBlock> GetBorderBlocksInSquare(int xCenter, int yCenter, int distance);

        MapState Save();
        void Load(MapState state);

        string ToString(bool useFov);

        IBlock BlockFor(int index);

        int IndexFor(int x, int y);

        int IndexFor(IBlock block);
    }
}