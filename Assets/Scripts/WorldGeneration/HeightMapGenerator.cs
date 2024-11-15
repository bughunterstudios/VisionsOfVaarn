using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, Vector2 sampleCentre, float meshScale)
    {
        float[,] values = new float[width, height];
        Color[,] colors = new Color[width, height];

        float x;
        float y;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                x = (sampleCentre.x + i) * meshScale;
                y = (sampleCentre.y - j) * meshScale;
                values[i, j] = NoiseControl.NoiseMapHeight(x, y) * 100f;
                colors[i, j] = NoiseControl.NoiseColorMap(x, y);

                //values[i, j] = 0;
                //colors[i, j] = Color.blue;
            }
        }

        return new HeightMap(values, colors);
    }

}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly Color[,] colors;

    public HeightMap(float[,] values, Color[,] colors)
    {
        this.values = values;
        this.colors = colors;
    }
}