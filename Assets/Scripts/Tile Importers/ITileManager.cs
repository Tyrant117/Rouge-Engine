using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public interface ITileManager
    {
        Material GetGridMaterial();
        Material GetUIMaterial();

        float GetGridTileHeight();
        float GetGridTileWidth();

        float GetUITileHeight();
        float GetUITileWidth();

        Tile GetGridTile(int id);
        Tile GetUITile(int id);
    }
}