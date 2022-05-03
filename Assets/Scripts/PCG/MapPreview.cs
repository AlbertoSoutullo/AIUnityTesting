using System;
using System.Collections.Generic;
using PCG.Data;
using UnityEngine;
using UnityEngine.AI;

namespace PCG
{
    public class MapPreview : MonoBehaviour {

        public Renderer textureRender;
        public MeshFilter meshFilter;

        [Range(0,6)]
        public int editorPreviewLOD;
        public bool autoUpdate;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;
        public Material terrainMaterial;

        // public NavMeshSurface surface; todo
        
        public void DrawMapInEditor() {
            textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            textureData.ApplyToMaterial (terrainMaterial);
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

            MeshData mesh = MeshGenerator.GenerateTerrainMesh(heightMap.values, editorPreviewLOD);
            DrawMesh(mesh);
            PutPrefabsInMap test = FindObjectOfType<PutPrefabsInMap>();
            // test.GeneratePrefabs(meshFilter.mesh, Vector2.zero);
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