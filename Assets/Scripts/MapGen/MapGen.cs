using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public bool autoUpdate;

    //Map Parameters
    [SerializeField]private int mapWidth;
    [SerializeField]private int mapHeight;
    [SerializeField]private int seed;
    [SerializeField]private float noiseScale;
    [SerializeField]private int octaves;
    [SerializeField][Range(0,1)]private float persistance;
    [SerializeField]private float lacunarity;    
    [SerializeField]private Vector2 offset;
    //

    public void GenerateMap(){
        float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapWidth,mapHeight,seed,noiseScale,octaves,persistance,lacunarity,offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
    private void OnValidate() {
        if(mapWidth < 1){
            mapWidth = 1;
        }    
        if(mapHeight < 1){
            mapHeight = 1;
        }
        if(lacunarity<1){
            lacunarity=1;
        }
        if(octaves<0){
            octaves = 0;
        }
        
    }
}
