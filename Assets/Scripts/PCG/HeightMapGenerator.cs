// Unity Imports
using UnityEngine;

// Project Imports
using PCG.Data;

namespace PCG
{
    public static class HeightMapGenerator
    {
        private static float[,] _falloffMap;
        
        public static HeightMap GenerateHeightMap(int mapSize, HeightMapSettings settings, 
            Vector2 sampleCenter)
        {
            float[,] values = Noise.GenerateNoiseMap(mapSize, settings.noiseSettings, sampleCenter);
            AnimationCurve heightCurveThreadsafe = new AnimationCurve(settings.heightCurve.keys);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            
            if (settings.useFalloff)
                _falloffMap ??= FallOffGenerator.GenerateFallOffMap(mapSize);

            EvaluateWithHeightMap(heightCurveThreadsafe, mapSize, values, ref minValue, ref maxValue, settings);

            return new HeightMap(values);
        }

        private static void EvaluateWithHeightMap(AnimationCurve heightCurveThreadsafe, int mapSize, float[,] values, 
            ref float minValue, ref float maxValue, HeightMapSettings settings)
        {
            for (int i = 0; i < mapSize; i++) {
                for (int j = 0; j < mapSize; j++) 
                {
                    values[i, j] *= heightCurveThreadsafe.Evaluate(values[i, j] - (settings.useFalloff ? 
                        _falloffMap[i, j] : 0)) * settings.heightMultiplier;

                    if (values[i, j] > maxValue){
                        maxValue = values[i, j];
                    }
                    if (values[i, j] < minValue) {
                        minValue = values[i, j];
                    }
                }
            }
        }
    }
    
    
    
    public readonly struct HeightMap
    {
        public readonly float[,] Values;

        public HeightMap(float[,] values)
        {
            Values = values;
        }
    }
}