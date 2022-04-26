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
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        
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
            
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, editorPreviewLOD));
        }

        private void DrawMesh(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh ();

            textureRender.gameObject.SetActive (false);
            meshFilter.gameObject.SetActive (true);
        }
        
        void OnValuesUpdated() {
            if (!Application.isPlaying) {
                DrawMapInEditor ();
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
        
        // todo : move this out
        public void InstantiatePrefabs(PrefabsInternalData prefabsInternalData)
        {
            Debug.Log("All prefabs to instantiate: " + prefabsInternalData.prefabsPositions.Count);
            List<Tuple<int, int>> prefabsPositions = prefabsInternalData.prefabsPositions;
        
            // create parent objects
            GameObject[] parents = new GameObject[prefabsInternalData.transformsNames.Length];
            for (int i = 0; i < prefabsInternalData.transformsNames.Length; ++i)
            {
                parents[i] = new GameObject(prefabsInternalData.transformsNames[i]);
            }
        
            // instantiate all prefabs
            var random = new System.Random();
            for (int i = 0; i < prefabsPositions.Count; ++i)
            {
                Vector3 position = prefabsInternalData.positions[prefabsPositions[i].Item1];
                List<Transform> prefabTransforms = prefabsInternalData.transforms[prefabsPositions[i].Item2];
                // sample a random item from the transforms list
                Transform prefabTransform = prefabTransforms[random.Next(prefabTransforms.Count)];
            
                var prefab = Instantiate(prefabTransform, parents[prefabsPositions[i].Item2].transform);
                prefab.position = position;

            }
        }
    }
}