using PCG.Data;
using UnityEngine;

namespace PCG
{

    public class TerrainChunk
    {
        public event System.Action<TerrainChunk, bool> onVisibilityChanged;
        public Transform _viewer;
        
        private readonly GameObject _meshObject;
        private Bounds _bounds;

        private HeightMap _heightMap;
        private bool _heightMapReceived;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;

        private int previousLODIndex = -1;

        private HeightMapSettings _heightMapSettings;
        private int _size;

        private float _maxViewDistance;
        private Vector2 _position;
        
        
        public TerrainChunk(Vector2 coord, HeightMapSettings settings, int size, LODInfo[] detailLevels, 
            Transform parent, Material material, Transform viewer)
        {
            _heightMapSettings = settings;
            _size = size;
            _detailLevels = detailLevels;
            _viewer = viewer;
            
            _position = coord * _size;
            _bounds = new Bounds(_position, Vector2.one * _size);

            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("Terrain Chunk");
            int layer = LayerMask.NameToLayer("Ground");
            _meshObject.layer = layer;
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

            _maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        }

        public void Load()
        {
            ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(_size, _size,
                _heightMapSettings, _position), OnHeightMapReceived);
        }

        void OnHeightMapReceived(object heightMap)
        {
            _heightMap = (HeightMap)heightMap;
            _heightMapReceived = true;
            
            UpdateTerrainChunk();
        }

        private Vector2 ViewerPosition()
        {
            return new Vector2(_viewer.position.x, _viewer.position.y);
        }

        public void UpdateTerrainChunk()
        {
            if (_heightMapReceived)
            {
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition()));

                bool wasVisible = IsVisible();
                bool visible = viewerDistanceFromNearestEdge <= _maxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < _detailLevels.Length - 1; i++) {
                        if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceThreshold) {
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
                            _meshCollider = _meshObject.AddComponent<MeshCollider>();
                            _meshCollider.sharedMesh = _meshFilter.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_heightMap);
                        }
                    }

                    if (wasVisible != visible)
                    {
                        SetVisible(visible);
                        if (onVisibilityChanged != null)
                        {
                            onVisibilityChanged(this, visible);
                        }
                    }

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

        void OnMeshDataReceived(object meshDataObject)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            hasMesh = true;

            _updateCallback();
        }

        public void RequestMesh(HeightMap heightMap)
        {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values,
                _lod), OnMeshDataReceived);
        }
    }
}


[System.Serializable]
public struct LODInfo
{
    public int lod;
    public float visibleDistanceThreshold;
}