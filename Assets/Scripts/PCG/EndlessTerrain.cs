using System;
using System.Collections.Generic;
using UnityEngine;

namespace PCG
{
    public class EndlessTerrain : MonoBehaviour
    {
        public const float MAXViewDistance = 450;
        public Transform viewer;

        public static Vector2 ViewerPosition;
        private int _chunkSize;
        private int _chunkVisibleInViewDistance;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDicctionary = new Dictionary<Vector2, TerrainChunk>();
        private readonly List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        
        private void Start()
        {
            _chunkSize = MapGenerator.ChunkSize - 1;
            _chunkVisibleInViewDistance = Mathf.RoundToInt(MAXViewDistance / _chunkSize);
        }

        private void Update()
        {
            var position = viewer.position;
            ViewerPosition = new Vector2(position.x, position.z);
            
            UpdateVisibleChunks();
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
                        if (_terrainChunkDicctionary[viewedChunkCoord].IsVisible())
                        {
                            _terrainChunksVisibleLastUpdate.Add(_terrainChunkDicctionary[viewedChunkCoord]);
                        }
                    }
                    else
                    {
                        _terrainChunkDicctionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize,
                            transform));
                    }
                }
            }
        }

        private class TerrainChunk
        {
            private readonly GameObject _meshObject;
            private Bounds _bounds;

            public TerrainChunk(Vector2 coord, int size, Transform parent)
            {
                var position = coord * size;
                _bounds = new Bounds(position, Vector2.one * size);

                Vector3 positionV3 = new Vector3(position.x, 0, position.y);

                _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _meshObject.transform.position = positionV3;
                _meshObject.transform.localScale = Vector3.one * size / 10f; //10  is default units in plane
                _meshObject.transform.parent = parent;
                SetVisible(false);
            }

            public void UpdateTerrainChunk()
            {
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

                bool visible = viewerDistanceFromNearestEdge <= MAXViewDistance;
            
                SetVisible(visible);
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
    }
}