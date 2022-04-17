using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class MapGenerator : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;

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
    public void Start()
    {
        MeshData mesh = GenerateMap();
        GeneratePrefabs(mesh);
        GeneratePlayerAndHunter();
        GenerateNavMesh();
    }

    public void GeneratePlayerAndHunter()
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

    public MeshData GenerateMap() {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap (mapWidth, mapHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        MapDisplay display = FindObjectOfType<MapDisplay> ();
        textureData.UpdateMeshHeights (terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        textureData.ApplyToMaterial(terrainMaterial);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve);
        display.DrawMesh (meshData);
        return meshData;
    }

    public void GeneratePrefabs(MeshData mesh)
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
        PrefabsInternalData prefabsInternalData = PrefabsGenerator.DeterminePrefabsPositions(mapWidth, mapHeight, terrainData.meshHeightMultiplier, positions, prefabsData);
        
        // instantiate the prefabs
        MapDisplay display = FindObjectOfType<MapDisplay> ();
        display.InstantiatePrefabs(prefabsInternalData);
    }

    public void GenerateNavMesh()
    {
        surface.BuildNavMesh();
    }

    private void OnValidate() {
        if (mapWidth < 1) 
        {
            mapWidth = 1;
        }
        if (mapHeight < 1) 
        {
            mapHeight = 1;
        }
    }
}
