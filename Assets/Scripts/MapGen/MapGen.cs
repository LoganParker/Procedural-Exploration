using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
   
    [SerializeField]private int mapWidth;
    [SerializeField]private int mapHeight;
    [SerializeField]private float noiseScale;
    public bool autoUpdate;

    public void GenerateMap(){
        float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapWidth,mapHeight,noiseScale);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

}
