using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGen
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float noiseScale){
        float[,] noiseMap = new float[mapWidth,mapHeight];

        if (noiseScale<=0){
            noiseScale = 0.0001f;
        }

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                 float sampleX = x / noiseScale;
                 float sampleY = y / noiseScale;

                float perlinValue = Mathf.PerlinNoise(sampleX,sampleY);
                noiseMap[x,y] = perlinValue;
            }
        }
        return noiseMap;
    }
}
