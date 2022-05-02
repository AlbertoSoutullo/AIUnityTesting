using System.Collections.Generic;
using PCG.Data;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace PCG
{
    public class TerrainGenerator : MonoBehaviour
    {
        private const float viewerMoveThresholdForChunkUpdate = 25f;

        private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate *
                                                                viewerMoveThresholdForChunkUpdate;

        public HeightMapSettings heightMapSettings;
        public TextureData textureSettings;
        public MeshSettings meshSettings;

        public LODInfo[] detailLevels;

        public Transform viewer;
        public Material mapMaterial;

        public NavMeshSurface surface;
        
        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;
        private int _chunkSize;
        private int _chunkVisibleInViewDistance;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDicctionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        
        
        private void Start()
        {
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            textureSettings.ApplyToMaterial(mapMaterial);
            
            float MAXViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
            
            _chunkSize = meshSettings.numVertsPerLine;
            _chunkVisibleInViewDistance = Mathf.RoundToInt(MAXViewDistance / _chunkSize);

            UpdateVisibleChunks();
            surface.BuildNavMesh();
        }

        private void Update()
        {
            var position = viewer.position;
            _viewerPosition = new Vector2(position.x, position.z);
            
            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                _viewerPositionOld = _viewerPosition;
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

            int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

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
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, _chunkSize,
                            detailLevels, transform, mapMaterial, viewer);
                        _terrainChunkDicctionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChange;
                        newChunk.Load();
                    }
                }
            }
        }

        void OnTerrainChunkVisibilityChange(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible) {
                _terrainChunksVisibleLastUpdate.Add(chunk);
            }
            else {
                _terrainChunksVisibleLastUpdate.Remove(chunk);
            }
        }
    }
}