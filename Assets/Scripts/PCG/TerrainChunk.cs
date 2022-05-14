// Unity Imports
using UnityEngine;

// Project Imports
using PCG.Data;

namespace PCG
{
    public class TerrainChunk : MonoBehaviour
    {
        private string _name = "";
        public event System.Action<GameObject, bool> ONVisibilityChanged;
        public Transform chunkViewer;
        
        private Bounds _bounds;

        private HeightMap _heightMap;
        public bool heightMapReceived;

        public MeshFilter meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;

        private int _previousLODIndex = -1;

        public HeightMapSettings heightMapSettings;
        public int chunkSize;

        private float _maxViewDistance;
        private Vector2 _position;
        
        public void PrepareChunk(Vector2 coord, HeightMapSettings settings, int size, LODInfo[] detailLevels, 
            Transform parent, Material material, Transform viewer)
        {
            SetVariables(settings, size, detailLevels, viewer, coord, material, parent);

            SetVisible(false);

            PrepareLODs(detailLevels);
        }

        private void PrepareLODs(LODInfo[] detailLevels)
        {
            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < _lodMeshes.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }
        }

        private void SetComponents()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        private void SetMeshProperties(Material material, HeightMapSettings settings, int size, LODInfo[] detailLevels,
            Transform viewer)
        {
            _meshRenderer.material = material;
            heightMapSettings = settings;
            chunkSize = size;
            _detailLevels = detailLevels;
            chunkViewer = viewer;
        }

        private void SetSpaceVariables(Vector2 coord, Transform parent)
        {
            _position = coord * (chunkSize-1);
            _bounds = new Bounds(_position, Vector2.one * chunkSize);
            
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);
            transform.position = positionV3;
            transform.parent = parent;
        }

        private void SetGameObjectInfo()
        {
            _name = $"{_position.x}:{_position.y}";
            int layer = LayerMask.NameToLayer("Ground");
            gameObject.layer = layer;
        }

        private void SetVariables(HeightMapSettings settings, int size, LODInfo[] detailLevels, Transform viewer,
            Vector2 coord, Material material, Transform parent)
        {
            SetComponents();
            SetMeshProperties(material, settings, size, detailLevels, viewer);
            SetSpaceVariables(coord, parent);
            SetGameObjectInfo();
            
            _maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        }

        public void Load()
        {
            ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(chunkSize, heightMapSettings, 
                _position), OnHeightMapReceived);
        }

        void OnHeightMapReceived(object heightMap)
        {
            _heightMap = (HeightMap)heightMap;
            heightMapReceived = true;
            
            UpdateTerrainChunk();
        }

        private Vector2 ViewerPosition()
        {
            Vector3 position = chunkViewer.position;
            return new Vector2(position.x, position.y);
        }

        public void UpdateTerrainChunk()
        {
            if (!heightMapReceived) return;
            
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition()));

            bool wasVisible = IsVisible();
            bool visible = viewerDistanceFromNearestEdge <= _maxViewDistance;

            if (visible)
            {
                RecalculateLODIfNeeded(viewerDistanceFromNearestEdge);

                if (wasVisible != true)
                {
                    SetVisible(true);
                    ONVisibilityChanged?.Invoke(gameObject, true);
                }
            }
            SetVisible(visible);
        }

        private void RecalculateLODIfNeeded(float viewerDistanceFromNearestEdge)
        {
            int lodIndex = RecalculateLODIndex(viewerDistanceFromNearestEdge);

            if (lodIndex != _previousLODIndex)
            {
                LODMesh lodMesh = _lodMeshes[lodIndex];
                if (lodMesh.HasMesh)
                {
                    UpdateMesh(lodIndex, lodMesh);

                    PutPrefabsInMap prefabInstantiator = FindObjectOfType<PutPrefabsInMap>();
                    prefabInstantiator.GeneratePrefabs(meshFilter.mesh, _position, gameObject.transform);
                }
                else if (!lodMesh.HasRequestedMesh)
                {
                    lodMesh.RequestMesh(_heightMap);
                }
            }
        }

        private void UpdateMesh(int lodIndex, LODMesh lodMesh)
        {
            _previousLODIndex = lodIndex;
            meshFilter.mesh = lodMesh.Mesh;
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = meshFilter.mesh;
        }

        private int RecalculateLODIndex(float viewerDistanceFromNearestEdge)
        {
            int lodIndex = 0;
            for (int i = 0; i < _detailLevels.Length - 1; i++) {
                if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceThreshold) {
                    lodIndex = i + 1;
                }
                else { break; }
            }

            return lodIndex;
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        private bool IsVisible()
        {
            return gameObject.activeSelf;
        }
    }
    
    class LODMesh
    {
        public Mesh Mesh;
        public bool HasRequestedMesh;
        public bool HasMesh;
        private readonly int _lod;
        private readonly System.Action _updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            _lod = lod;
            _updateCallback = updateCallback;
        }

        void OnMeshDataReceived(object meshDataObject)
        {
            Mesh = ((MeshData)meshDataObject).CreateMesh();
            HasMesh = true;

            _updateCallback();
        }

        public void RequestMesh(HeightMap heightMap)
        {
            HasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values,
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