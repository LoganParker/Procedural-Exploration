using System;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    // Map Generator
    public float[,] GenerateNoiseMap(int mapHeight, int mapWidth, float scale, float offsetX, float offsetZ,Wave[] waves ,String NoiseType){        
        float[,] noiseMap = new float[mapHeight, mapWidth];

        for (int zIndex = 0; zIndex < mapHeight ; zIndex++){
            for(int xIndex = 0; xIndex < mapWidth; xIndex++){
                
                float x = (xIndex + offsetX)/ scale;
                float z = (zIndex + offsetZ)/ scale;
                float noise = 0f;
                float normalization = 0f;
                foreach(Wave wave in waves){
                    // Generate noise values for given waves
                    noise += wave.amplitude * NoiseGen(x*wave.frequency + wave.seed,z*wave.frequency + wave.seed,NoiseType);
                    normalization += wave.amplitude;
                }
                // Normalize noise so that it's between 0 and 1
                noise /= normalization;
                
                noiseMap[zIndex,xIndex] = noise;
            }
        }
        return noiseMap;
    }
    // Dictates which noise we are using for terrain gen
    private float NoiseGen(float x,float z,String type)
    {
        if(type == "perlin"){
            return Mathf.PerlinNoise(x,z);
        }
        else return -1;   
    }
}
[System.Serializable]
public class Wave {
    public float seed;
    public float frequency;
    public float amplitude;
}