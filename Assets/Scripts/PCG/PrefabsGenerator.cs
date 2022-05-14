// Unity Imports
using System;
using System.Collections.Generic;
using UnityEngine;

// Project Imports
using PCG.Data;

namespace PCG
{
    public static class PrefabsGenerator
    {
        private const int RandomCabinPositions = 40;

        public static PrefabsInternalData DeterminePrefabsPositions(int mapSize, float heightMultiplier, 
            Vector3[] oldPositions, PrefabsData prefabsData) 
        {
            System.Random random = new System.Random(prefabsData.seed);
            AnimationCurve noiseImportance = prefabsData.noiseImportance;
            PrefabsData.Prefab[] prefabs = prefabsData.prefabs;
            List<float[,]> noiseMaps = new List<float[,]>();
            
            Vector3[] positions = new Vector3[oldPositions.Length + 1];
            Array.Copy(oldPositions, positions, oldPositions.Length);
            List<Transform>[] prefabsTransforms = new List<Transform>[prefabs.Length + 1];
            String[] transformsNames = new String[prefabs.Length + 1];
        
            AddCabin(prefabsTransforms, prefabs, prefabsData, transformsNames, positions, heightMultiplier,
                oldPositions, random);
            
            GeneratePrefabNoiseMap(prefabs, prefabsData, mapSize, prefabsTransforms, transformsNames, noiseMaps);
            
            PrefabsInternalData prefabsInternalData = new PrefabsInternalData(positions, prefabsTransforms, 
                transformsNames);
            
            prefabsInternalData.AddPositionPrefabs(oldPositions.Length, prefabs.Length);

            DeterminePrefabAppearanceInInternalData(prefabs, positions, noiseMaps, heightMultiplier, noiseImportance, 
                prefabsInternalData, random);

            return prefabsInternalData;
        }

        private static void DeterminePrefabAppearanceInInternalData(PrefabsData.Prefab[] prefabs, Vector3[] positions, 
            List<float[,]> noiseMaps, float heightMultiplier, AnimationCurve noiseImportance, 
            PrefabsInternalData prefabsInternalData , System.Random random)
        {
            for (int i = 0; i < positions.Length - 1; ++i)
            {
                for (int j = 0; j < prefabs.Length; ++j)
                {
                    Vector3 position = positions[i];
                    PrefabsData.Prefab prefab = prefabs[j];
                    float[,] noiseMap = noiseMaps[j];

                    (float heightProbability, float noiseProbability) = CalculateHeightAndNoiseProbability(prefab,
                        position, noiseMap, heightMultiplier, noiseImportance);
                    
                    // Sample true or false with the given probability
                    double randomNumber = random.NextDouble();
                    if (randomNumber < heightProbability & randomNumber < noiseProbability)
                        prefabsInternalData.AddPositionPrefabs(i, j);
                }
            }
        }

        private static (float, float) CalculateHeightAndNoiseProbability(PrefabsData.Prefab prefab, Vector3 position,
            float[,] noiseMap, float heightMultiplier, AnimationCurve noiseImportance)
        {
            float normalizedPositionX = Mathf.Abs(position.x) % ((noiseMap.GetLength (0) - 1) / 2f);
            float normalizedPositionZ = Mathf.Abs(position.z) % ((noiseMap.GetLength (1) - 1) / 2f);
            
            // Weighted sum between the position height and the noise of the prefab
            int positionInNoiseMapX = (int) Math.Round(normalizedPositionX - 
                                                       (noiseMap.GetLength (0) - 1) / -2f);
            int positionInNoiseMapZ = (int) Math.Round((noiseMap.GetLength (1) - 1) / 2f - 
                                                       normalizedPositionZ);
            float positionY = position[1] / heightMultiplier;

            float heightProbability = prefab.heightImportance.Evaluate(positionY);
            float noiseProbability = noiseImportance.Evaluate(noiseMap[positionInNoiseMapX, positionInNoiseMapZ]);

            return (heightProbability, noiseProbability);
        }

        private static void GeneratePrefabNoiseMap(PrefabsData.Prefab[] prefabs, PrefabsData prefabsData, int mapSize, 
            List<Transform>[] prefabsTransforms, String[] transformsNames, List<float[,]> noiseMaps)
        {
            for (int i = 0; i < prefabs.Length; ++i)
            {
                float[,] noiseMap = Noise.GenerateNoiseMap (mapSize, prefabsData.prefabs[i].noise.noiseSettings,
                    Vector2.zero);
                noiseMaps.Add(noiseMap);
                prefabsTransforms[i] = prefabs[i].transforms;
                transformsNames[i] = prefabs[i].name;
            }
        }

        private static void AddCabin(List<Transform>[] prefabsTransforms, PrefabsData.Prefab[] prefabs, 
            PrefabsData prefabsData, String[] transformsNames, Vector3[] positions, float heightMultiplier,
            Vector3[] oldPositions, System.Random random)
        {
            prefabsTransforms[prefabs.Length] = new List<Transform>();
            prefabsTransforms[prefabs.Length].Add(prefabsData.cabinPrefab.transform);
            transformsNames[prefabs.Length] = "cabin";
            
            Vector3 cabinPosition = DetermineCabinPosition(positions, prefabsData.cabinPrefab.heightImportance, 
                heightMultiplier, random, prefabsData.seed);
            positions[oldPositions.Length] = cabinPosition;
        }

        private static Vector3 DetermineCabinPosition(Vector3[] positions, AnimationCurve heightImportance, 
            float heightMultiplier, System.Random random, int seed)
        {
            List<Vector3> randomlySampledPositions = SampleRandomPositions(positions, random);
            
            int maxWeightIndex = 0;
            float totalWeight = -1;
            float[] weights = AssignWeightToPosition(randomlySampledPositions, heightImportance, heightMultiplier,
                ref maxWeightIndex, ref totalWeight);

            int selectedIndex = SelectIndexPosition(weights, seed, totalWeight);

            if (selectedIndex != 1)
                return randomlySampledPositions[selectedIndex];

            return randomlySampledPositions[maxWeightIndex];
        }

        private static float[] AssignWeightToPosition(List<Vector3> randomlySampledPositions, 
            AnimationCurve heightImportance, float heightMultiplier, ref int maxWeightIndex, ref float maxWeight)
        {
            float[] weights = new float[randomlySampledPositions.Count];
            float totalWeight = heightImportance.Evaluate(randomlySampledPositions[0][1] / heightMultiplier);
            maxWeight = totalWeight;
            
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

            return weights;
        }

        private static int SelectIndexPosition(float[] weights, int seed, float totalWeight)
        {
            // Sample one of the positions with the given weights (Fitness proportionate selection approach) 
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

            return selectedIndex;
        }

        private static List<Vector3> SampleRandomPositions(Vector3[] positions, System.Random random)
        {
            List<Vector3> randomlySampledPositions = new List<Vector3>();
            for (int i = 0; i < RandomCabinPositions; ++i)
            {
                randomlySampledPositions.Add(positions[random.Next(positions.Length)]);
            }

            return randomlySampledPositions;
        }
    }

    public class PrefabsInternalData
    {
        public readonly Vector3[] Positions;
        public readonly List<Transform>[] Transforms;
        public readonly String[] TransformsNames;
        public readonly List<Tuple<int, int>> PrefabsPositions;

        public PrefabsInternalData(Vector3[] positions, List<Transform>[] transforms, String[] transformsNames)
        {
            Positions = positions;
            Transforms = transforms;
            TransformsNames = transformsNames;
            PrefabsPositions = new List<Tuple<int, int>>();
        }

        public void AddPositionPrefabs(int positionIndex, int prefabsIndex)
        {
            PrefabsPositions.Add(new Tuple<int, int>(positionIndex, prefabsIndex));
        }
    }
}