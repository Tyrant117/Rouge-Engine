using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class MeshLayer : MonoBehaviour
    {
        [HideInInspector]
        public Mesh mesh;

        [HideInInspector]
        public Vector3[] meshVertices;

        [HideInInspector]
        public Vector2[] meshUVs;

        [HideInInspector]
        public Color[] meshColors;

        private MeshRenderer meshRenderer;

        /// <summary>
        /// Creates the mesh and assigns it to the layer it is on.
        /// </summary>
        /// <param name="combineInstances">The meshes to combine.</param>
        /// <param name="name">Name of the mesh.</param>
        /// <param name="material">Material the mesh will use.</param>
        /// <param name="z">Layer of the mesh.</param>
        public void CreateMesh(CombineInstance[] combineInstances, string name, Material material, float x, float y, float z)
        {

            // combine quads into a single mesh
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            mesh = meshFilter.mesh = new Mesh();
            mesh.name = name;
            mesh.CombineMeshes(combineInstances, true, true);

            // add mesh renderer
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            meshRenderer.enabled = false;
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // get mesh vertices
            meshVertices = mesh.vertices;

            // get mesh uvs
            meshUVs = mesh.uv;

            // init combined mesh vertex colors
            meshColors = new Color[mesh.vertexCount];

            // recalculate bounds
            mesh.RecalculateBounds();

            // center mesh
            gameObject.transform.position = new Vector3(x, y, z);
            //-meshRenderer.bounds.extents.x
            //+meshRenderer.bounds.extents.y
            //x, y,
        }

        /// <summary>
        /// Updates the mesh and displays it.
        /// </summary>
        public void UpdateMesh()
        {
            mesh.vertices = meshVertices;
            mesh.uv = meshUVs;
            mesh.colors = meshColors;
            meshRenderer.enabled = true;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }

        public bool RequireRedraw()
        {
            if(meshRenderer.enabled == false)
            {
                return true;
            }
            return false;
        }
    }
}
