using UnityEditor;
using UnityEngine;

namespace PCG.Data
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : Editor {

        public override void OnInspectorGUI() {
            MapPreview mapPreview = (MapPreview)target;

            if (DrawDefaultInspector()) {
                if (mapPreview.autoUpdate)
                {
                    mapPreview.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate")) {
                mapPreview.DrawMapInEditor();
            }
        }
    }
}