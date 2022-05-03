using System.Collections;
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
        public GameObject terrainChunk;
        
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
        
        public LayerMask TerrainLayer;
        
        private void Start()
        {
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            textureSettings.ApplyToMaterial(mapMaterial);
            
            float MAXViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
            
            _chunkSize = meshSettings.numVertsPerLine;
            _chunkVisibleInViewDistance = Mathf.RoundToInt(MAXViewDistance / _chunkSize);

            UpdateVisibleChunks();
            StartCoroutine(UpdatePlayerPosition());
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
                        TerrainChunk newChunk = gameObject.AddComponent<TerrainChunk>();
                        newChunk.PrepareChunk(viewedChunkCoord, heightMapSettings, _chunkSize, detailLevels, 
                            transform, mapMaterial, viewer);
                        // TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, _chunkSize,
                        //     detailLevels, transform, mapMaterial, viewer);
                        _terrainChunkDicctionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChange;
                        newChunk.Load();
                    }
                }
            }
        }

        private IEnumerator UpdatePlayerPosition()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 3;
            
            float positionYPlayer = 0;
            float positionYHunter = 0;
            RaycastHit hit;
            
            if (Physics.Raycast(new Vector3(0, 9999f, 0), Vector3.down,
                out hit, Mathf.Infinity, TerrainLayer))
            {
                Debug.Log("HIT");
                positionYPlayer = hit.point.y;
            }

            positionYPlayer += 1.5f / 2; // model scale is not correct, so distance is manually added
            
            Vector3 positionPlayer = new Vector3(0, positionYPlayer, 0);

            viewer.position = positionPlayer;
            viewer.gameObject.SetActive(true);
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