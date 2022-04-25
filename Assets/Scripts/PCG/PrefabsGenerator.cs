using System;
using UnityEngine;
using System.Collections.Generic;
using PCG;

public static class PrefabsGenerator {

    public static PrefabsInternalData DeterminePrefabsPositions(int mapWidth, int mapHeight, float heightMultiplier, Vector3[] oldPositions, PrefabsData prefabsData,
        NoiseGenerator.NormalizeMode normalizeMode) {
        System.Random random = new System.Random(prefabsData.seed);
        
        AnimationCurve noiseImportance = prefabsData.noiseImportance;
        PrefabsData.Prefab[] prefabs = prefabsData.prefabs;
        
        // create objects for the display class later on
        List<float[,]> noiseMaps = new List<float[,]>();
        Vector3[] positions = new Vector3[oldPositions.Length + 1];
        Array.Copy(oldPositions, positions, oldPositions.Length);
        List<Transform>[] prefabsTransforms = new List<Transform>[prefabs.Length + 1];
        String[] transformsNames = new String[prefabs.Length + 1];
        
        // add cabin objects
        prefabsTransforms[prefabs.Length] = new List<Transform>();
        prefabsTransforms[prefabs.Length].Add(prefabsData.cabinPrefab.Transform);
        transformsNames[prefabs.Length] = "cabin";
        
        // determine cabin position
        Vector3 cabinPosition = DetermineCabinPosition(positions, prefabsData.cabinPrefab.heightImportance, heightMultiplier, random, prefabsData.seed);
        positions[oldPositions.Length] = cabinPosition;

        // create the noise maps for all prefabs
        for (int i = 0; i < prefabs.Length; ++i)
        {
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap (
                mapWidth, 
                mapHeight, 
                prefabs[i].noise.seed, 
                prefabs[i].noise.noiseScale, 
                prefabs[i].noise.octaves, 
                prefabs[i].noise.persistance, 
                prefabs[i].noise.lacunarity, 
                prefabs[i].noise.offset,
                normalizeMode);
            noiseMaps.Add(noiseMap);
            prefabsTransforms[i] = prefabs[i].transforms;
            transformsNames[i] = prefabs[i].name;
        }
        
        // create the internal data object for display everything later on
        PrefabsInternalData prefabsInternalData = new PrefabsInternalData(positions, prefabsTransforms, transformsNames);
        
        // add cabin
        prefabsInternalData.AddPositionPrefabs(oldPositions.Length, prefabs.Length);

        // loop all positions and determine whether to instantiate the prefabs
        for (int i = 0; i < positions.Length; ++i)
        {
            for (int j = 0; j < prefabs.Length; ++j)
            {
                Vector3 position = positions[i];
                PrefabsData.Prefab prefab = prefabs[j];
                float[,] noiseMap = noiseMaps[j];
                
                // weighted sum between the position height and the noise of the prefab
                int positionInNoiseMapX = (int) Math.Round(position[0] - (noiseMap.GetLength (0) - 1) / -2f);
                int positionInNoiseMapZ = (int) Math.Round((noiseMap.GetLength (1) - 1) / 2f - position[2]);
                float positionY = position[1] / heightMultiplier;

                float heightProbability = prefab.heightImportance.Evaluate(positionY);
                float noiseProbability = noiseImportance.Evaluate(noiseMap[positionInNoiseMapX, positionInNoiseMapZ]);

                // sample true or false with the given probability
                double randomNumber = random.NextDouble();
                if (randomNumber < heightProbability & randomNumber < noiseProbability)
                {
                    prefabsInternalData.AddPositionPrefabs(i, j);
                }
            }
        }

        return prefabsInternalData;
    }

    private static Vector3 DetermineCabinPosition(Vector3[] positions, AnimationCurve heightImportance, float heightMultiplier, System.Random random, int seed)
    {
        // randomly sample a set of positions
        List<Vector3> randomlySampledPositions = new List<Vector3>();
        for (int i = 0; i < 40; ++i)
        {
            randomlySampledPositions.Add(positions[random.Next(positions.Length)]);
        }
        
        // give a weight to each position regarding the height
        float[] weights = new float[randomlySampledPositions.Count];
        float totalWeight = heightImportance.Evaluate(randomlySampledPositions[0][1] / heightMultiplier);
        float maxWeight = totalWeight;
        int maxWeightIndex = 0;
        for (int i = 1; i < weights.Length; ++i)
        {
            weights[i] = heightImportance.Evaluate(randomlySampledPositions[i][1] / heightMultiplier);
            if (weights[i] > maxWeight)
            {
                maxWeight = weights[i];
                maxWeightIndex = i;
            }
            totalWeight += weights[i];
        }
        
        // sample one of the positions with the given weights (Fitness proportionate selection approach) 
        float total = 0;
        UnityEngine.Random.seed = seed;
        float amount = UnityEngine.Random.Range(0.0f, totalWeight);
        int selectedIndex = -1;
        for (int i = 0; i < weights.Length; ++i){
            total += weights[i];
            if (amount <= total)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex != 1)
        {
            return randomlySampledPositions[selectedIndex];
        }
        else
        {
            return randomlySampledPositions[maxWeightIndex];
        }
    }
}

public class PrefabsInternalData
{
    public Vector3[] positions;
    public List<Transform>[] transforms;
    public String[] transformsNames;
    public List<Tuple<int, int>> prefabsPositions;

    public PrefabsInternalData(Vector3[] positions, List<Transform>[] transforms, String[] transformsNames)
    {
        this.positions = positions;
        this.transforms = transforms;
        this.transformsNames = transformsNames;
        this.prefabsPositions = new List<Tuple<int, int>>();
    }

    public void AddPositionPrefabs(int positionIndex, int prefabsIndex)
    {
        this.prefabsPositions.Add(new Tuple<int, int>(positionIndex, prefabsIndex));
    }

}