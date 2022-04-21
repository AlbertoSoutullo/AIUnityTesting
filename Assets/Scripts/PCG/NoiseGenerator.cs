using UnityEngine;

namespace PCG
{
    public static class NoiseGenerator {

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, 
            float persistance, float lacunarity, Vector2 offset) {
        
            float[,] noiseMap = new float[mapWidth, mapHeight];
            System.Random rng = new System.Random(seed);
            
            // Track variables to normalize again values between [-1,1]    
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
            
            Vector2[] octaveOffsets = NavigateThroughNoiseMap(octaves, offset, rng);
            
            scale = ClampScale(scale);
            
            noiseMap = FillNoiseMap(mapHeight, mapWidth, octaves, scale, persistance, lacunarity, octaveOffsets,
                noiseMap, ref maxNoiseHeight, ref minNoiseHeight);

            noiseMap = NormalizeNoiseMap(noiseMap, mapHeight, mapWidth, minNoiseHeight, maxNoiseHeight);

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
            float scale, float persistance, float lacunarity, Vector2[] octaveOffsets)
        {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;
            
            for (int i = 0; i < octaves; i++) {
                float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;
                        
                // Default PerlinNoise is [0,1], we do *2-1 to get in the range of [-1,1]
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }

            return noiseHeight;
        }

        private static float ClampNoiseHeight(float noiseHeight, ref float maxNoiseHeight, ref float minNoiseHeight)
        {
            if (noiseHeight > maxNoiseHeight) {
                maxNoiseHeight = noiseHeight;
            } else if (noiseHeight < minNoiseHeight) {
                minNoiseHeight = noiseHeight;
            }

            return noiseHeight;
        }

        private static float[,] FillNoiseMap(int mapHeight, int mapWidth, int octaves, float scale, float persistance,
            float lacunarity, Vector2[] octaveOffsets, float[,] noiseMap, 
            ref float maxNoiseHeight, ref float minNoiseHeight)
        {
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = CreateNoiseHeight(x, y, octaves, halfWidth, halfHeight,
                        scale, persistance, lacunarity, octaveOffsets);

                    noiseHeight = ClampNoiseHeight(noiseHeight, ref maxNoiseHeight, ref minNoiseHeight);
                    
                    noiseMap[x, y] = noiseHeight;
                }
            }

            return noiseMap;
        }

        private static float[,] NormalizeNoiseMap( float[,] noiseMap, int mapHeight, int mapWidth, 
            float minNoiseHeight, float maxNoiseHeight)
        {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
                }
            }

            return noiseMap;
        }

        private static Vector2[] NavigateThroughNoiseMap(int octaves, Vector2 offset, System.Random rng){
            Vector2[] octaveOffsets = new Vector2[octaves];
            
            for (int i = 0; i < octaves; i++) {
                float offsetX = rng.Next(-100000, 100000) + offset.x;
                float offsetY = rng.Next(-100000, 100000) + offset.y;
                octaveOffsets [i] = new Vector2(offsetX, offsetY);
            }

            return octaveOffsets;
        }
    }
}