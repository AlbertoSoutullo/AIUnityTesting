// Unity Imports
using UnityEngine;

namespace PCG
{
    public static class Noise
    {
        public enum NormalizeMode { Local, Global };

        public static float[,] GenerateNoiseMap(int mapSize, NoiseSettings settings, Vector2 sampleCentre) 
        {
            float[,] noiseMap = new float[mapSize, mapSize];
            System.Random rng = new System.Random(settings.seed);
            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;
            
            // Track variables to normalize again values between [-1,1]    
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
            
            Vector2[] octaveOffsets = NavigateThroughNoiseMap(settings.octaves, settings.offset, rng, ref amplitude, 
                settings.persistance, ref maxPossibleHeight, sampleCentre);

            noiseMap = FillNoiseMap(mapSize, settings, octaveOffsets, noiseMap, ref maxNoiseHeight, 
                ref minNoiseHeight, ref amplitude, ref frequency, maxPossibleHeight);

            noiseMap = NormalizeNoiseMap(noiseMap, mapSize, minNoiseHeight, maxNoiseHeight, settings.normalizeMode);

            return noiseMap;
        }

        private static float CreateNoiseHeight(int x, int y, int octaves, float halfSize, float scale, 
            float persistance, float lacunarity, Vector2[] octaveOffsets, ref float amplitude, 
            ref float frequency)
        {
            amplitude = 1;
            frequency = 1;
            float noiseHeight = 0;
            
            for (int i = 0; i < octaves; i++) {
                float sampleX = (x-halfSize + octaveOffsets[i].x) / scale * frequency ;
                float sampleY = (y-halfSize + octaveOffsets[i].y) / scale * frequency ;
                        
                // Default PerlinNoise is [0,1], we do *2-1 to get in the range of [-1,1]
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }

            return noiseHeight;
        }

        private static float ClampNoiseHeight(float noiseHeight, ref float maxLocalNoiseHeight, ref float minLocalNoiseHeight)
        {
            if (noiseHeight > maxLocalNoiseHeight) {
                maxLocalNoiseHeight = noiseHeight;
            } 
            if (noiseHeight < minLocalNoiseHeight) {
                minLocalNoiseHeight = noiseHeight;
            }
            
            return noiseHeight;
        }

        private static float[,] FillNoiseMap(int mapSize, NoiseSettings settings, Vector2[] octaveOffsets, float[,] noiseMap, 
            ref float maxNoiseHeight, ref float minNoiseHeight, ref float amplitude, ref float frequency, float maxPossibleNoiseHeight)
        {
            float halfSize = mapSize / 2f;

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++)
                {
                    float noiseHeight = CreateNoiseHeight(x, y, settings.octaves, halfSize, settings.scale, 
                        settings.persistance, settings.lacunarity, octaveOffsets, ref amplitude, ref frequency);

                    noiseHeight = ClampNoiseHeight(noiseHeight, ref maxNoiseHeight, ref minNoiseHeight);
                    
                    noiseMap[x, y] = noiseHeight;

                    if (settings.normalizeMode == NormalizeMode.Global) {
                        float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleNoiseHeight / 0.9f);
                        noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            return noiseMap; 
        }

        private static float[,] NormalizeNoiseMap(float[,] noiseMap, int mapSize, float minLocalNoiseHeight, 
            float maxLocalNoiseHeight, NormalizeMode normalizeMode)
        {
            if (normalizeMode == NormalizeMode.Local) {
                for (int y = 0; y < mapSize; y++) {
                    for (int x = 0; x < mapSize; x++) {
                        noiseMap[x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, 
                            noiseMap [x, y]);
                    }
                }
            }

            return noiseMap;
        }

        private static Vector2[] NavigateThroughNoiseMap(int octaves, Vector2 offset, System.Random rng, 
            ref float amplitude, float persistance, ref float maxPossibleHeight, Vector2 sampleCentre){
            Vector2[] octaveOffsets = new Vector2[octaves];
            
            for (int i = 0; i < octaves; i++) {
                float offsetX = rng.Next(-100000, 100000) + offset.x + sampleCentre.x;
                float offsetY = rng.Next(-100000, 100000) - offset.y - sampleCentre.y;
                octaveOffsets [i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            return octaveOffsets;
        }
    }

    [System.Serializable]
    public class NoiseSettings
    {
        public Noise.NormalizeMode normalizeMode;
    
        public float scale = 50;

        public int octaves = 5;
        [Range(0,1)]
        public float persistance = .5f;
        public float lacunarity = 2;

        public int seed;
        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}