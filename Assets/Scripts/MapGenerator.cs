using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public int width = 100, depth = 100, height = 100;
    public float scale = 2f;
    public bool autoUpdate = true;
    public int octaves;
    public float lacunarity, persistence;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, depth, scale, octaves, lacunarity, persistence);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
        display.GenerateMesh(noiseMap, scale, height);
    }
}
