using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class RootConsole : MonoBehaviour
    {
        private const string TAG = "<color=cyan><b>[Root Console]</b></color>";

        public static RootConsole s_Singleton;

        public bool Initialized { get; set; }
        public Camera MainCamera;

        [Header("Display Resolution")]
        public int DisplayWidth = 80;
        public int DisplayHeight = 60;

        [Header("Transitions")]
        public AnimationCurve colorLerpCurve;

        [Header("Logging")]
        public LogFilter.FilterLevel LogLevel = LogFilter.FilterLevel.Debug;

        private ITileManager m_TileManager;
        private List<Console> m_Consoles = new List<Console>();

        private void OnValidate()
        {
            if (Application.isEditor)
            {
                // update the debug log level in the editor.
                LogFilter.currentLogLevel = (int)LogLevel;
            }
        }

        private void Awake()
        {
            // create singleton of object.
            s_Singleton = this;

            // add all console windows to the root.
            m_Consoles.Clear();
            m_Consoles.AddRange(GetComponentsInChildren<Console>());
        }

        private void Start()
        {
            m_TileManager = GetComponent<ITileManager>();

            // TODO: Add code here to allow user to switch out tiles easily. This will require changing the tilemanager attatched to the gameobject.

            if (m_TileManager == null)
            {
                if (LogFilter.logFatal) { Debug.LogErrorFormat("{1} Requires a tile manager component", TAG); }
            }

            // calculate quad size
            float quadWidth = m_TileManager.GetGridTileWidth() / m_TileManager.GetGridTileHeight();
            float quadHeight = m_TileManager.GetGridTileHeight() / m_TileManager.GetGridTileWidth();

            // calculate ui width scaling
            float scaledWidth = m_TileManager.GetUITileWidth() / m_TileManager.GetUITileHeight();

            // derive displayheight from desired display width
            int maxDisplayHeight = Mathf.RoundToInt((((float)Screen.height / (float)Screen.width) * (float)DisplayWidth) / quadHeight);

            // calculate the height adjustment needed to center the camera if the display isn't auto calibrated.
            float yCameraAdjust = (DisplayHeight - maxDisplayHeight) / 2F * quadHeight;

            // update camera orthographic size and position
            MainCamera.orthographicSize = (((float)Screen.height / (float)Screen.width) * (float)DisplayWidth) / quadHeight * quadHeight * 0.5F;
            var vec = MainCamera.transform.position;
            MainCamera.transform.position = new Vector3(vec.x, yCameraAdjust, vec.z);

            if (LogFilter.logInfo) { Debug.LogFormat("{0} Display Size: {1}x{2}", TAG, DisplayWidth, DisplayHeight); }

            // template primitive for creating quad meshes.
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(go.GetComponent<MeshCollider>());

            // Create the consoles that will be used for updating cells.
            foreach (Console console in m_Consoles)
            {
                switch (console.WidthScaling)
                {
                    case Console.ENUM_WidthScaling.Fixed:
                        console.Create(go, m_TileManager.GetGridMaterial(), DisplayWidth, DisplayHeight, m_TileManager, quadWidth);
                        break;
                    case Console.ENUM_WidthScaling.Scaled:
                        console.Create(go, m_TileManager.GetUIMaterial(), DisplayWidth, DisplayHeight, m_TileManager, scaledWidth);
                        break;
                }
            }

            //destroy template and complete intialization
            Destroy(go);
            Initialized = true;
        }
    }
}