using System.Numerics;
using PCG.Data;

namespace PCG
{
    public class PutPrefabsInMap
    {
        //todo
        /*
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
                ChunkSize, terrainData.meshHeightMultiplier, positions, prefabsData, normalizeMode, useFalloff,
                fallOffMap);
        
            // instantiate the prefabs
            MapDisplay display = FindObjectOfType<MapDisplay> ();
            display.InstantiatePrefabs(prefabsInternalData);
        }
        */
    }
}