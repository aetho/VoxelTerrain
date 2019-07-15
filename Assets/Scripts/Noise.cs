using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int width, int depth, float scale, int octaves, float lacunarity, float persistence) {
        if (scale <= 0) scale = 0.0001f;

        float[,] noiseMap = new float[width, depth];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                // Generating noise
                for (int i = 0; i < octaves; i++) {
                    float sampleX = x / scale * frequency;
                    float sampleZ = z / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Keeping track of max noise height to normalize later on
                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, z] = noiseHeight;
            }
        }

        // Normalizing noise values
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }

        return noiseMap;
    }
}
