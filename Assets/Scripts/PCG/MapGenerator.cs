using System;
using System.Threading;
using PCG.Data;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TerrainData = PCG.Data.TerrainData;

namespace PCG
{
    public class MapGenerator : MonoBehaviour
    {
        public const int ChunkSize = 241;
        [Range(0,6)]
        public int editorPreviewLOD;

        public NoiseGenerator.NormalizeMode normalizeMode;
        
        public CameraMovement cameraMovement;

        public TerrainData terrainData;
        public NoiseData noiseData;
        public TextureData textureData;
        public PrefabsData prefabsData;

        public NavMeshSurface surface;

        public Material terrainMaterial;

        public GameObject player;
        public GameObject hunter;
        public LayerMask TerrainLayer;

        private Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        private Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

        public void Start()
        {
            //MeshData mesh = GenerateMap();
            //GeneratePrefabs(mesh);
            //GeneratePlayerAndHunter();
            //GenerateNavMesh();
            textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
            textureData.ApplyToMaterial(terrainMaterial);
        }

        private void GeneratePlayerAndHunter()
        {
            float positionYPlayer = 9999;
            float positionYHunter = 9999;
            RaycastHit hit;
            
            if (Physics.Raycast(new Vector3(0, 9999f, 0), Vector3.down,
                out hit, Mathf.Infinity, TerrainLayer))
            {
                positionYPlayer = hit.point.y;
                positionYHunter = hit.point.y;
            }

            positionYPlayer += 1.5f / 2;
            positionYHunter += hunter.GetComponent<CapsuleCollider>().height / 2;
            Vector3 positionPlayer = new Vector3(0, positionYPlayer, 0);
            Vector3 positionHunter = new Vector3(3, positionYHunter, 0);

            var playerObject = Instantiate(player, positionPlayer, Quaternion.identity);
            Instantiate(hunter, positionHunter, Quaternion.identity);

            cameraMovement.target = playerObject.transform;
        }

        public MapData GenerateMapData(Vector2 center)
        {
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(ChunkSize, ChunkSize, noiseData.seed, 
                noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, 
                center + noiseData.offset, normalizeMode);

            return new MapData(noiseMap);
        }

        public MeshData GenerateMap()
        {
            MapData mapData = GenerateMapData(transform.position);

            MapDisplay display = FindObjectOfType<MapDisplay>();

            textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

            textureData.ApplyToMaterial(terrainMaterial);

            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, terrainData.meshHeightMultiplier, 

            terrainData.meshHeightCurve, editorPreviewLOD);
            display.DrawMesh(meshData);

            return meshData;

        }

        private void GeneratePrefabs(MeshData mesh)
        {
            // loop through all triangles and select the positions to where instantiate prefabs
            int[] triangles = mesh.triangles;
            Vector3[] positions = new Vector3[triangles.Length / 3];
            int count = 0;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // only selecting the first vertex for each triangle (greedy approach)
                positions[count] = mesh.vertices[triangles[i]];
                ++count;
            }
        
            // determine where to instantiate the prefabs
            PrefabsInternalData prefabsInternalData = PrefabsGenerator.DeterminePrefabsPositions(ChunkSize, 
                ChunkSize, terrainData.meshHeightMultiplier, positions, prefabsData, normalizeMode);
        
            // instantiate the prefabs
            MapDisplay display = FindObjectOfType<MapDisplay> ();
            display.InstantiatePrefabs(prefabsInternalData);
        }

        public void RequestMapData(Vector2 center, Action<MapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, callback);
            };
            
            new Thread(threadStart).Start();
        }

        void MapDataThread(Vector2 center, Action<MapData> callback)
        {
            MapData mapData = GenerateMapData(center);

            lock (_mapDataThreadInfoQueue)
            {
                _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));    
            }
        }

        void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, terrainData.meshHeightMultiplier, 
                terrainData.meshHeightCurve, lod);
            lock (_meshDataThreadInfoQueue)
            {
                _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
            }
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, callback);
            };
            
            new Thread(threadStart).Start();
        }
        
        private void Update()
        {
            if (_mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }

            if (_meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }

        }

        private void GenerateNavMesh()
        {
            surface.BuildNavMesh();
        }
    }

    internal struct MapThreadInfo<T>
    {
        public readonly Action<T> Callback;
        public readonly T Parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            Callback = callback;
            Parameter = parameter;
        }
    }
}

public struct MapData
{
    public float[,] HeightMap;

    public MapData(float[,] heightMap)
    {
        HeightMap = heightMap;
    }
}