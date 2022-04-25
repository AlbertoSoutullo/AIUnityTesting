using System;
using System.Collections.Generic;
using PCG.Data;
using UnityEngine;

namespace PCG
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float viewerMoveThresholdForChunkUpdate = 25f;

        private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate *
                                                                viewerMoveThresholdForChunkUpdate;
        
        public LODInfo[] detailLevels;
        public static float MAXViewDistance;
        
        public Transform viewer;
        public Material mapMaterial;

        public static Vector2 ViewerPosition;
        private Vector2 _viewerPositionOld;
        private int _chunkSize;
        private int _chunkVisibleInViewDistance;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDicctionary = new Dictionary<Vector2, TerrainChunk>();
        private static List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        private static MapGenerator _mapGenerator;
        
        private void Start()
        {
            MAXViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
            
            _chunkSize = MapGenerator.ChunkSize - 1;
            _chunkVisibleInViewDistance = Mathf.RoundToInt(MAXViewDistance / _chunkSize);
            _mapGenerator = FindObjectOfType<MapGenerator>();
            
            UpdateVisibleChunks();
        }

        private void Update()
        {
            var position = viewer.position;
            ViewerPosition = new Vector2(position.x, position.z);
            
            if ((_viewerPositionOld - ViewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate);
            {
                _viewerPositionOld = ViewerPosition;
                UpdateVisibleChunks();                
            }

        }

        void UpdateVisibleChunks()
        {
            foreach (var terrainChunk in _terrainChunksVisibleLastUpdate)
            {
                terrainChunk.SetVisible(false);
            }
            _terrainChunksVisibleLastUpdate.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

            for (int yOffset = -_chunkVisibleInViewDistance; yOffset <= _chunkVisibleInViewDistance; yOffset++)
            {
                for (int xOffset = -_chunkVisibleInViewDistance; xOffset <= _chunkVisibleInViewDistance; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset,
                        currentChunkCoordY + yOffset);

                    if (_terrainChunkDicctionary.ContainsKey(viewedChunkCoord))
                    {
                        _terrainChunkDicctionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        _terrainChunkDicctionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize,
                            detailLevels, transform, mapMaterial));
                    }
                }
            }
        }

        private class TerrainChunk
        {
            private readonly GameObject _meshObject;
            private Bounds _bounds;

            private MapData _mapData;
            private bool _mapDataReceived;

            private MeshRenderer _meshRenderer;
            private MeshFilter _meshFilter;
            private LODInfo[] _detailLevels;
            private LODMesh[] _lodMeshes;

            private int previousLODIndex = -1;
            
            public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
            {
                _detailLevels = detailLevels;
                
                Vector2 position = coord * size;
                _bounds = new Bounds(position, Vector2.one * size);

                Vector3 positionV3 = new Vector3(position.x, 0, position.y);

                _meshObject = new GameObject("Terrain Chunk");
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshFilter = _meshObject.AddComponent<MeshFilter>();

                _meshRenderer.material = material;
                _meshObject.transform.position = positionV3;
                _meshObject.transform.parent = parent;
                SetVisible(false);

                _lodMeshes = new LODMesh[detailLevels.Length];
                for (int i = 0; i < _lodMeshes.Length; i++)
                {
                    _lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                }

                _mapGenerator.RequestMapData(position, OnMapDataReceived);
            }

            void OnMapDataReceived(MapData mapData)
            {
                _mapData = mapData;
                _mapDataReceived = true;
                
                UpdateTerrainChunk();
            }

            public void UpdateTerrainChunk()
            {
                if (_mapDataReceived)
                {
                    float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

                    bool visible = viewerDistanceFromNearestEdge <= MAXViewDistance;

                    if (visible)
                    {
                        int lodIndex = 0;
                        for (int i = 0; i < _detailLevels.Length - 1; i++)
                        {
                            if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceThreshold)
                            {
                                lodIndex = i + 1;
                            }
                            else { break; }
                        }

                        if (lodIndex != previousLODIndex)
                        {
                            LODMesh lodMesh = _lodMeshes[lodIndex];
                            if (lodMesh.hasMesh)
                            {
                                previousLODIndex = lodIndex;
                                _meshFilter.mesh = lodMesh.mesh;
                            }
                            else if (!lodMesh.hasRequestedMesh)
                            {
                                lodMesh.RequestMesh(_mapData);
                            }
                        }
                        _terrainChunksVisibleLastUpdate.Add(this);
                    }
                    SetVisible(visible);
                }
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }
            
            public bool IsVisible()
            {
                return _meshObject.activeSelf;
            }
        }
        
        class LODMesh
        {
            public Mesh mesh;
            public bool hasRequestedMesh;
            public bool hasMesh;
            private int _lod;
            private System.Action _updateCallback;

            public LODMesh(int lod, System.Action updateCallback)
            {
                _lod = lod;
                _updateCallback = updateCallback;
            }

            void OnMeshDataReceived(MeshData meshData)
            {
                mesh = meshData.CreateMesh();
                hasMesh = true;

                _updateCallback();
            }

            public void RequestMesh(MapData mapData)
            {
                hasRequestedMesh = true;
                _mapGenerator.RequestMeshData(mapData, _lod, OnMeshDataReceived);
            }
        }

        [System.Serializable]
        public struct LODInfo
        {
            public int lod;
            public float visibleDistanceThreshold;
        }
    }
}