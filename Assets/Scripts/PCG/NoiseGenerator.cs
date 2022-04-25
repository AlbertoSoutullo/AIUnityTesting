using UnityEngine;

namespace PCG
{
    public static class NoiseGenerator
    {

        public enum NormalizeMode
        {
            Local,
            Global
        };

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, 
            float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) {
        
            float[,] noiseMap = new float[mapWidth, mapHeight];
            System.Random rng = new System.Random(seed);
            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;
            
            // Track variables to normalize again values between [-1,1]    
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
            
            Vector2[] octaveOffsets = NavigateThroughNoiseMap(octaves, offset, rng, amplitude, persistance, 
                ref maxPossibleHeight);
            
            scale = ClampScale(scale);
            
            noiseMap = FillNoiseMap(mapHeight, mapWidth, octaves, scale, persistance, lacunarity, octaveOffsets,
                noiseMap, ref maxNoiseHeight, ref minNoiseHeight, ref amplitude, ref frequency);

            noiseMap = NormalizeNoiseMap(noiseMap, mapHeight, mapWidth, minNoiseHeight, maxNoiseHeight, normalizeMode,
                maxPossibleHeight);

            return noiseMap;
        }

        private static float ClampScale(float scale)
        {
            if (scale <= 0) {
                scale = 0.0001f;
            }

            return scale;
        }

        private static float CreateNoiseHeight(int x, int y, int octaves, float halfWidth, float halfHeight,
            float scale, float persistance, float lacunarity, Vector2[] octaveOffsets, ref float amplitude, 
            ref float frequency)
        {
            amplitude = 1;
            frequency = 1;
            float noiseHeight = 0;
            
            for (int i = 0; i < octaves; i++) {
                float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency ;
                float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency ;
                        
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
            } else if (noiseHeight < minLocalNoiseHeight) {
                minLocalNoiseHeight = noiseHeight;
            }
            return noiseHeight;
        }

        private static float[,] FillNoiseMap(int mapHeight, int mapWidth, int octaves, float scale, float persistance,
            float lacunarity, Vector2[] octaveOffsets, float[,] noiseMap, 
            ref float maxNoiseHeight, ref float minNoiseHeight, ref float amplitude, ref float frequency)
        {
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = CreateNoiseHeight(x, y, octaves, halfWidth, halfHeight,
                        scale, persistance, lacunarity, octaveOffsets, ref amplitude, ref frequency);

                    noiseHeight = ClampNoiseHeight(noiseHeight, ref maxNoiseHeight, ref minNoiseHeight);
                    
                    noiseMap[x, y] = noiseHeight;
                }
            }

            return noiseMap;
        }

        private static float[,] NormalizeNoiseMap( float[,] noiseMap, int mapHeight, int mapWidth, 
            float minLocalNoiseHeight, float maxLocalNoiseHeight, NormalizeMode normalizeMode, 
            float maxPossibleNoiseHeight)
        {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);                        
                    } 
                    else
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleNoiseHeight / 2.5f);
                        noiseMap[x, y] = normalizedHeight;
                    }
                }
            }

            return noiseMap;
        }

        private static Vector2[] NavigateThroughNoiseMap(int octaves, Vector2 offset, System.Random rng, 
            float amplitude, float persistance, ref float maxPossibleHeight){
            Vector2[] octaveOffsets = new Vector2[octaves];
            
            for (int i = 0; i < octaves; i++) {
                float offsetX = rng.Next(-100000, 100000) + offset.x;
                float offsetY = rng.Next(-100000, 100000) - offset.y;
                octaveOffsets [i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            return octaveOffsets;
        }
    }
}