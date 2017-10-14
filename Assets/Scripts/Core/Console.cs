using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class Console : MonoBehaviour
    {
        public enum ENUM_WidthScaling { Fixed, Scaled }
        public enum ENUM_Layer { Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine }

        public ENUM_WidthScaling WidthScaling;
        public ENUM_Layer Layer;
        public bool Visible;
        public bool Initialized { get; set; }
        public Material backgroundMaterial;

        public Vector2 Start;
        public Vector2 End;

        public MeshLayer Background;
        public MeshLayer Foreground;
        public MeshLayer Animation;

        private ITileManager m_TileManager;

        private int displayWidth;
        private int displayHeight;
        private float quadWidth;
        private float quadHeight;

        private bool isDirty;

        private static Vector3 zero3 = Vector3.zero;
        private static Vector2 zero2 = Vector2.zero;
        private Color clearColor = Color.clear;

        private Cell[,] Cells;
        private Stack<Cell> DirtyCells = new Stack<Cell>();
        private int MaxDirtCells = 0;

        #region --- Unity Methods and Initialization ---
        public void Create(GameObject go, Material foregroundMaterial, int screenWidth, int screenHeight, ITileManager tileManager, float widthRatio)
        {
            int xStart = (int)Start.x;
            int yStart = (int)Start.y;
            int xEnd = (int)End.x;
            int yEnd = (int)End.y;

            m_TileManager = tileManager;

            // Calculate the console dimensions.
            displayWidth = Mathf.RoundToInt((xEnd - xStart));
            displayHeight = Mathf.RoundToInt((yEnd - yStart));

            // Calculate the max number of tiles that can fit in aspect ratio.
            int maxTile = Mathf.RoundToInt((((float)Screen.height / (float)Screen.width) * (float)screenWidth) / 1F);

            // Calculate heigh scaling adjustment to fit inside aspect ratio.
            float adjust = (((float)maxTile / (float)screenHeight)); 
            quadHeight = 1F * adjust;

            // Does the width scale. This increases number of horizontal tiles, but makes them not square. Used for displaying UI text, not maps.
            switch (WidthScaling)
            {
                case ENUM_WidthScaling.Fixed:
                    quadWidth = 1F;
                    break;
                case ENUM_WidthScaling.Scaled:
                    quadWidth = 1F * widthRatio;
                    break;
            }
            displayWidth = Mathf.RoundToInt(displayWidth / quadWidth);

            // Create cells
            Cells = new Cell[displayWidth, displayHeight];
            for (int y = 0; y < displayHeight; y++)
            {
                for (int x = 0; x < displayWidth; x++)
                {
                    Cells[x, y] = CreateCell(x, y);
                }
            }
            DirtyCells = new Stack<Cell>(MaxDirtCells * 2);

            // Instantiate quads
            int quadMeshFilterIndex = 0;
            MeshFilter[] quadMeshFilters = new MeshFilter[displayWidth * displayHeight];
            for (int y = 0; y < displayHeight; y++)
            {
                for (int x = 0; x < displayWidth; x++)
                {

                    // Instantiate from prefab
                    GameObject quad = (GameObject)GameObject.Instantiate(go);
                    quad.transform.parent = transform;
                    quad.transform.localScale = new Vector3(quadWidth, quadHeight, 1f);
                    quad.transform.position = new Vector3(x * quadWidth + quadWidth * 0.5f, -y * quadHeight - quadHeight * 0.5f, 0f);

                    // Add to array for combining later
                    quadMeshFilters[quadMeshFilterIndex] = quad.GetComponent<MeshFilter>();
                    quadMeshFilterIndex++;
                }
            }

            // Add quads to combine instances
            CombineInstance[] combineInstances = new CombineInstance[quadMeshFilters.Length];
            for (int i = 0; i < quadMeshFilters.Length; i++)
            {
                combineInstances[i].mesh = quadMeshFilters[i].sharedMesh;
                combineInstances[i].transform = quadMeshFilters[i].transform.localToWorldMatrix;
            }

            // Combine quads to foreground and background
            Background.CreateMesh(combineInstances, "background_quads", backgroundMaterial, xStart - screenWidth * 1 / 2, screenHeight * 1 / 2 - yStart * quadHeight, 0.001F);
            Foreground.CreateMesh(combineInstances, "foreground_quads", foregroundMaterial, xStart - screenWidth * 1 / 2, screenHeight * 1 / 2 - yStart * quadHeight, 0F);
            Animation.CreateMesh(combineInstances, "animation_quads", foregroundMaterial, xStart - screenWidth * 1 / 2, screenHeight * 1 / 2 - yStart * quadHeight, -0.001F);

            // Destroy original quads
            for (int i = quadMeshFilters.Length - 1; i >= 0; i--)
            {
                GameObject.Destroy(quadMeshFilters[i].gameObject);
            }
            quadMeshFilters = null;

            transform.position = new Vector3(0, 0, -1 * (int)Layer);
            Initialized = true;
        }

        public void Update()
        {
            if (!Initialized) { return; }
            // Update dirty cells
            while (DirtyCells.Count > 0)
            {
                DirtyCells.Pop().Update();
                isDirty = true;
            }
        }

        private void LateUpdate()
        {
            if (!Initialized) { return; }
            // Draw cells if console is visible and dirty.
            if (Visible && isDirty)
            {
                Draw();
                isDirty = false;
            }
            else if (Visible)
            {
                // No update needs to happen, but redraw if it was disabled.
                if(Background.RequireRedraw() || Foreground.RequireRedraw() || Animation.RequireRedraw())
                {
                    Draw();
                    isDirty = false;
                }
            }
            else
            {
                // Hide the mesh.
                Background.Hide();
                Foreground.Hide();
                Animation.Hide();
            }
        }
        #endregion

        #region --- Drawing ---
        public void Draw()
        {
            for (int y = 0; y < displayHeight; y++)
            {
                for (int x = 0; x < displayWidth; x++)
                {
                    Cell cell = null;
                    cell = Cells[x, y];

                    // empty cell
                    if (cell == null || cell.content == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            // update display mesh vertices, uvs and colors
                            Foreground.meshVertices[(y * displayWidth + x) * 4 + i] = zero3;
                            Foreground.meshUVs[(y * displayWidth + x) * 4 + i] = zero2;
                            Foreground.meshColors[(y * displayWidth + x) * 4 + i] = clearColor;

                            Animation.meshVertices[(y * displayWidth + x) * 4 + i] = zero3;
                            Animation.meshUVs[(y * displayWidth + x) * 4 + i] = zero2;
                            Animation.meshColors[(y * displayWidth + x) * 4 + i] = clearColor;

                            Background.meshColors[(y * displayWidth + x) * 4 + i] = cell != null ? cell.backgroundColor : clearColor;
                        }
                    }
                    else // filled cell
                    {
                        Tile tile = (WidthScaling == ENUM_WidthScaling.Fixed) ? m_TileManager.GetGridTile(cell.content) : m_TileManager.GetUITile(cell.content);
                        for (int i = 0; i < 4; i++)
                        {
                            // update display mesh vertices, uvs and colors
                            Foreground.meshVertices[(y * displayWidth + x) * 4 + i] = new Vector3(x * quadWidth + tile.vertices[i].x * quadWidth, -y * quadHeight + tile.vertices[i].y * quadHeight - quadHeight, 0f);
                            Foreground.meshUVs[(y * displayWidth + x) * 4 + i] = tile.uvs[i];
                            Foreground.meshColors[(y * displayWidth + x) * 4 + i] = cell.color;

                            Animation.meshVertices[(y * displayWidth + x) * 4 + i] = new Vector3(x * quadWidth + tile.vertices[i].x * quadWidth, -y * quadHeight + tile.vertices[i].y * quadHeight - quadHeight, 0f);
                            Animation.meshUVs[(y * displayWidth + x) * 4 + i] = tile.uvs[i];
                            Animation.meshColors[(y * displayWidth + x) * 4 + i] = cell.animationColor;

                            Background.meshColors[(y * displayWidth + x) * 4 + i] = cell.backgroundColor;
                        }
                    }
                }
            }

            // apply display mesh updates
            Background.UpdateMesh();
            Foreground.UpdateMesh();
            Animation.UpdateMesh();
        }
        #endregion

        #region --- Cell Methods ---
        private Cell CreateCell(int x, int y)
        {
            // Create a new cell
            Cell cell = new Cell();
            cell.X = x;
            cell.Y = y;
            cell.owner = this;
            MaxDirtCells++;
            return cell;
        }

        public void DiryCell(Cell c)
        {
            DirtyCells.Push(c);
        }

        public Cell GetCell(int x, int y)
        {
            if (!Initialized) { return null; }
            // If cell is within bound return it.
            if (x >= 0 && y >= 0 && x < displayWidth && y < displayHeight)
            {
                return Cells[x, y];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region --- Helper Methods ---
        public int GetWidth()
        {
            return displayWidth;
        }

        public int GetHeight()
        {
            return displayHeight;
        }
        #endregion
    }
}