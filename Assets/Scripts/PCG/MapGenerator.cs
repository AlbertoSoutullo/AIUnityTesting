using System;
using System.Threading;
using PCG.Data;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace PCG
{
    public class MapGenerator : MonoBehaviour
    {
        public const int ChunkSize = 241;
        
        private float[,] fallOffMap;

        public PrefabsData prefabsData;
        
        public void Start()
        {
            fallOffMap = FallOffGenerator.GenerateFallOffMap(ChunkSize);

            //MeshData mesh = GenerateMap();
            //GeneratePrefabs(mesh);
            //GeneratePlayerAndHunter();
            //GenerateNavMesh();
        }
        /*
        public MeshData GenerateMap()
        {
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(ChunkSize, ChunkSize, heightMapSettings,
                Vector2.zero);

            MapPreview preview = FindObjectOfType<MapPreview>();

            textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

            textureData.ApplyToMaterial(terrainMaterial);

            MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD);
            preview.DrawMesh(meshData);

            return meshData;
        }

        private void GenerateNavMesh()
        {
            surface.BuildNavMesh();
        }*/

        private void OnValidate()
        {
            fallOffMap = FallOffGenerator.GenerateFallOffMap(ChunkSize);
        }
    }


}

