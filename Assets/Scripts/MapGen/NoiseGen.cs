using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGen
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset){
        float[,] noiseMap = new float[mapWidth,mapHeight];
        
        // For seed, in case we need to generate the same map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        for(int i =0;i<octaves;i++){
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX,offsetY);
        }
        // Prevent noise scale from being negative.
        if (noiseScale<=0){
            noiseScale = 0.0001f;
        }
        //set to temp values. updated in loop
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i<octaves;i++){
                    // Higher frequency  = further apart samples meaning steeper slopes
                    float sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / noiseScale * frequency + octaveOffset[i].y;
                    //Perlin value in range -1 to 1
                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY)*2 -1;
                    noiseHeight += perlinValue*amplitude;
                    //Amplitude decreases per octave
                    amplitude *= persistance;
                    //Frequency increases per octave
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight){
                    maxNoiseHeight=noiseHeight;
                }else if(noiseHeight < minNoiseHeight){
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }
        // Normalize noisemap
        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                    noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);

            }
        }
        
        return noiseMap;
    }
}
