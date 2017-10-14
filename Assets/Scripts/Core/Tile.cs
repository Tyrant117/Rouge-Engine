using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class Tile
    {
        public int ID; // Reference Name for accessing from a dictionary.
        public float x; // Start x position in tileset (Top left is 0).
        public float y; // Start y position in tileset (Top left is 0).
        public float xOffset; // X offset if using bitmapped font. (not square)
        public float yOffset; // Y offset if using bitmapped font. (not square)
        public float Width; // Width of tile.
        public float Height; // Height of tile.

        public Vector3[] vertices = new Vector3[4]; // Used in generating the mesh.
        public Vector2[] uvs = new Vector2[4]; // Used to assign uv to mesh.

        public Tile()
        {

        }

        public Tile(int id, float x, float y, float width, float height, float xoffset = 0, float yoffset = 0)
        {
            ID = id;
            this.x = x;
            this.y = y;
            Width = width;
            Height = height;
            xOffset = xoffset;
            yOffset = yoffset;
        }

        public Tile(int id, Rect rect, float xoffset = 0, float yoffset = 0)
        {
            ID = id;
            x = rect.x;
            y = rect.y;
            Width = rect.width;
            Height = rect.height;
            xOffset = xoffset;
            yOffset = yoffset;
        }

        /// <summary>
        /// Recalculates the tiles mesh and uvs.
        /// </summary>
        /// <param name="scaledWidth">Should be the same as tile width unless scaling.</param>
        /// <param name="scaledHeight">Should be the same as tile height unless scaling.</param>
        /// <param name="textureSize">Size of the texture being uv mapped.</param>
        /// <param name="bleed">If filtering is not set to point account for edge bleed of the tile.</param>
        public void RecalculateTile(float scaledWidth, float scaledHeight, float textureSize, float bleed)
        {

            // calculate tile vertices
            vertices[0] = new Vector3(0f + xOffset / scaledWidth, 0f + yOffset / scaledHeight, 0f);
            vertices[1] = new Vector3(1f * (Width / scaledWidth) + xOffset / scaledWidth, 1f * (Height / scaledHeight) + yOffset / scaledHeight, 0f);
            vertices[2] = new Vector3(1f * (Width / scaledWidth) + xOffset / scaledWidth, 0f + yOffset / scaledHeight, 0f);
            vertices[3] = new Vector3(0f + xOffset / scaledWidth, 1f * (Height / scaledHeight) + yOffset / scaledHeight, 0f);

            // calculate tile uvs
            uvs[0] = new Vector2((x / textureSize) + (bleed / textureSize), ((textureSize - (y + Height)) / textureSize) + (bleed / textureSize));
            uvs[1] = new Vector2(((x + Width) / textureSize) - (bleed / textureSize), ((textureSize - y) / textureSize) - (bleed / textureSize));
            uvs[2] = new Vector2(((x + Width) / textureSize) - (bleed / textureSize), ((textureSize - (y + Height)) / textureSize) + (bleed / textureSize));
            uvs[3] = new Vector2((x / textureSize) + (bleed / textureSize), ((textureSize - y) / textureSize) - (bleed / textureSize));
        }
    }
}