using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGen
{
    public enum NormalizeMode{Local,Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode){
        float[,] noiseMap = new float[mapWidth,mapHeight];
        
        // For seed, in case we need to generate the same map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        
        float maxPossibleHeight = 0;
                        
        float amplitude = 1;
        float frequency = 1;

        for(int i =0;i<octaves;i++){
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) - offset.y;
            octaveOffset[i] = new Vector2(offsetX,offsetY);

            maxPossibleHeight+=amplitude;
            amplitude*=persistance;
        }
        // Prevent noise scale from being negative.
        if (noiseScale<=0){
            noiseScale = 0.0001f;
        }
        //set to temp values. updated in loop
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i<octaves;i++){
                    // Higher frequency  = further apart samples meaning steeper slopes
                    float sampleX = (x - halfWidth + octaveOffset[i].x) / noiseScale * frequency ;
                    float sampleY = (y - halfHeight + octaveOffset[i].y) / noiseScale * frequency;
                    //Perlin value in range -1 to 1
                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY)*2 -1;
                    noiseHeight += perlinValue*amplitude;
                    //Amplitude decreases per octave
                    amplitude *= persistance;
                    //Frequency increases per octave
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxLocalNoiseHeight){
                    maxLocalNoiseHeight=noiseHeight;
                }else if(noiseHeight < minLocalNoiseHeight){
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }
        // Normalize noisemap
        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                    if(normalizeMode== NormalizeMode.Local){
                        //For non-endless terrain
                        noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]);
                    }
                    else{
                        float normalizedHeight = (noiseMap[x,y] + 1)/(maxPossibleHeight);
                        noiseMap[x,y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);

                    }
                }
            }
            return noiseMap;
        }
}
