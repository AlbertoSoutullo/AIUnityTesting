using PCG.Data;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PCG
{

    public class TerrainChunk : MonoBehaviour
    {
        private string _name = "";
        public event System.Action<GameObject, bool> onVisibilityChanged;
        public Transform _viewer;
        
        private Mesh _mesh;
        private Bounds _bounds;

        private HeightMap _heightMap;
        public bool _heightMapReceived;

        private MeshRenderer _meshRenderer;
        public MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;

        private int previousLODIndex = -1;

        public HeightMapSettings _heightMapSettings;
        public int _size;

        private float _maxViewDistance;
        private Vector2 _position;
        
        
        public void PrepareChunk(Vector2 coord, HeightMapSettings settings, int size, LODInfo[] detailLevels, 
            Transform parent, Material material, Transform viewer)
        {
            _heightMapSettings = settings;
            _size = size;
            _detailLevels = detailLevels;
            _viewer = viewer;
            
            _position = coord * (_size-1);
            _name = $"{_position.x}:{_position.y}";
            _bounds = new Bounds(_position, Vector2.one * _size);

            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);
            
            int layer = LayerMask.NameToLayer("Ground");
            gameObject.layer = layer;
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();

            _meshRenderer.material = material;
            transform.position = positionV3;
            transform.parent = parent;
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
                            
                            //GetComponent<MeshCollider>().sharedMesh = null;
                            //GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
                            
                            _meshCollider = gameObject.AddComponent<MeshCollider>();
                            _meshCollider.sharedMesh = null;
                            _meshCollider.sharedMesh = _meshFilter.mesh;

                            PutPrefabsInMap test = FindObjectOfType<PutPrefabsInMap>();
                            test.GeneratePrefabs(_meshFilter.mesh, _position, gameObject.transform);
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
                            onVisibilityChanged(gameObject, visible);
                        }
                    }

                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
        
        public bool IsVisible()
        {
            return gameObject.activeSelf;
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