using UnityEditor;
using UnityEngine;

namespace PCG
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor {

        public override void OnInspectorGUI() {
            MapGenerator mapGen = (MapGenerator)target;

            if (DrawDefaultInspector()) {
                mapGen.GenerateMap();
            }

            if (GUILayout.Button("Generate")) {
                mapGen.GenerateMap ();
            }
        }
    }
}