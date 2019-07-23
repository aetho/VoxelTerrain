using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    private Vector3[] vertices;
    private Mesh mesh;
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public void DrawNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int depth = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, depth);

        Color[] colourMap = new Color[width * depth];

        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                colourMap[z * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, z]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, depth);
    }

    private Color NormalColor(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public void DrawTerrainTexture(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int depth = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, depth);

        Color[] colourMap = new Color[width * depth];

        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                Color temp;

                if (noiseMap[x, z] < 0.3) { // Deep sea
                    temp = NormalColor(3, 57, 252);
                } else if (noiseMap[x, z] < 0.5) { // Surface water
                    temp = NormalColor(84, 190, 255);
                } else if (noiseMap[x, z] < 0.52) { // Sands
                    temp = NormalColor(232, 225, 146);
                } else if (noiseMap[x, z] < 0.8) { // Grass
                    temp = NormalColor(0, 161, 99);
                } else { // Snow 
                    temp = Color.white;
                }

                colourMap[z * width + x] = temp;
            }
        }

        // texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colourMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, depth);
    }

    public void GenerateMesh(float[,] noiseMap, float scale, float mapHeight) {
        int mapWidth = noiseMap.GetLength(0);
        int mapDepth = noiseMap.GetLength(1);

        mesh = new Mesh();
        mesh.name = "Terrain Mesh";
        meshFilter.mesh = mesh;

        vertices = new Vector3[(mapWidth + 1) * (mapDepth + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];

        for (int i = 0, z = 0; z <= mapDepth; z++) {
            for (int x = 0; x <= mapWidth; x++, i++) {
                float xPos = (x - mapWidth / 2);
                float zPos = (z - mapDepth / 2);
                float yPos;

                int xIndex = (x == mapWidth) ? x - 1 : x;
                int zIndex = (z == mapWidth) ? z - 1 : z;

                float threshold = 0.5f;
                if (noiseMap[xIndex, zIndex] < threshold) noiseMap[xIndex, zIndex] = threshold;

                // noiseMap[xIndex, zIndex] = (Mathf.Round(noiseMap[xIndex, zIndex] * 20.0f)) / 20.0f;

                yPos = noiseMap[xIndex, zIndex] * scale * mapHeight;

                vertices[i] = new Vector3(xPos, yPos, zPos);

                uv[i] = new Vector2((float)x / mapWidth, (float)z / mapDepth);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] triangles = new int[6 * mapWidth * mapDepth];
        for (int ti = 0, vi = 0, z = 0; z < mapDepth; z++, vi++) {
            for (int x = 0; x < mapWidth; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 4] = vi + mapWidth + 1;
                triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                triangles[ti + 5] = vi + mapWidth + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
