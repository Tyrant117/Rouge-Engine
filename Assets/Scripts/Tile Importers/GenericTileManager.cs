using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class GenericTileManager : MonoBehaviour, ITileManager
    {
        private const string TAG = "<color=black><b>[Tile Manager]</b></color>";
        [Header ("Grid")]
        public Texture2D GridTexture;
        public Material GridMaterial;
        public int GridTileWidth;
        public int GridTileHeight;
        public float GridBleed;

        [Header ("Interface")]
        public Texture2D UITexture;
        public Material UIMaterial;
        public int UITileWidth;
        public int UITileHeight;
        public float UIBleed;

        public bool Loaded { get; set; }

        private Dictionary<int, Tile> m_Tiles = new Dictionary<int, Tile>();
        private Dictionary<int, Tile> m_UITiles = new Dictionary<int, Tile>();

        private void Awake()
        {
            int xRes = GridTexture.width / GridTileWidth;
            int yRes = GridTexture.height / GridTileHeight;

            int tileID = 0;
            for (int y = 0; y < yRes; y++)
            {
                for (int x = 0; x < xRes; x++)
                {
                    Tile tile = new Tile(tileID, x * GridTileWidth, y * GridTileHeight, GridTileWidth, GridTileHeight);
                    tileID++;
                    m_Tiles.Add(tile.ID, tile);
                    tile.RecalculateTile(GridTileWidth, GridTileHeight, GridTexture.width, GridBleed);
                }
            }

            xRes = UITexture.width / UITileWidth;
            yRes = UITexture.height / UITileHeight;

            tileID = 0;
            for (int y = 0; y < yRes; y++)
            {
                for (int x = 0; x < xRes; x++)
                {
                    Tile tile = new Tile(tileID, x * UITileWidth, y * UITileHeight, UITileWidth, UITileHeight);
                    tileID++;
                    m_UITiles.Add(tile.ID, tile);
                    tile.RecalculateTile(UITileWidth, UITileHeight, UITexture.width, UIBleed);
                }
            }

            Loaded = true;
            if (LogFilter.logInfo) { Debug.LogFormat("{0} Texture Size: {1} || Tiles Loaded: {2}", TAG, GridTexture.width, m_Tiles.Count); }
            if (LogFilter.logInfo) { Debug.LogFormat("{0} Texture Size: {1} || Tiles Loaded: {2}", TAG, UITexture.width, m_UITiles.Count); }
        }

        #region --- Grid ---
        public Material GetGridMaterial()
        {
            return GridMaterial;
        }

        public float GetGridTileHeight()
        {
            return GridTileHeight;
        }

        public float GetGridTileWidth()
        {
            return GridTileWidth;
        }

        public Tile GetGridTile(int id)
        {
            Tile tile;
            if (m_Tiles.TryGetValue(id, out tile))
            {
                return tile;
            }
            else
            {
                Debug.LogError("Tile is not in dictionary");
                return tile;
            }
        }
        #endregion

        #region --- Interface ---
        public Material GetUIMaterial() { return UIMaterial; }

        public float GetUITileHeight() { return UITileHeight; }

        public float GetUITileWidth() { return UITileWidth; }

        public Tile GetUITile(int id)
        {
            Tile tile;
            if (m_UITiles.TryGetValue(id, out tile))
            {
                return tile;
            }
            else
            {
                if (LogFilter.logFatal) { Debug.LogErrorFormat("{1} Tile is not in UI Dictionary.", TAG); }
                return tile;
            }
        }
        #endregion
    }
}