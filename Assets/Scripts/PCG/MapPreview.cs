// Unity Imports
using UnityEngine;

//Project Imports
using PCG.Data;

namespace PCG
{
    public class MapPreview : MonoBehaviour {

        public Renderer textureRender;
        public MeshFilter meshFilter;

        [Range(0,6)]
        public int editorPreviewLOD;
        public bool autoUpdate;

        public bool autoInstantiatePrefabs;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;
        public Material terrainMaterial;

        public void DrawMapInEditor() 
        {
            textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            textureData.ApplyToMaterial (terrainMaterial);
            
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(MeshSettings.numVertsPerLine, 
                heightMapSettings, Vector2.zero);

            MeshData mesh = MeshGenerator.GenerateTerrainMesh(heightMap.Values, editorPreviewLOD);
            DrawMesh(mesh);
            
            PutPrefabsInMap prefabGenerator = FindObjectOfType<PutPrefabsInMap>();
            
            if (autoInstantiatePrefabs)
            {
                foreach (Transform child in meshFilter.gameObject.transform) {
                    DestroyImmediate(child.gameObject);
                }
                prefabGenerator.GeneratePrefabs(meshFilter.mesh, Vector2.zero, meshFilter.gameObject.transform);
            }
        }

        private void DrawMesh(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh ();

            textureRender.gameObject.SetActive (false);
            meshFilter.gameObject.SetActive (true);
        }
        
        void OnValuesUpdated() {
            if (!Application.isPlaying) {
                DrawMapInEditor();
            }
        }

        void OnTextureValuesUpdated() {
            textureData.ApplyToMaterial (terrainMaterial);
        }

        void OnValidate() {

            if (meshSettings != null) {
                meshSettings.OnValuesUpdated -= OnValuesUpdated;
                meshSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (heightMapSettings != null) {
                heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (textureData != null) {
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
    }
}